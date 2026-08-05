Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class ParticelleCatastalixMacrousixUtilizzo_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '####################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal PROV As String,
                          ByVal COM As String,
                          ByVal SEZIONE As String,
                          ByVal FOGLIO As Int32,
                          ByVal NUMERO As Int32,
                          ByVal SUBALTERNO As String,
                          ByVal Macrouso_Cod As String,
                          ByVal Veg_Cod_Agea As String,
                          ByVal Cul_Cod_Agea As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal Numero_Fascicolo As String = "",
                          Optional ByVal Data_Validazione_Fascicolo As Date = AGRODATAINIZIO
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Part_Cod = ""
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = ""
        '   Macrouso_Cod = ""
        '   Veg_Cod = ""
        '   Cul_Cod = ""
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, MACROUSO_COD, VEG_COD_AGEA, CUL_COD_AGEA, SUPERFICIE, Validita_Inizio, Validita_Fine ")
                    StrSQL.Append(" FROM  ParticelleCatastalixMacrousixUtilizzo ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If PROV <> "" Then
                        StrSQL.Append(" AND PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    If Macrouso_Cod <> "" Then
                        StrSQL.Append(" AND Macrouso_Cod =  '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
                    End If

                    If Veg_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Agea =  '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
                    End If
                    If Cul_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Cul_Cod_Agea =  '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
                    End If

                    If Numero_Fascicolo <> "" Then
                        StrSQL.Append(" AND Numero_Fascicolo =  '" & Agro_SQL_SaveText(Numero_Fascicolo) & "' ")
                    End If

                    If Data_Validazione_Fascicolo <> AGRODATAINIZIO Then
                        StrSQL.Append(" AND Data_Validazione_Fascicolo =  " & Agro_SQL_SaveDateTime(Data_Validazione_Fascicolo) & " ")
                    End If

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
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Validita_inizio ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  ParticelleCatastalixMacrousixUtilizzo ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PROV <> "" Then
                        StrSQL.Append(" AND PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    If Macrouso_Cod <> "" Then
                        StrSQL.Append(" AND Macrouso_Cod =  '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
                    End If

                    If Veg_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Agea =  '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
                    End If
                    If Cul_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Cul_Cod_Agea =  '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
                    End If

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
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Validita_inizio ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  ParticelleCatastalixMacrousixUtilizzo.* ,  ISNULL(Codifica_SpecieVegetali_Agea.Veg_Des_Agea,'') as Veg_Des_Agea, ISNULL(Codifica_SpecieVegetali_Agea.Cul_Des_Agea,'') as Cul_Des_Agea ")

                    StrSQL.Append(" FROM    ParticelleCatastalixMacrousixUtilizzo LEFT OUTER JOIN ")
                    StrSQL.Append("         Codifica_SpecieVegetali_Agea ON ParticelleCatastalixMacrousixUtilizzo.Veg_Cod_Agea = Codifica_SpecieVegetali_Agea.Veg_Cod_Agea AND  ")
                    StrSQL.Append("         ParticelleCatastalixMacrousixUtilizzo.Cul_Cod_Agea = Codifica_SpecieVegetali_Agea.Cul_Cod_Agea ")

                    StrSQL.Append(" WHERE ParticelleCatastalixMacrousixUtilizzo.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   ParticelleCatastalixMacrousixUtilizzo.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PROV <> "" Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If
                    If COM <> "" Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If
                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If
                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If
                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If
                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    If Macrouso_Cod <> "" Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.Macrouso_Cod =  '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
                    End If

                    If Veg_Cod_Agea <> "" Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.Veg_Cod_Agea =  '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
                    End If
                    If Cul_Cod_Agea <> "" Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.Cul_Cod_Agea =  '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   ParticelleCatastalixMacrousixUtilizzo.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   ParticelleCatastalixMacrousixUtilizzo.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY ParticelleCatastalixMacrousixUtilizzo.Validita_inizio ASC")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta


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

    '#####################################################################
    Public Function Esiste_PartxMacrousoxUtilizzo(ByVal Piva As String,
                                                  ByVal PROV As String,
                                                ByVal COM As String,
                                                ByVal SEZIONE As String,
                                                ByVal FOGLIO As Int32,
                                                ByVal NUMERO As Int32,
                                                ByVal SUBALTERNO As String,
                                                ByVal Macrouso_Cod As String,
                                                ByVal Veg_Cod_Agea As String,
                                                ByVal Cul_Cod_Agea As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R.Esiste_PartxMacrousoxUtilizzo()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim Flag_Esiste As Boolean = False

        Try
            '---------------------------------------------

            DT = Leggi(Piva, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO,
                       Macrouso_Cod,
                       Veg_Cod_Agea, Cul_Cod_Agea,
                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                       xFiltroAggiuntivo, "", objParametri)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                Flag_Esiste = True
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Flag_Esiste

    End Function

    Public Function Leggi_DaCentro(
                        ByVal Piva As String,
                        ByVal Sa_Cod As Integer,
                        ByVal PROV As String,
                        ByVal COM As String,
                        ByVal SEZIONE As String,
                        ByVal FOGLIO As Int32,
                        ByVal NUMERO As Int32,
                        ByVal SUBALTERNO As String,
                        ByVal Macrouso_Cod As String,
                        ByVal Veg_Cod_Agea As String,
                        ByVal Cul_Cod_Agea As String,
                            ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R.Leggi_DaCentro()"

        '====================================================================================
        'Parametri opzionali :
        '   Part_Cod = ""
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = ""
        '   Macrouso_Cod = ""
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT ParticelleCatastalixMacrousixUtilizzo.* , Macrousi.Macrouso_Des as Macrouso_Des ")
            StrSQL.Append(" FROM    ImpreseXParticelle INNER JOIN ")
            StrSQL.Append(" ParticelleCatastalixMacrousixUtilizzo ON ImpreseXParticelle.PROV = ParticelleCatastalixMacrousixUtilizzo.PROV AND ImpreseXParticelle.COM = ParticelleCatastalixMacrousixUtilizzo.COM AND ")
            StrSQL.Append(" ImpreseXParticelle.SEZIONE = ParticelleCatastalixMacrousixUtilizzo.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastalixMacrousixUtilizzo.FOGLIO AND  ")
            StrSQL.Append(" ImpreseXParticelle.NUMERO = ParticelleCatastalixMacrousixUtilizzo.NUMERO AND  ")
            StrSQL.Append(" ImpreseXParticelle.SUBALTERNO = ParticelleCatastalixMacrousixUtilizzo.SUBALTERNO INNER JOIN ")
            StrSQL.Append(" Macrousi ON ParticelleCatastalixMacrousixUtilizzo.Macrouso_Cod = Macrousi.Macrouso_Cod ")

            StrSQL.Append(" WHERE ParticelleCatastalixMacrousixUtilizzo.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   ParticelleCatastalixMacrousixUtilizzo.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND ImpreseXParticelle.piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND ImpreseXParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If PROV <> "" Then
                StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If Macrouso_Cod <> "" Then
                StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.Macrouso_Cod =  '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
            End If

            If Veg_Cod_Agea <> "" Then
                StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.Veg_Cod_Agea =  '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
            End If

            If Cul_Cod_Agea <> "" Then
                StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.Cul_Cod_Agea =  '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   ParticelleCatastalixMacrousixUtilizzo.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   ParticelleCatastalixMacrousixUtilizzo.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY ParticelleCatastalixMacrousixUtilizzo.Validita_inizio ASC")
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


    ' #############################################################################################################
    Public Function Leggi_Utilizzi(ByVal Piva As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R.Leggi_Utilizzi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT ParticelleCatastalixMacrousixUtilizzo.Veg_Cod_Agea, Codifica_SpecieVegetali_Agea.Veg_Des_Agea ")
            StrSQL.Append(" FROM ParticelleCatastalixMacrousixUtilizzo ")
            StrSQL.Append(" INNER JOIN Codifica_SpecieVegetali_Agea ON ParticelleCatastalixMacrousixUtilizzo.Veg_Cod_Agea = Codifica_SpecieVegetali_Agea.Veg_Cod_Agea ")

            StrSQL.Append(" WHERE ParticelleCatastalixMacrousixUtilizzo.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   ParticelleCatastalixMacrousixUtilizzo.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND ParticelleCatastalixMacrousixUtilizzo.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   ParticelleCatastalixMacrousixUtilizzo.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   ParticelleCatastalixMacrousixUtilizzo.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Veg_Des_Agea ASC")
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




Public Class ParticelleCatastalixMacrousixUtilizzo_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Scrivi(ByVal PIVA As String,
                           ByVal PROV As String,
                           ByVal COM As String,
                           ByVal SEZIONE As String,
                           ByVal FOGLIO As Integer,
                           ByVal NUMERO As Integer,
                           ByVal SUBALTERNO As String,
                           ByVal Macrouso_Cod As String,
                           ByVal Veg_Cod_Agea As String,
                           ByVal Cul_Cod_Agea As String,
                           ByVal Superficie As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Numero_Fascicolo As String = "",
                           Optional ByVal Data_Validazione_Fascicolo As Date = AGRODATAINIZIO,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO ParticelleCatastalixMacrousixUtilizzo(       ")
            StrSQL.Append("             PIVA,                                            ")
            StrSQL.Append("             PROV,               COM,            SEZIONE,     ")
            StrSQL.Append("             FOGLIO,             NUMERO,         SUBALTERNO,  ")
            StrSQL.Append("             Macrouso_Cod,   Veg_Cod_Agea, Cul_Cod_Agea,                ")
            StrSQL.Append("             Superficie,                  ")
            StrSQL.Append("             Numero_Fascicolo,   Data_Validazione_Fascicolo,                  ")

            StrSQL.Append("             Inviato,            DataInvio, ")
            StrSQL.Append("             Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("             UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("             Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("             ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(PIVA)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(PROV)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  ")
            StrSQL.Append("         ,'" & If(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append("         , " & If(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append("         , " & If(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append("         ,'" & If(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Macrouso_Cod) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Superficie) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Numero_Fascicolo) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Validazione_Fascicolo) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.Append(") ")

            '---------------------------------------------


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = "Particella: " & PROV & " " & COM & " " & SEZIONE & " " & CStr(FOGLIO) & " " & CStr(NUMERO) & " " & SUBALTERNO & vbCrLf
            MessaggioErrore &= "Macrouso: " & CStr(Macrouso_Cod) & vbCrLf
            MessaggioErrore &= "Codice Specie: " & CStr(Veg_Cod_Agea) & vbCrLf
            MessaggioErrore &= "Codice Varietà: " & CStr(Cul_Cod_Agea) & vbCrLf
            MessaggioErrore &= "Errore: " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Modifica(ByVal PIVA As String,
                             ByVal PROV As String,
                             ByVal COM As String,
                             ByVal SEZIONE As String,
                             ByVal FOGLIO As Int32,
                             ByVal NUMERO As Int32,
                             ByVal SUBALTERNO As String,
                             ByVal Macrouso_Cod As String,
                             ByVal Veg_Cod_Agea As String,
                             ByVal Cul_Cod_Agea As String,
                             ByVal Superficie As Decimal,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Macrouso_Cod = "" Then
                Throw New Exception("Parametro non corretto nella query (Macrouso_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE ParticelleCatastalixMacrousixUtilizzo SET ")
            StrSQL.Append("    Superficie        =  " & Agro_SQL_SaveNum(Superficie))
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            StrSQL.Append(" AND      PROV         = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.Append(" AND      Sezione     = '" & If(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append(" AND      FOGLIO      =  " & If(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append(" AND      Numero      =  " & If(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append(" AND      SUBALTERNO  = '" & If(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

            StrSQL.Append(" AND      Macrouso_Cod = '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
            StrSQL.Append(" AND      Veg_Cod_Agea  = '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "'  ")
            StrSQL.Append(" AND      Cul_Cod_Agea  = '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "'  ")


            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = "Particella: " & PROV & " " & COM & " " & SEZIONE & " " & CStr(FOGLIO) & " " & CStr(NUMERO) & " " & SUBALTERNO & vbCrLf
            MessaggioErrore &= "Macrouso: " & CStr(Macrouso_Cod) & vbCrLf
            MessaggioErrore &= "Codice Specie: " & CStr(Veg_Cod_Agea) & vbCrLf
            MessaggioErrore &= "Codice Varietà: " & CStr(Cul_Cod_Agea) & vbCrLf
            MessaggioErrore &= "Errore: " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    'dati particella, fascicolo, macrouso e Veg_Cod_Agea modifica Cul_Cod_Agea, superficie e validita
    Public Function Modifica_con_Fascicolo(
                        ByVal PIVA As String,
                        ByVal PROV As String,
                        ByVal COM As String,
                        ByVal SEZIONE As String,
                        ByVal FOGLIO As Int32,
                        ByVal NUMERO As Int32,
                        ByVal SUBALTERNO As String,
                        ByVal Numero_Fascicolo As String,
                        ByVal Macrouso_Cod As String,
                        ByVal Veg_Cod_Agea As String,
                                ByVal Cul_Cod_Agea As String,
                                ByVal Superficie As Decimal,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_W.Modifica_con_Fascicolo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Macrouso_Cod = "" Then
                Throw New Exception("Parametro non corretto nella query (Macrouso_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE ParticelleCatastalixMacrousixUtilizzo SET ")
            StrSQL.Append("    Cul_Cod_Agea        =  '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "'")
            StrSQL.Append("    ,Superficie        =  " & Agro_SQL_SaveNum(Superficie))
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            StrSQL.Append(" AND      PROV         = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.Append(" AND      Sezione     = '" & If(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append(" AND      FOGLIO      =  " & If(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append(" AND      Numero      =  " & If(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append(" AND      SUBALTERNO  = '" & If(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

            StrSQL.Append(" AND      Macrouso_Cod = '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
            StrSQL.Append(" AND      Veg_Cod_Agea  = '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "'  ")

            If Numero_Fascicolo <> "" Then
                StrSQL.Append(" AND      Numero_Fascicolo  = '" & Agro_SQL_SaveText(Numero_Fascicolo) & "'  ")
            End If


            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = "Particella: " & PROV & " " & COM & " " & SEZIONE & " " & CStr(FOGLIO) & " " & CStr(NUMERO) & " " & SUBALTERNO & vbCrLf
            MessaggioErrore &= "Macrouso: " & CStr(Macrouso_Cod) & vbCrLf
            MessaggioErrore &= "Codice Specie: " & CStr(Veg_Cod_Agea) & vbCrLf
            MessaggioErrore &= "Codice Varietà: " & CStr(Cul_Cod_Agea) & vbCrLf
            MessaggioErrore &= "Errore: " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Cancella(ByVal PIVA As String,
                             ByVal PROV As String,
                             ByVal COM As String,
                             ByVal SEZIONE As String,
                             ByVal FOGLIO As Int32,
                             ByVal NUMERO As Int32,
                             ByVal SUBALTERNO As String,
                             ByVal Macrouso_Cod As String,
                             ByVal Veg_Cod_Agea As String,
                             ByVal Cul_Cod_Agea As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.ParticelleCatastalixMacrousixUtilizzo_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE ParticelleCatastalixMacrousixUtilizzo ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("     ,Inviato = -1 ")

                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM ParticelleCatastalixMacrousixUtilizzo  ")

                StrSQL.Append(" WHERE  1=1 ")

            End If

            ' La clausola è la stessa per entrambe le query
            StrSQL.Append(" AND      PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            StrSQL.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.Append(" AND      Sezione     = '" & If(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append(" AND      FOGLIO      =  " & If(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append(" AND      Numero      =  " & If(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append(" AND      SUBALTERNO =  '" & If(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

            If Macrouso_Cod <> "" Then
                StrSQL.Append(" AND      Macrouso_Cod = '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
            End If

            If Veg_Cod_Agea <> "" Then
                StrSQL.Append(" AND Veg_Cod_Agea =  '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
            End If
            If Cul_Cod_Agea <> "" Then
                StrSQL.Append(" AND Cul_Cod_Agea =  '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = "Particella: " & PROV & " " & COM & " " & SEZIONE & " " & CStr(FOGLIO) & " " & CStr(NUMERO) & " " & SUBALTERNO & vbCrLf
            MessaggioErrore &= "Macrouso: " & CStr(Macrouso_Cod) & vbCrLf
            MessaggioErrore &= "Codice Specie: " & CStr(Veg_Cod_Agea) & vbCrLf
            MessaggioErrore &= "Codice Varietà: " & CStr(Cul_Cod_Agea) & vbCrLf
            MessaggioErrore &= "Errore: " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Aggiorna_Validita_Fine_Fascicoli_Precedenti(ByVal Piva As String, ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_W.Aggiorna_Validita_Fine_Fascicoli_Precedenti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE ParticelleCatastalixMacrousixUtilizzo       ")
            StrSQL.Append("        SET ParticelleCatastalixMacrousixUtilizzo.Validita_Fine = (  ")
            StrSQL.Append("             SELECT DATEADD(dd, -1,MIN(M2.Validita_Inizio))  ")
            StrSQL.Append("             FROM ParticelleCatastalixMacrousixUtilizzo M2  ")
            StrSQL.Append("             WHERE ParticelleCatastalixMacrousixUtilizzo.Data_Validazione_Fascicolo < M2.Data_Validazione_Fascicolo ")
            StrSQL.Append("             AND ParticelleCatastalixMacrousixUtilizzo.Piva = M2.Piva)   ")

            StrSQL.Append(" FROM ParticelleCatastalixMacrousixUtilizzo ")

            StrSQL.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")

            StrSQL.Append(" AND exists (SELECT * from ParticelleCatastalixMacrousixUtilizzo M2 ")
            StrSQL.Append("            WHERE ParticelleCatastalixMacrousixUtilizzo.Data_Validazione_Fascicolo < M2.Data_Validazione_Fascicolo  ")
            StrSQL.Append("            AND ParticelleCatastalixMacrousixUtilizzo.Piva = M2.Piva)   ")
            StrSQL.Append(" AND Numero_Fascicolo IS NOT NULL  ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore &= "Errore: " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
