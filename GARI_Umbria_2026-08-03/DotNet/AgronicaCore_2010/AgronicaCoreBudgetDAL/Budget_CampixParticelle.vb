Imports System.Data.Common
Imports System.Text
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Public Class Budget_CampixParticelle_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Id_Budget As Integer,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Campo_Cod As Int32,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Long,
                            ByVal NUMERO As Long,
                            ByVal SUBALTERNO As String,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.CampixParticelle_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT ParticelleCatastali.PART_COD, 
                                    ParticelleCatastali.PROV, ParticelleCatastali.COM, 
                                    ParticelleCatastali.SEZIONE, 
                                    ParticelleCatastali.FOGLIO, 
                                    ParticelleCatastali.NUMERO, 
                                    ParticelleCatastali.SUBALTERNO, 
                                    ParticelleCatastali.PARTITA_CATASTALE, 
                                    ParticelleCatastali.ETTARI, 
                                    ParticelleCatastali.ARE, 
                                    ParticelleCatastali.CENTIARE, 
                                    Budget_CampixParticelle.AREA")
                    StrSQL.Append(" FROM  Budget_CampixParticelle , ParticelleCatastali ")
                    StrSQL.Append(" WHERE Budget_CampixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Budget_CampixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Budget_CampixParticelle.PROV = ParticelleCatastali.PROV ")
                    StrSQL.Append(" AND   Budget_CampixParticelle.COM = ParticelleCatastali.COM ")
                    StrSQL.Append(" AND   Budget_CampixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    StrSQL.Append(" AND   Budget_CampixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    StrSQL.Append(" AND   Budget_CampixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    StrSQL.Append(" AND   Budget_CampixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append(" AND Budget_CampixParticelle.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "  ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND Budget_CampixParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_CampixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_CampixParticelle.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        StrSQL.Append(" AND Budget_CampixParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND Budget_CampixParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND Budget_CampixParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND Budget_CampixParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND Budget_CampixParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND Budget_CampixParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Budget_CampixParticelle.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Budget_CampixParticelle.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

                    End If
                    '---------------------------------------------

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * , Budget_CampixParticelle.Validita_Inizio as xValidita_Inizio, Budget_CampixParticelle.Validita_Fine as xValidita_Fine ")
                    StrSQL.Append(" FROM  Budget_CampixParticelle , ParticelleCatastali ")
                    StrSQL.Append(" WHERE Budget_CampixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Budget_CampixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Budget_CampixParticelle.PROV = ParticelleCatastali.PROV ")
                    StrSQL.Append(" AND   Budget_CampixParticelle.COM = ParticelleCatastali.COM ")
                    StrSQL.Append(" AND   Budget_CampixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    StrSQL.Append(" AND   Budget_CampixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    StrSQL.Append(" AND   Budget_CampixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    StrSQL.Append(" AND   Budget_CampixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append(" AND Budget_CampixParticelle.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "  ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND Budget_CampixParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_CampixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_CampixParticelle.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        StrSQL.Append(" AND Budget_CampixParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND Budget_CampixParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND Budget_CampixParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND Budget_CampixParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND Budget_CampixParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND Budget_CampixParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Budget_CampixParticelle.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Budget_CampixParticelle.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

                    End If
                    '---------------------------------------------


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Budget_CampixParticelle.*, ")
                    StrSQL.Append(" Budget_CampixParticelle.Validita_Inizio as xValidita_Inizio, ")
                    StrSQL.Append(" Budget_CampixParticelle.Validita_Fine as xValidita_Fine, ")
                    StrSQL.Append(" Budget_Campi.Campo_Des, Budget_Campi.Validita_Inizio as Validita_Inizio_Campo, Budget_Campi.Validita_Fine as Validita_Fine_Campo,")
                    StrSQL.Append(" ParticelleCatastali.ETTARI, ParticelleCatastali.[ARE], ParticelleCatastali.CENTIARE, ")
                    StrSQL.Append(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Lista_Province.PROVINCIA, ")
                    StrSQL.Append(" Budget_CampixParticelle.Piva, Imprese.rag_soc, Budget_CampixParticelle.Sa_Cod, Centri_Aziendali.sa_nome ")

                    StrSQL.Append(" FROM  Budget_CampixParticelle INNER JOIN  ")
                    StrSQL.Append(" Budget_Campi ON Budget_CampixParticelle.Id_Budget = Budget_Campi.Id_Budget AND Budget_CampixParticelle.PIVA = Budget_Campi.PIVA AND Budget_CampixParticelle.SA_COD = Budget_Campi.SA_COD AND  ")
                    StrSQL.Append(" Budget_CampixParticelle.Campo_Cod = Budget_Campi.Campo_Cod INNER JOIN ")
                    StrSQL.Append(" ParticelleCatastali ON Budget_CampixParticelle.PROV = ParticelleCatastali.PROV AND Budget_CampixParticelle.COM = ParticelleCatastali.COM AND  ")
                    StrSQL.Append(" Budget_CampixParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND Budget_CampixParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
                    StrSQL.Append(" Budget_CampixParticelle.NUMERO = ParticelleCatastali.NUMERO AND ")
                    StrSQL.Append(" Budget_CampixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO INNER JOIN ")
                    StrSQL.Append(" ISTAT ON Budget_CampixParticelle.PROV = ISTAT.PROV AND Budget_CampixParticelle.COM = ISTAT.COM INNER JOIN ")
                    StrSQL.Append(" Lista_Province ON ISTAT.PROV = Lista_Province.PROV INNER JOIN ")
                    StrSQL.Append(" Centri_Aziendali ON Budget_CampixParticelle.PIVA = Centri_Aziendali.PIVA AND Budget_CampixParticelle.sa_cod = Centri_Aziendali.sa_cod INNER JOIN ")
                    StrSQL.Append(" Imprese ON Centri_Aziendali.PIVA = Imprese.PIVA ")

                    StrSQL.Append(" WHERE   (ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    StrSQL.Append(" AND     (ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
                    StrSQL.Append(" AND     (Budget_CampixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    StrSQL.Append(" AND     (Budget_CampixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")


                    '----- Condizioni

                    If Id_Budget <> 0 Then
                        StrSQL.Append(" AND Budget_CampixParticelle.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "  ")
                    End If

                    If (Piva <> "") Then
                        StrSQL.Append(" AND Budget_CampixParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_CampixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_CampixParticelle.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        StrSQL.Append(" AND Budget_CampixParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND Budget_CampixParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND Budget_CampixParticelle.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND Budget_CampixParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND Budget_CampixParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND Budget_CampixParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Budget_CampixParticelle.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Budget_CampixParticelle.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

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

    '################################################################################

End Class

Public Class Budget_CampixParticelle_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Cancella(ByVal Id_Budget As Int32,
                            ByVal PIVA As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Campo_Cod As Int32,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Int32,
                            ByVal NUMERO As Int32,
                            ByVal SUBALTERNO As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_CampixParticelle_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Budget_CampixParticelle ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  PIVA <> '0' ")
                StrSQL.Append(" AND Inviato >= 0")

                If Id_Budget <> 0 Then
                    StrSQL.Append(" AND Id_Budget = " & Agro_SQL_SaveNum(Id_Budget))
                End If

                If PIVA <> "" Then
                    StrSQL.Append(" AND  Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                End If

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
                End If

                If Campo_Cod <> 0 Then
                    StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod))
                End If

                If PROV <> "" Then
                    StrSQL.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
                End If
                If COM <> "" Then
                    StrSQL.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
                End If
                If SEZIONE <> "" Then
                    StrSQL.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                End If
                If FOGLIO <> 0 Then
                    StrSQL.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
                End If
                If NUMERO <> 0 Then
                    StrSQL.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
                End If
                If SUBALTERNO <> "" Then
                    StrSQL.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Budget_CampixParticelle ")
                StrSQL.Append(" WHERE  PIVA <> '0' ")

                If Id_Budget <> 0 Then
                    StrSQL.Append(" AND Id_Budget = " & Agro_SQL_SaveNum(Id_Budget))
                End If

                If PIVA <> "" Then
                    StrSQL.Append(" AND  Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                End If

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
                End If

                If Campo_Cod <> 0 Then
                    StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod))
                End If

                If PROV <> "" Then
                    StrSQL.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
                End If
                If COM <> "" Then
                    StrSQL.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
                End If
                If SEZIONE <> "" Then
                    StrSQL.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                End If
                If FOGLIO <> 0 Then
                    StrSQL.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
                End If
                If NUMERO <> 0 Then
                    StrSQL.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
                End If
                If SUBALTERNO <> "" Then
                    StrSQL.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
                End If


            End If


            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function Scrivi(ByVal Id_Budget As Int32,
                            ByVal PIVA As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Campo_Cod As Int32,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Int32,
                            ByVal NUMERO As Int32,
                            ByVal SUBALTERNO As String,
                            ByVal SAU_Convenz_Ettari As Decimal,
                            ByVal SAU_Convenz_Are As Int32,
                            ByVal SAU_Convenz_Centiare As Int32,
                            ByVal SAU_Convers_Ettari As Decimal,
                            ByVal SAU_Convers_Are As Int32,
                            ByVal SAU_Convers_Centiare As Int32,
                            ByVal SAU_Bio_Ettari As Decimal,
                            ByVal SAU_Bio_Are As Int32,
                            ByVal SAU_Bio_Centiare As Int32,
                            ByVal Area As Decimal,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_CampixParticelle_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Budget_CampixParticelle(       ")
            StrSQL.Append("                    Id_Budget, PIVA, Sa_Cod, Campo_Cod, PROV, COM, SEZIONE, ")
            StrSQL.Append("                    FOGLIO, NUMERO, SUBALTERNO, Area, ")
            StrSQL.Append("                    SAU_Convenz_Ettari, SAU_Convenz_Are, SAU_Convenz_Centiare, ")
            StrSQL.Append("                    SAU_Convers_Ettari, SAU_Convers_Are, SAU_Convers_Centiare, ")
            StrSQL.Append("                    SAU_Bio_Ettari,     SAU_Bio_Are,     SAU_Bio_Centiare, ")
            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("           " & Agro_SQL_SaveNum(Id_Budget) & "  ") 'Aggiunta Id_Budget
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Trim(PIVA)) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(PROV)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  ")
            StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Area) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convenz_Ettari) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convenz_Are) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convenz_Centiare) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convers_Ettari) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convers_Are) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convers_Centiare) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Bio_Ettari) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Bio_Are) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Bio_Centiare) & "   ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function AggiornaValidita(ByVal Id_Budget As Integer,
                                ByVal PIVA As String,
                                ByVal Sa_Cod As Long,
                                ByVal Campo_Cod As Long,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreBudgetDAL.Budget_CampixParticelle_W.AggiornaValiditaFine()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Budget_CampixParticelle SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE    PIVA        <> '0'")

            If Id_Budget <> 0 Then
                StrSQL.Append(" AND Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If PIVA <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod))
            End If


            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function


End Class