Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class ZonexParticelle_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Zona_Cod As Integer,
                          ByVal PROV As String,
                          ByVal COM As String,
                          ByVal SEZIONE As String,
                          ByVal FOGLIO As Integer,
                          ByVal NUMERO As Integer,
                          ByVal SUBALTERNO As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ZonexParticelle_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable = Nothing

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM ZonexParticelle ")
                    StrSQL.Append(" WHERE   (ZonexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    StrSQL.Append(" AND     (ZonexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
                    StrSQL.Append(" AND     (ZonexParticelle.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "') ")

                    '----- Condizioni

                    If Zona_Cod <> 0 Then
                        StrSQL.Append(" AND ZonexParticelle.Zona_Cod = " & Agro_SQL_SaveNum(Zona_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        StrSQL.Append(" AND ZonexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND ZonexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND ZonexParticelle.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND ZonexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND ZonexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND ZonexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  DISTINCT ParticelleCatastali.PART_COD, ")
                    StrSQL.AppendLine(" ParticelleCatastali.PROV, ")
                    StrSQL.AppendLine(" ParticelleCatastali.COM, ")
                    StrSQL.AppendLine(" ParticelleCatastali.SEZIONE, ")
                    StrSQL.AppendLine(" ParticelleCatastali.FOGLIO, ")
                    StrSQL.AppendLine(" ParticelleCatastali.NUMERO, ")
                    StrSQL.AppendLine(" ParticelleCatastali.SUBALTERNO, ")
                    StrSQL.AppendLine(" ParticelleCatastali.PARTITA_CATASTALE, ")
                    StrSQL.AppendLine(" ParticelleCatastali.ETTARI, ")
                    StrSQL.AppendLine(" ParticelleCatastali.ARE, ")
                    StrSQL.AppendLine(" ParticelleCatastali.CENTIARE, ")
                    StrSQL.AppendLine(" ParticelleCatastali.TitoloPossesso, ")
                    StrSQL.AppendLine(" ISNULL(Zone.Zona_Cod,0) AS Zona_Cod, ISNULL(Zone.Descrizione,'') AS Descrizione, ISNULL(ZonexParticelle.Area,0) AS Area,")
                    StrSQL.AppendLine(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Lista_Province.PROVINCIA, ")
                    StrSQL.AppendLine(" ZonexParticelle.Validita_Inizio,  ")
                    StrSQL.AppendLine(" ZonexParticelle.Validita_Fine  ")

                    StrSQL.AppendLine(" FROM  Zone INNER JOIN  ")
                    StrSQL.AppendLine(" ZonexParticelle ON Zone.Zona_Cod = ZonexParticelle.Zona_Cod INNER JOIN ")
                    StrSQL.AppendLine(" ParticelleCatastali INNER JOIN ")
                    StrSQL.AppendLine(" ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV AND ParticelleCatastali.COM = ISTAT.COM INNER JOIN ")
                    StrSQL.AppendLine(" Lista_Province ON ISTAT.PROV = Lista_Province.PROV ON ZonexParticelle.PROV = ParticelleCatastali.PROV AND  ")
                    StrSQL.AppendLine(" ZonexParticelle.COM = ParticelleCatastali.COM AND ZonexParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND  ")
                    StrSQL.AppendLine(" ZonexParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND ZonexParticelle.NUMERO = ParticelleCatastali.NUMERO AND  ")
                    StrSQL.AppendLine(" ZonexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

                    StrSQL.AppendLine(" WHERE   (ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    StrSQL.AppendLine(" AND     (ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
                    StrSQL.AppendLine(" AND     (ZonexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    StrSQL.AppendLine(" AND     (ZonexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
                    StrSQL.AppendLine(" AND     (ZonexParticelle.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "') ")

                    '----- Condizioni

                    If Zona_Cod <> 0 Then
                        StrSQL.AppendLine(" AND ZonexParticelle.Zona_Cod = " & Agro_SQL_SaveNum(Zona_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        StrSQL.AppendLine(" AND ZonexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.AppendLine(" AND ZonexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.AppendLine(" AND ZonexParticelle.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.AppendLine(" AND ZonexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.AppendLine(" AND ZonexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.AppendLine(" AND ZonexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinCompleta



            End Select

            If StrSQL.ToString <> "" Then

                '--------------------------------------------------------------------------
                If xFiltroAggiuntivo <> "" Then
                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If
                '--------------------------------------------------------------------------
                Select Case objParametri.FlagVisibilita
                    Case enumVisibilita.Visibilita_SoloNonCancellati
                        StrSQL.Append(" AND   ZonexParticelle.Inviato >=0 ")
                    Case enumVisibilita.Visibilita_SoloCancellati
                        StrSQL.Append(" AND   ZonexParticelle.Inviato =-1 ")
                    Case enumVisibilita.Visibilita_Tutti
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

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '##############################################################################################
    Public Function ImpresaInZona(ByVal PivaSuperUser As String,
                                  ByVal Piva As String,
                                  ByVal Zona_Cod As Integer,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ZonexParticelle_R.ImpresaInZona()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT COUNT(*) ")
            StrSQL.Append(" FROM  ZonexParticelle INNER JOIN ")
            StrSQL.Append(" ImpreseXParticelle ON ZonexParticelle.PROV = ImpreseXParticelle.PROV AND ZonexParticelle.COM = ImpreseXParticelle.COM AND ")
            StrSQL.Append(" ZonexParticelle.SEZIONE = ImpreseXParticelle.SEZIONE AND ZonexParticelle.FOGLIO = ImpreseXParticelle.FOGLIO AND ")
            StrSQL.Append(" ZonexParticelle.NUMERO = ImpreseXParticelle.NUMERO And ZonexParticelle.SUBALTERNO = ImpreseXParticelle.SUBALTERNO ")

            StrSQL.Append(" WHERE ImpreseXParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If PivaSuperUser <> "" Then
                StrSQL.Append(" AND ZonexParticelle.Piva_SuperUser = '" & Replace(PivaSuperUser, "'", "''") & "' ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND ImpreseXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Zona_Cod <> 0 Then
                StrSQL.Append(" AND ZonexParticelle.Zona_Cod =  " & Agro_SQL_SaveNum(Zona_Cod) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   ZonexParticelle.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   ZonexParticelle.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' se esiste almeno una particella dell'azienda in Zona_Cod, restituisco 0
            bRet = False
            If DT.Rows.Count > 0 Then
                bRet = True
            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return bRet

    End Function

    '##############################################################################################
    Public Function AppezzamentoInZona(ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Appezza As Integer,
                                       ByVal Zona_Cod As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ZonexParticelle_R.AppezzamentoInZona()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT COUNT(*) ")
            StrSQL.Append(" FROM ZonexParticelle INNER JOIN ")
            StrSQL.Append(" AppezzamentiXParticelle ON ZonexParticelle.PROV = AppezzamentiXParticelle.PROV AND ZonexParticelle.COM = AppezzamentiXParticelle.COM AND ")
            StrSQL.Append(" ZonexParticelle.SEZIONE = AppezzamentiXParticelle.SEZIONE AND ZonexParticelle.FOGLIO = AppezzamentiXParticelle.FOGLIO AND ")
            StrSQL.Append(" ZonexParticelle.NUMERO = AppezzamentiXParticelle.NUMERO And ZonexParticelle.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO ")

            StrSQL.Append(" WHERE AppezzamentiXParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND ZonexParticelle.Piva_SuperUser = '" & Replace(objParametri.PivaSuperUser, "'", "''") & "' ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND AppezzamentiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND AppezzamentiXParticelle.Sa_Cod = '" & Agro_SQL_SaveText(Sa_Cod) & "'")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND AppezzamentiXParticelle.Appezza = '" & Agro_SQL_SaveText(Appezza) & "'")
            End If

            If Zona_Cod <> 0 Then
                StrSQL.Append(" AND ZonexParticelle.Zona_Cod =  " & Agro_SQL_SaveNum(Zona_Cod) & " ")
            End If



            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   ZonexParticelle.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   ZonexParticelle.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' se esiste almeno una particella dell'azienda in Zona_Cod, restituisco 0
            bRet = False
            If DT.Rows.Count > 0 Then
                bRet = True
            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return bRet

    End Function

    '##############################################################################################
    Public Function Esiste_ZonaxParticella(ByVal PROV As String,
                                           ByVal COM As String,
                                           ByVal SEZIONE As String,
                                           ByVal FOGLIO As Integer,
                                           ByVal NUMERO As Integer,
                                           ByVal SUBALTERNO As String,
                                           ByVal Zona_Cod As Integer,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ZonexParticelle_R.Esiste_ZonaxParticella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim bRet As Boolean = False

        Try
            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            If PROV = "" Then
                Throw New Exception("Parametro non corretto nella query (Provincia obbligatoria)")
            End If

            If COM = "" Then
                Throw New Exception("Parametro non corretto nella query (Comune obbligatorio)")
            End If

            If SEZIONE = "" Then
                Throw New Exception("Parametro non corretto nella query (Sezione obbligatoria)")
            End If

            If FOGLIO = 0 Then
                Throw New Exception("Parametro non corretto nella query (Foglio obbligatorio)")
            End If

            If NUMERO = 0 Then
                Throw New Exception("Parametro non corretto nella query (Numero obbligatorio)")
            End If

            If SUBALTERNO = "" Then
                Throw New Exception("Parametro non corretto nella query (Subalterno obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  ZonexParticelle ")
            StrSQL.Append(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append(" AND Zona_Cod = " & Agro_SQL_SaveNum(Zona_Cod))

            StrSQL.Append(" AND PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.Append(" AND COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.Append(" AND SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            StrSQL.Append(" AND FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            StrSQL.Append(" AND NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            StrSQL.Append(" AND SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")



            End Select
            '---------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            If DT IsNot Nothing Then
                If DT.Rows.Count > 0 Then
                    bRet = True
                Else
                    bRet = False
                End If
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        DT = Nothing
        Return bRet

    End Function

    '##############################################################################################
    Public Function ZoneCentro_Leggi(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal PROV As String,
                                     ByVal COM As String,
                                     ByVal SEZIONE As String,
                                     ByVal FOGLIO As Integer,
                                     ByVal NUMERO As Integer,
                                     ByVal SUBALTERNO As String,
                                     ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByVal xOrderBy As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ZonexParticelle_R.NewCom_ZoneCentro_Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  ParticelleCatastali.*, ")
                    StrSQL.Append(" ImpresexParticelle.Validita_Inizio as xValidita_Inizio, ")
                    StrSQL.Append(" ImpresexParticelle.Validita_Fine as xValidita_Fine, ")
                    StrSQL.Append(" ImpresexParticelle.TitoloPossesso as xTitoloPossesso, ")
                    StrSQL.Append(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Lista_Province.PROVINCIA, ")
                    StrSQL.Append(" ISNULL(ZonexParticelle.Zona_Cod,0) AS Zona_Cod, ISNULL(Zone.Descrizione,'') AS Descrizione ")

                    StrSQL.Append(" FROM Zone INNER JOIN ")
                    StrSQL.Append(" ZonexParticelle ON Zone.Zona_Cod = ZonexParticelle.Zona_Cod RIGHT OUTER JOIN ")
                    StrSQL.Append(" ImpreseXParticelle INNER JOIN ")
                    StrSQL.Append(" ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM AND ")
                    StrSQL.Append(" ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND ")
                    StrSQL.Append(" ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO INNER JOIN ")
                    StrSQL.Append(" ISTAT ON ImpreseXParticelle.PROV = ISTAT.PROV AND ImpreseXParticelle.COM = ISTAT.COM INNER JOIN ")
                    StrSQL.Append(" Lista_Province ON ISTAT.PROV = Lista_Province.PROV ON ZonexParticelle.PROV = ImpreseXParticelle.PROV AND ")
                    StrSQL.Append(" ZonexParticelle.COM = ImpreseXParticelle.COM AND ZonexParticelle.SEZIONE = ImpreseXParticelle.SEZIONE AND ")
                    StrSQL.Append(" ZonexParticelle.FOGLIO = ImpreseXParticelle.FOGLIO AND ZonexParticelle.NUMERO = ImpreseXParticelle.NUMERO AND ")
                    StrSQL.Append(" ZonexParticelle.SUBALTERNO = ImpreseXParticelle.SUBALTERNO ")

                    StrSQL.Append(" WHERE   (ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    StrSQL.Append(" AND     (ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
                    StrSQL.Append(" AND     (ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    StrSQL.Append(" AND     (ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")


                    If (Piva <> "") Then
                        StrSQL.Append(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        StrSQL.Append(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   ZonexParticelle.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   ZonexParticelle.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
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

End Class

'############################################################################################
'############################################################################################
'############################################################################################
'############################################################################################
'############################################################################################
'############################################################################################
'############################################################################################



Public Class ZonexParticelle_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi(ByVal Zona_Cod As Integer,
                           ByVal PROV As String,
                           ByVal COM As String,
                           ByVal SEZIONE As String,
                           ByVal FOGLIO As Integer,
                           ByVal NUMERO As Integer,
                           ByVal SUBALTERNO As String,
                           ByVal Area As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.ZonexParticelle_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO ZonexParticelle ")

            StrSQL.Append(" (Piva_SuperUser, Zona_Cod, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, Area, ")
            StrSQL.Append(" Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ) ")

            StrSQL.Append(" VALUES ('" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "', ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(Zona_Cod)) & ", ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PROV)) & "', ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(COM)) & "', ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(SEZIONE)) & "', ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(FOGLIO)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(NUMERO)) & ", ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(SUBALTERNO)) & "', ")
            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(Area)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveDateTime(Now) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveDateTime(Now) & ", ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(objParametri.UsernameOperazione)) & "', ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(objParametri.UsernameOperazione)) & "', ")
            StrSQL.Append(" " & Agro_SQL_SaveDate(Trim(Validita_Inizio)) & ", ")
            StrSQL.Append(" " & Agro_SQL_SaveDate(Trim(Validita_Fine)) & " ) ")
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = "Particella: " & PROV & " " & COM & " " & SEZIONE & " " & CStr(FOGLIO) & " " & CStr(NUMERO) & " " & SUBALTERNO & vbCrLf
            MessaggioErrore &= "Zona: " & CStr(Zona_Cod) & vbCrLf
            MessaggioErrore &= "Errore: " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    '============================================================================
    Public Function Cancella(ByVal Zona_Cod As Integer,
                             ByVal PROV As String,
                             ByVal COM As String,
                             ByVal SEZIONE As String,
                             ByVal FOGLIO As Integer,
                             ByVal NUMERO As Integer,
                             ByVal SUBALTERNO As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ZonexParticelle_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.Append(" DELETE FROM ZonexParticelle ")
            StrSQL.Append(" WHERE  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Zona_Cod <> 0 Then
                StrSQL.Append(" AND    Zona_Cod = " & Agro_SQL_SaveNum(Zona_Cod) & " ")
            End If

            If PROV <> "" Then
                StrSQL.Append(" AND    PROV = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If
            If COM <> "" Then
                StrSQL.Append(" AND    COM = '" & Agro_SQL_SaveText(COM) & "' ")
            End If
            If SEZIONE <> "" Then
                StrSQL.Append(" AND    SEZIONE = '" & Agro_SQL_SaveText(SEZIONE) & "' ")
            End If
            If FOGLIO <> -1 Then
                StrSQL.Append(" AND    FOGLIO = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If
            If NUMERO <> -1 Then
                StrSQL.Append(" AND    NUMERO = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If
            If SUBALTERNO <> "" Then
                StrSQL.Append(" AND    SUBALTERNO = '" & Agro_SQL_SaveText(SUBALTERNO) & "' ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = "Particella: " & PROV & " " & COM & " " & SEZIONE & " " & CStr(FOGLIO) & " " & CStr(NUMERO) & " " & SUBALTERNO & vbCrLf
            MessaggioErrore &= "Zona: " & CStr(Zona_Cod) & vbCrLf
            MessaggioErrore &= "Errore: " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function
    
    '============================================================================
    Public Function Modifica(ByVal Zona_Cod As Integer,
                             ByVal PROV As String,
                             ByVal COM As String,
                             ByVal SEZIONE As String,
                             ByVal FOGLIO As Integer,
                             ByVal NUMERO As Integer,
                             ByVal SUBALTERNO As String,
                             ByVal Area As Decimal,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.ZonexParticelle_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Zona_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Zona_Cod obbligatorio)")
            End If

            If PROV = "" Then
                Throw New Exception("Parametro non corretto nella query (PROV obbligatorio)")
            End If

            If COM = "" Then
                Throw New Exception("Parametro non corretto nella query (COM obbligatorio)")
            End If

            If SEZIONE = "" Then
                Throw New Exception("Parametro non corretto nella query (SEZIONE obbligatorio)")
            End If

            If FOGLIO = 0 Then
                Throw New Exception("Parametro non corretto nella query (FOGLIO obbligatorio)")
            End If

            If NUMERO = 0 Then
                Throw New Exception("Parametro non corretto nella query (NUMERO obbligatorio)")
            End If

            If SUBALTERNO = "" Then
                Throw New Exception("Parametro non corretto nella query (SUBALTERNO obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE ZonexParticelle SET ")

            StrSQL.Append("   Area              =  " & Agro_SQL_SaveNum(Area) & " ")
            StrSQL.Append("   ,Validita_Inizio  =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine    =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE    PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.Append(" AND      Sezione     = '" & Agro_SQL_SaveText(SEZIONE) & "'  ")
            StrSQL.Append(" AND      FOGLIO      = " & Agro_SQL_SaveNum(FOGLIO) & "  ")
            StrSQL.Append(" AND      Numero      = " & Agro_SQL_SaveNum(NUMERO) & "  ")
            StrSQL.Append(" AND      SUBALTERNO =  '" & Agro_SQL_SaveText(SUBALTERNO) & "' ")
            StrSQL.Append(" AND      Zona_Cod      = " & Agro_SQL_SaveNum(Zona_Cod) & "  ")

            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = "Particella: " & PROV & " " & COM & " " & SEZIONE & " " & CStr(FOGLIO) & " " & CStr(NUMERO) & " " & SUBALTERNO & vbCrLf
            MessaggioErrore &= "Zona: " & CStr(Zona_Cod) & vbCrLf
            MessaggioErrore &= "Errore: " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
