Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreModelsSTD.Zoo


Public Class Ricette_Operazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Ricetta_Cod As Int32,
                          ByVal Ricetta_Operazione_Cod As Int32,
                          ByVal Lav_Cod As Int32,
                          ByVal Gru_Op As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal joinRicette As Boolean = False
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Operazioni_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_SuperUser = ""
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '   Lav_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

                    StrSQL.AppendLine(" SELECT Ricette_Operazioni.Ricetta_Cod, Ricette_Operazioni.Ricetta_Operazione_Cod, Ricette_Operazioni.Ricetta_Operazione_Des, Ricette_Operazioni.Lav_Cod, ")
                    StrSQL.AppendLine(" Ricette_Operazioni.Note, Ricette_Operazioni.Num_Protocollo, Ricette_Operazioni.Id_Rcdpi, Ricette_Operazioni.Extra_Int, Ricette_Operazioni.Mezzo, ")
                    StrSQL.AppendLine(" Ricette_Operazioni.Gru_Op, Ricette_Operazioni.Costo, Ricette_Operazioni.Noleggio_Passivo, Ricette_Operazioni.Validita_Inizio, ")
                    StrSQL.AppendLine(" Ricette_Operazioni.Validita_Fine, Ricette_Operazioni.Disciplinare_PubblicoPrivato, Ricette_Operazioni.Raccoglitore_Cod ")

                    'TODO: fare meglio questa verifica .. ?
                    If joinRicette Then
                        StrSQL.AppendLine(", Ricette_Operazioni.W_Anagrafica_Stati_Cod")
                    End If

                    StrSQL.AppendLine(" FROM  Ricette_Operazioni ")
                    If joinRicette Then
                        StrSQL.AppendLine(" INNER JOIN  Ricette on Ricette.Ricetta_Cod = Ricette_Operazioni.Ricetta_COD ")
                    End If

                    StrSQL.AppendLine(" WHERE Ricette_Operazioni.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.AppendLine(" AND   Ricette_Operazioni.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.AppendLine(" AND Ricette_Operazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Operazioni.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Ricetta_Operazione_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Operazioni.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
                    End If

                    If Lav_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Operazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                    End If

                    If Gru_Op <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Operazioni.Gru_Op = " & Agro_SQL_SaveNum(Gru_Op) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Ricette_Operazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Ricette_Operazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else

                        If joinRicette Then
                            StrSQL.AppendLine(" ORDER BY Ricette_Operazioni.Ricetta_SuperUser, Ricette_Operazioni.Ricetta_Cod, Ricette_Operazioni.Ricetta_Operazione_Cod Asc ")
                        Else
                            StrSQL.AppendLine(" ORDER BY Ricetta_SuperUser, Ricetta_Cod, Ricetta_Operazione_Cod Asc ")
                        End If

                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

                    StrSQL.AppendLine(" SELECT Ricette_Operazioni.* ")
                    If joinRicette Then
                        StrSQL.AppendLine(", CASE WHEN Ricette_Operazioni.W_Anagrafica_Stati_Cod = 300 THEN 'Ricetta' WHEN Ricette_Operazioni.W_Anagrafica_Stati_Cod = 301 THEN 'Brogliaccio' END AS RicettaBrogliaccio")
                        StrSQL.AppendLine(", Ricette.Ricetta_Numero AS Codice_Ricetta")
                    End If

                    StrSQL.AppendLine(" FROM  Ricette_Operazioni ")
                    If joinRicette Then
                        StrSQL.AppendLine(" INNER JOIN  Ricette on Ricette.Ricetta_Cod = Ricette_Operazioni.Ricetta_COD ")
                    End If

                    StrSQL.AppendLine(" WHERE Ricette_Operazioni.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.AppendLine(" AND   Ricette_Operazioni.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.AppendLine(" AND Ricette_Operazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Operazioni.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Ricetta_Operazione_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Operazioni.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
                    End If

                    If Lav_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Operazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                    End If

                    If Gru_Op <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Operazioni.Gru_Op = " & Agro_SQL_SaveNum(Gru_Op) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Ricette_Operazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Ricette_Operazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        If joinRicette Then
                            StrSQL.AppendLine(" ORDER BY Ricette_Operazioni.Ricetta_SuperUser, Ricette_Operazioni.Ricetta_Cod, Ricette_Operazioni.Ricetta_Operazione_Cod Asc ")
                        Else
                            StrSQL.AppendLine(" ORDER BY Ricetta_SuperUser, Ricetta_Cod, Ricetta_Operazione_Cod Asc ")
                        End If
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

                    StrSQL.AppendLine(" SELECT Ricette_Operazioni.*, Operazioni.Lav_Des ")
                    If joinRicette Then
                        StrSQL.AppendLine(" FROM  Ricette_Operazioni INNER JOIN  Ricette on Ricette.Ricetta_Cod = Ricetta_Operazioni.Ricetta_COD LEFT OUTER JOIN ")
                    Else
                        StrSQL.AppendLine(" FROM  Ricette_Operazioni LEFT OUTER JOIN ")
                    End If
                    StrSQL.AppendLine(" Operazioni ON Ricette_Operazioni.Lav_Cod = Operazioni.LAV_COD ")

                    StrSQL.AppendLine(" WHERE Ricette_Operazioni.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.AppendLine(" AND   Ricette_Operazioni.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.AppendLine(" AND Ricette_Operazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Operazioni.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Ricetta_Operazione_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Operazioni.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
                    End If

                    If Lav_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Operazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                    End If

                    If Gru_Op <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Operazioni.Gru_Op = " & Agro_SQL_SaveNum(Gru_Op) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Ricette_Operazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Ricette_Operazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        If joinRicette Then
                            StrSQL.AppendLine(" ORDER BY Ricette_Operazioni.Ricetta_SuperUser, Ricette_Operazioni.Ricetta_Cod, Ricette_Operazioni.Ricetta_Operazione_Cod Asc ")
                        Else
                            StrSQL.AppendLine(" ORDER BY Ricetta_SuperUser, Ricetta_Cod, Ricetta_Operazione_Cod Asc ")
                        End If
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

                    StrSQL.AppendLine(" Select ")
                    StrSQL.AppendLine("   r.Ricetta_SuperUser ")
                    StrSQL.AppendLine(" , r.Ricetta_Cod ")
                    StrSQL.AppendLine(" , Ricetta_Des ")
                    StrSQL.AppendLine(" , r.Ricetta_Des_Long ")
                    StrSQL.AppendLine(" , r.Veg_Cod ")
                    StrSQL.AppendLine(" , r.Note ")
                    StrSQL.AppendLine("  ")
                    StrSQL.AppendLine(" , r.DataLock ")
                    StrSQL.AppendLine(" , r.Piva ")
                    StrSQL.AppendLine(" , r.Sa_Cod ")
                    StrSQL.AppendLine(" , r.Tipo_Ricetta ")
                    StrSQL.AppendLine(" , r.Programmazione_Cod ")
                    StrSQL.AppendLine(" , r.Ricetta_Numero ")
                    StrSQL.AppendLine(" , r.Imputazione_Cod ")
                    StrSQL.AppendLine(" , r.Imputazione_Fase_Cod ")
                    StrSQL.AppendLine(" , r.Blocco_Flag ")
                    StrSQL.AppendLine(" , r.Blocco_Data ")
                    StrSQL.AppendLine(" , r.Blocco_Username ")
                    StrSQL.AppendLine(" , r.Origine ")
                    StrSQL.AppendLine(" , o.Ricetta_Operazione_Cod ")
                    StrSQL.AppendLine(" , o.Ricetta_Operazione_Des ")
                    StrSQL.AppendLine(" , o.Lav_Cod ")
                    StrSQL.AppendLine(" , o.Note as Note_Operazione ")
                    StrSQL.AppendLine(" , o.Num_Protocollo ")
                    StrSQL.AppendLine(" , o.Id_Rcdpi ")
                    StrSQL.AppendLine(" , o.Extra_Int ")
                    StrSQL.AppendLine(" , o.Mezzo ")
                    StrSQL.AppendLine("  ")
                    StrSQL.AppendLine(" , o.Gru_Op ")
                    StrSQL.AppendLine(" , o.Costo ")
                    StrSQL.AppendLine(" , o.Noleggio_Passivo ")
                    StrSQL.AppendLine(" , o.Id_Tp_Fer ")
                    StrSQL.AppendLine(" , o.EM_Cod ")
                    StrSQL.AppendLine(" , o.Eff_Perc ")
                    StrSQL.AppendLine(" , o.Disciplinare_PubblicoPrivato ")
                    StrSQL.AppendLine(" , o.W_Anagrafica_Stati_Cod ")
                    StrSQL.AppendLine(" , o.Ricetta_Operazione_Cod_RIF ")
                    StrSQL.AppendLine(" , o.APP_Ricetta_Operazione_ID ")
                    StrSQL.AppendLine(" , o.Raccoglitore_Cod ")
                    StrSQL.AppendLine("  ")
                    StrSQL.AppendLine("  ")
                    StrSQL.AppendLine(" From ricette_operazioni o ")
                    StrSQL.AppendLine("  inner Join Ricette r ")
                    StrSQL.AppendLine("         On r.Ricetta_Cod = o.Ricetta_Cod")

                    StrSQL.AppendLine(" WHERE o.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.AppendLine(" AND   o.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.AppendLine(" AND o.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.AppendLine(" AND o.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Ricetta_Operazione_Cod <> 0 Then
                        StrSQL.AppendLine(" AND o.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
                    End If

                    If Lav_Cod <> 0 Then
                        StrSQL.AppendLine(" AND o.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                    End If

                    If Gru_Op <> 0 Then
                        StrSQL.AppendLine(" AND o.Gru_Op = " & Agro_SQL_SaveNum(Gru_Op) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   o.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   o.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY r.Ricetta_SuperUser, o.Ricetta_Cod, o.Ricetta_Operazione_Cod Asc ")
                    End If
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)
        End Try

        Return dt

    End Function

    Public Function Leggi1SeRicettaConGuidEsiste(ByVal guid As String,
                                  ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Operazioni_R.Leggi1SeRicettaConGuidEsiste()"
        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim dt As DataTable

        Try

            sb.AppendLine(" SELECT ")
            sb.AppendLine("     1 ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     Ricette_Operazioni ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     APP_Ricetta_Operazione_ID = '" & Agro_SQL_SaveText(guid) & "' ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Public Function LeggiRicettaOperazioneCodConGuid(ByVal guid As String, ByRef objParametriServer As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Operazioni_R.LeggiRicettaOperazioneCodPerOperazioneConGuid()"
        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim dt As DataTable

        Try

            sb.AppendLine(" SELECT ")
            sb.AppendLine("     Ricetta_Operazione_Cod ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     Ricette_Operazioni ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     APP_Ricetta_Operazione_ID LIKE '%" & Agro_SQL_SaveText(guid) & "%' ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Public Function LeggiRicettaOperazioneConRaccoglitoreCod(ByVal raccoglitore_cod As Integer, ByRef objParametriServer As AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Operazioni_R.LeggiRicettaOperazioneConRaccoglitoreCod()"
        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim dt As DataTable

        Try

            sb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            sb.AppendLine(" SELECT ")
            sb.AppendLine("     Ricetta_SuperUser, Lav_Cod, Ricetta_Cod, Ricetta_Operazione_Cod ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     Ricette_Operazioni ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     Raccoglitore_Cod = " & Agro_SQL_SaveNum(raccoglitore_cod) & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Public Function LeggiBrogliacciEAttivitaPianificatePerApp(ByVal piva As String, ByVal dataRif As Date, ByVal dataUltimaSincro As Date, ByVal soloOperazioniConTuttiGliImpiantiAttivi As Boolean, ByRef objParametriServer As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Operazioni_R.LeggiBrogliaccixApp()"

        Dim messaggioErrore As String = ""
        Dim sb = New Text.StringBuilder()
        Dim dt As DataTable

        Try

            Dim listOfStati As New List(Of Integer) From {
                CInt(AgronicaCoreModelsSTD.attivita.Attivita.Stati.Eseguita),
                CInt(AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire)
            }

            sb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            sb.AppendLine("WITH #cteBrogliacciFiltrati AS ( ")
            sb.AppendLine(" SELECT ")
            sb.AppendLine("	  r.Ricetta_SuperUser, r.Piva, r.Ricetta_Cod,")
            sb.AppendLine("	  ro.Ricetta_Operazione_Cod, ro.Raccoglitore_Cod, ro.Lav_Cod,")
            sb.AppendLine("     ro.W_Anagrafica_Stati_Cod as StatoRicetta,")
            sb.AppendLine(" 	  CASE")
            sb.AppendLine(" 	  	  WHEN CHARINDEX('|', ro.APP_Ricetta_Operazione_ID) > 0")
            sb.AppendLine(" 	  	  THEN LEFT(ro.APP_Ricetta_Operazione_ID, CHARINDEX('|', ro.APP_Ricetta_Operazione_ID) - 1)")
            sb.AppendLine(" 	  	  ELSE ro.APP_Ricetta_Operazione_ID")
            sb.AppendLine(" 	  END as GuidRicetta, ")
            sb.AppendLine("     ISNULL(ad.ID, '') as CodiceGiasPianificata ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("	  Ricette_Operazioni ro")
            sb.AppendLine(" JOIN ")
            sb.AppendLine("	  Ricette r")
            sb.AppendLine("	  on r.Ricetta_SuperUser = ro.Ricetta_SuperUser")
            sb.AppendLine("	  and r.Ricetta_Cod = ro.Ricetta_Cod")
            sb.AppendLine(" LEFT JOIN ")
            sb.AppendLine("     Agronica_Log_Ricette_UltimaOperazione ru ")
            sb.AppendLine("     ON ru.SuperUser = ro.Ricetta_SuperUser ")
            sb.AppendLine("     AND ru.Chiave = ro.Ricetta_Operazione_Cod ")
            sb.AppendLine("     AND ru.Tipo = '" & Agro_SQL_SaveText("Ricette_Operazioni") & "'")
            sb.AppendLine(" LEFT JOIN ")
            sb.AppendLine("     Ricette_Operazioni roPianificata ")
            sb.AppendLine("     ON roPianificata.Ricetta_Operazione_Cod = ro.Ricetta_Operazione_Cod_RIF ")
            sb.AppendLine(" LEFT JOIN ")
            sb.AppendLine("     APP_Dati ad ")
            sb.AppendLine("     ON ad.ID = LEFT(roPianificata.APP_Ricetta_Operazione_ID, CHARINDEX('|', roPianificata.APP_Ricetta_Operazione_ID + '|') - 1) ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     r.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            sb.AppendLine("     AND ro.W_Anagrafica_Stati_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", listOfStati), False) & ") ")
            sb.AppendLine("     AND ro.Lav_Cod IN (" & OPERAZIONI_GESTITE_APP_DEMETRA & ")")
            If Not soloOperazioniConTuttiGliImpiantiAttivi Then
                sb.AppendLine("     AND ro.Validita_Inizio >= " & Agro_SQL_SaveDate(dataRif) & " ")
                sb.AppendLine("     AND ru.Data_Ora_RegistrazioneLog >= " & Agro_SQL_SaveDateTime(dataUltimaSincro) & " ")
            End If
            sb.AppendLine("     AND NOT EXISTS (SELECT 1  ") 'solo brogliacci non ribaltati in agenda
            sb.AppendLine("                     FROM RicettexAgenda ")
            sb.AppendLine("                     WHERE Ricetta_Operazione_Cod = ro.Ricetta_Operazione_Cod)")
            sb.AppendLine("     AND ro.APP_Ricetta_Operazione_ID IS NOT NULL ")
            sb.AppendLine("     AND ro.APP_Ricetta_Operazione_ID <> '' ") 'solo le operazioni nate su app/demetra avranno id diverso da stringa vuota
            sb.AppendLine(")")

            If soloOperazioniConTuttiGliImpiantiAttivi Then
                sb.AppendLine(", ")
                sb.AppendLine(" ")
                sb.AppendLine("#cteImpiantiAttivi as ( ")
                sb.AppendLine(" SELECT  ")
                sb.AppendLine("	bf.Ricetta_Operazione_Cod, ")
                sb.AppendLine("	CASE  ")
                sb.AppendLine("           WHEN EXISTS ( ")
                sb.AppendLine("               SELECT  ")
                sb.AppendLine("					1 ")
                sb.AppendLine("               FROM  ")
                sb.AppendLine("					Imprese_Progetti es ")
                sb.AppendLine("               JOIN ")
                sb.AppendLine("					Reg_Impianti imp ")
                sb.AppendLine("					ON imp.piva = es.piva ")
                sb.AppendLine("					and imp.SA_COD = es.Sa_Cod ")
                sb.AppendLine("					and imp.APPEZZA = es.Appezza ")
                sb.AppendLine("					and imp.ID_REG = es.Id_Reg ")
                sb.AppendLine("			   WHERE  ")
                sb.AppendLine("					es.piva = rdest.piva ")
                sb.AppendLine("                   AND es.sa_cod = rdest.sa_cod ")
                sb.AppendLine("                   AND es.appezza = rdest.appezza ")
                sb.AppendLine("                   AND es.id_reg = rdest.Id_Reg ")
                sb.AppendLine("				    AND imp.Validita_Inizio <= " & Agro_SQL_SaveDate(dataRif) & " ")
                sb.AppendLine("					AND imp.Validita_Fine >= " & Agro_SQL_SaveDate(dataRif) & " ")
                sb.AppendLine("				    AND es.Validita_Inizio <= " & Agro_SQL_SaveDate(dataRif) & " ")
                sb.AppendLine("					AND es.Validita_Fine >= " & Agro_SQL_SaveDate(dataRif) & " ")
                sb.AppendLine("		   ) ")
                sb.AppendLine("           THEN 1 ")
                sb.AppendLine("           ELSE 0 ")
                sb.AppendLine("       END AS EsercizioValido ")
                sb.AppendLine(" FROM  ")
                sb.AppendLine("	#cteBrogliacciFiltrati bf ")
                sb.AppendLine(" JOIN ")
                sb.AppendLine("	Ricette_Destinazioni rdest ")
                sb.AppendLine("	on rdest.Ricetta_Operazione_Cod = bf.Ricetta_Operazione_Cod ")
                sb.AppendLine("	and rdest.Piva = bf.PIVA ")
                sb.AppendLine(" WHERE  ")
                sb.AppendLine("	rdest.Tipo_Destinazione = 0 ")
                sb.AppendLine(") ")

                sb.AppendLine(" ")

                sb.AppendLine(" SELECT  ")
                sb.AppendLine(" 	bf.*, ad.Versione, ad.Tipo ")
                sb.AppendLine(" FROM  ")
                sb.AppendLine(" 	#cteBrogliacciFiltrati bf")
                sb.AppendLine(" JOIN")
                sb.AppendLine("	APP_Dati ad")
                sb.AppendLine("	ON ad.ID = bf.GuidRicetta")
                sb.AppendLine(" WHERE ")
                sb.AppendLine(" 	NOT EXISTS(SELECT ")
                sb.AppendLine(" 					1 ")
                sb.AppendLine(" 			   FROM ")
                sb.AppendLine(" 					#cteImpiantiAttivi ")
                sb.AppendLine(" 			   WHERE ")
                sb.AppendLine(" 					#cteImpiantiAttivi.Ricetta_Operazione_Cod = bf.Ricetta_Operazione_Cod ")
                sb.AppendLine(" 					AND #cteImpiantiAttivi.EsercizioValido = 0)")
                sb.AppendLine("   AND CHARINDEX('|', ad.riferimento) > 0 ")
                sb.AppendLine("	AND SUBSTRING(ad.riferimento,0,CHARINDEX('|',riferimento,0)) = 0") 'solo brogliacci non ribaltati in agenda (ulteriore controllo)
                sb.AppendLine("	AND ad.cancellato = 0") 'solo brogliacci non cancellati 
            Else
                sb.AppendLine("")
                sb.AppendLine(" SELECT  ")
                sb.AppendLine(" 	bf.*, ad.Versione, ad.Tipo ")
                sb.AppendLine(" FROM  ")
                sb.AppendLine(" 	#cteBrogliacciFiltrati bf")
                sb.AppendLine(" JOIN")
                sb.AppendLine("	APP_Dati ad")
                sb.AppendLine("	ON ad.ID = bf.GuidRicetta")
                sb.AppendLine(" WHERE")
                sb.AppendLine("   CHARINDEX('|', ad.riferimento) > " & Agro_SQL_SaveNum(0))
                sb.AppendLine("	AND SUBSTRING(ad.riferimento,0,CHARINDEX('|',riferimento,0)) = " & Agro_SQL_SaveNum(0)) 'solo brogliacci non ribaltati in agenda (ulteriore controllo)
                sb.AppendLine("	AND ad.cancellato = " & Agro_SQL_SaveNum(0)) 'solo brogliacci non cancellati 
            End If

            sb.AppendLine("OPTION (RECOMPILE); ")
            sb.AppendLine("")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function LeggiGuidRicetta(ByVal Ricetta_Cod As Integer, ByVal Ricetta_Operazione_Cod As Integer, objParametri As AgronicaCoreParametri) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Operazioni_R.LeggiGuidRicetta()"
        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim guidRicetta As String = ""

        Try

            sb.AppendLine(" SELECT ")
            sb.AppendLine("     ISNULL(APP_Ricetta_Operazione_ID, '') as GuidRicetta ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     Ricette_Operazioni ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & " ")
            sb.AppendLine("     AND Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & " ")

            Dim dt = EseguiQuery_Lettura(objParametri, sb.ToString, nomeRoutine)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                guidRicetta = CStr(dt.Rows(0)("GuidRicetta"))
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return guidRicetta

    End Function

    '##############################################################################################
    Public Function Leggi_LavCod_Cul(ByVal Lav_Cod As Int32,
                                     ByVal Veg_Cod As Int32,
                                     ByVal Validita_Inizio As Date,
                                     ByVal Validita_Fine As Date,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByVal xOrderBy As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Operazioni_R.Leggi_LavCod_Cul()"

        '====================================================================================
        'Parametri opzionali :
        '   Lav_Cod = 0
        '   Veg_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.Append(" SELECT     Ricette_Operazioni.Ricetta_Operazione_Des, Ricette_Operazioni.Lav_Cod, Ricette_Operazioni.Ricetta_Cod, Ricette.Veg_Cod, Ricette.Piva,   Ricette_Operazioni.Ricetta_Operazione_Cod, Ricette.Tipo_Ricetta, Ricette_Operazioni.Validita_inizio, Ricette_Operazioni.Validita_Fine ")
            strSql.Append(" FROM         Ricette_Operazioni INNER JOIN ")
            strSql.Append("       Ricette ON Ricette_Operazioni.Ricetta_SuperUser = Ricette.Ricetta_SuperUser AND Ricette_Operazioni.Ricetta_Cod = Ricette.Ricetta_Cod ")

            strSql.Append(" WHERE Ricette_Operazioni.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.Append(" AND   Ricette_Operazioni.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                strSql.Append(" AND Ricette_Operazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Lav_Cod <> 0 Then
                strSql.Append(" AND Ricette_Operazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Veg_Cod <> 0 Then
                strSql.Append(" AND Ricette.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Ricette_Operazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Ricette_Operazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY Ricetta_Operazione_Des Asc ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_conDettagli(ByVal Lav_Cod As Int32,
                                      ByVal Veg_Cod As Int32,
                                      ByVal Id_Cod As Int32,
                                      ByVal Validita_Inizio As Date,
                                      ByVal Validita_Fine As Date,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Operazioni_R.Leggi_conDettagli()"

        '====================================================================================
        'Parametri opzionali :
        '   Lav_Cod = 0
        '   Veg_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            'linee tecniche (senza destinazioni)

            StrSQL.Append(" (")

            StrSQL.Append(" SELECT   ricette.Ricetta_Des,  Ricette_Operazioni.Ricetta_Operazione_Des, Ricette_Operazioni.Lav_Cod, Ricette_Operazioni.Ricetta_Cod, Ricette.Veg_Cod, Ricette.Piva,   Ricette_Operazioni.Ricetta_Operazione_Cod, Ricette.Tipo_Ricetta,  ")
            StrSQL.Append("            Ricette_Operazioni.Validita_inizio, Ricette_Operazioni.Validita_Fine, ")
            StrSQL.Append("            Ricette_Dettagli.Ricetta_Dettaglio_Cod, Ricette_Dettagli.Elem_Cod, Ricette_Dettagli.Pro_Cod, Ricette_Dettagli.Mat_Cod, ")
            StrSQL.Append("            ISNULL(Formulati.Fr_Des,'') AS Fr_Des, ISNULL(Fertilizzanti.Fer_Des,'') AS Fer_Des, ISNULL(Trappole.Trap_Des,'') AS Trap_Des, ISNULL(Materie_Prime.Mat_Des,'') AS Mat_Des ")
            StrSQL.Append("            ,'' as piva, 0 as sa_cod, 0 as appezza, 0 as Id_Reg ")
            StrSQL.Append("            ,'' As sa_nome, '' As app_nome ")

            StrSQL.Append(" FROM         Ricette_Operazioni INNER JOIN ")
            StrSQL.Append("       Ricette ON Ricette_Operazioni.Ricetta_SuperUser = Ricette.Ricetta_SuperUser AND Ricette_Operazioni.Ricetta_Cod = Ricette.Ricetta_Cod  ")
            StrSQL.Append("       INNER JOIN Ricette_Dettagli ON Ricette_Operazioni.Ricetta_SuperUser = Ricette_Dettagli.Ricetta_SuperUser AND Ricette_Operazioni.Ricetta_Cod = Ricette_Dettagli.Ricetta_Cod AND Ricette_Operazioni.Ricetta_Operazione_Cod = Ricette_Dettagli.Ricetta_Operazione_Cod ")

            StrSQL.Append("    LEFT JOIN Formulati  ")
            StrSQL.Append("        ON Ricette_Dettagli.Pro_Cod = Formulati.Fr_Cod  ")
            StrSQL.Append("  ")
            StrSQL.Append("    LEFT JOIN Fertilizzanti  ")
            StrSQL.Append("        ON Ricette_Dettagli.Pro_Cod = Fertilizzanti.Fer_Cod  ")
            StrSQL.Append("  ")
            StrSQL.Append("    LEFT JOIN Trappole  ")
            StrSQL.Append("        ON Ricette_Dettagli.Pro_Cod = Trappole.TRAP_COD  ")
            StrSQL.Append("    LEFT JOIN Materie_Prime  ")
            StrSQL.Append("        ON Ricette_Dettagli.Elem_Cod = Materie_Prime.Elem_Cod  ")
            StrSQL.Append("        AND  Ricette_Dettagli.Mat_Cod = Materie_Prime.Mat_Cod  ")
            StrSQL.Append("  ")

            StrSQL.Append(" WHERE Ricette_Operazioni.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            'StrSQL.Append(" AND   Ricette_Operazioni.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            StrSQL.Append(" AND   Ricette.tipo_ricetta = " & Agro_SQL_SaveNum(enum_TipoRicetta.Standard) & " ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND Ricette_Operazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Operazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Ricette.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Ricette_Operazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Ricette_Operazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            StrSQL.Append(" )")

            'If xFiltroAggiuntivo <> "" Then
            StrSQL.Append(" UNION ")
            'End If

            StrSQL.Append(" (")

            StrSQL.Append(" SELECT  ricette.Ricetta_Des,   Ricette_Operazioni.Ricetta_Operazione_Des, Ricette_Operazioni.Lav_Cod, Ricette_Operazioni.Ricetta_Cod, Ricette.Veg_Cod, Ricette.Piva,   Ricette_Operazioni.Ricetta_Operazione_Cod, Ricette.Tipo_Ricetta,  ")
            StrSQL.Append("            Ricette_Operazioni.Validita_inizio, Ricette_Operazioni.Validita_Fine, ")
            StrSQL.Append("            Ricette_Dettagli.Ricetta_Dettaglio_Cod, Ricette_Dettagli.Elem_Cod, Ricette_Dettagli.Pro_Cod, Ricette_Dettagli.Mat_Cod, ")
            StrSQL.Append("            ISNULL(Formulati.Fr_Des,'') AS Fr_Des, ISNULL(Fertilizzanti.Fer_Des,'') AS Fer_Des, ISNULL(Trappole.Trap_Des,'') AS Trap_Des, ISNULL(Materie_Prime.Mat_Des,'') AS Mat_Des ")
            StrSQL.Append("            ,Ricette_Destinazioni.piva,Ricette_Destinazioni.sa_cod,Ricette_Destinazioni.appezza,Ricette_Destinazioni.Id_Reg ")
            StrSQL.Append("            , ISNULL(Centri_Aziendali.sa_nome,'') As sa_nome, ISNULL(appezzamento.app_nome,'') As app_nome ")

            StrSQL.Append(" FROM         Ricette_Operazioni INNER JOIN ")
            StrSQL.Append("       Ricette ON Ricette_Operazioni.Ricetta_SuperUser = Ricette.Ricetta_SuperUser AND Ricette_Operazioni.Ricetta_Cod = Ricette.Ricetta_Cod  ")
            StrSQL.Append("       INNER JOIN Ricette_Dettagli ON Ricette_Operazioni.Ricetta_SuperUser = Ricette_Dettagli.Ricetta_SuperUser AND Ricette_Operazioni.Ricetta_Cod = Ricette_Dettagli.Ricetta_Cod AND Ricette_Operazioni.Ricetta_Operazione_Cod = Ricette_Dettagli.Ricetta_Operazione_Cod ")
            StrSQL.Append("       INNER JOIN Ricette_Destinazioni ON Ricette_Destinazioni.Ricetta_SuperUser = Ricette_Dettagli.Ricetta_SuperUser AND Ricette_Destinazioni.Ricetta_Cod = Ricette_Dettagli.Ricetta_Cod AND Ricette_Destinazioni.Ricetta_Operazione_Cod = Ricette_Dettagli.Ricetta_Operazione_Cod AND Ricette_Destinazioni.Ricetta_Dettaglio_Cod = Ricette_Dettagli.Ricetta_Dettaglio_Cod ")
            StrSQL.Append("       INNER JOIN Centri_Aziendali ON Ricette_Destinazioni.piva = Centri_Aziendali.piva AND Ricette_Destinazioni.sa_cod = Centri_Aziendali.sa_cod ")
            StrSQL.Append("       INNER JOIN appezzamento ON Ricette_Destinazioni.piva = appezzamento.piva AND Ricette_Destinazioni.sa_cod = appezzamento.sa_cod AND Ricette_Destinazioni.appezza = appezzamento.appezza ")
            StrSQL.Append("       INNER JOIN reg_impianti ON Ricette_Destinazioni.piva = reg_impianti.piva AND Ricette_Destinazioni.sa_cod = reg_impianti.sa_cod AND Ricette_Destinazioni.appezza = reg_impianti.appezza AND Ricette_Destinazioni.id_reg = reg_impianti.id_reg ")
            StrSQL.Append("       INNER JOIN imprese_progetti ON Ricette_Destinazioni.piva = imprese_progetti.piva AND Ricette_Destinazioni.sa_cod = imprese_progetti.sa_cod AND Ricette_Destinazioni.appezza = imprese_progetti.appezza AND Ricette_Destinazioni.id_reg = imprese_progetti.id_reg ")

            StrSQL.Append("       LEFT JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

            If Id_Cod <> 0 Then
                StrSQL.Append(" LEFT JOIN Reg_Impianti_Codici ON Reg_Impianti.piva = Reg_Impianti_Codici.piva and Reg_Impianti.sa_cod = Reg_Impianti_Codici.sa_cod and Reg_Impianti.appezza = Reg_Impianti_Codici.appezza and Reg_Impianti.id_reg = Reg_Impianti_Codici.id_reg ")
            End If

            StrSQL.Append("    LEFT JOIN Formulati  ")
            StrSQL.Append("        ON Ricette_Dettagli.Pro_Cod = Formulati.Fr_Cod  ")
            StrSQL.Append("  ")
            StrSQL.Append("    LEFT JOIN Fertilizzanti  ")
            StrSQL.Append("        ON Ricette_Dettagli.Pro_Cod = Fertilizzanti.Fer_Cod  ")
            StrSQL.Append("  ")
            StrSQL.Append("    LEFT JOIN Trappole  ")
            StrSQL.Append("        ON Ricette_Dettagli.Pro_Cod = Trappole.TRAP_COD  ")
            StrSQL.Append("    LEFT JOIN Materie_Prime  ")
            StrSQL.Append("        ON Ricette_Dettagli.Elem_Cod = Materie_Prime.Elem_Cod  ")
            StrSQL.Append("        AND  Ricette_Dettagli.Mat_Cod = Materie_Prime.Mat_Cod  ")
            StrSQL.Append("  ")

            StrSQL.Append(" WHERE Ricette_Operazioni.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            'StrSQL.Append(" AND   Ricette_Operazioni.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            StrSQL.Append(" AND imprese_progetti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND   imprese_progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            StrSQL.Append(" AND   Ricette.tipo_ricetta IN (" & enum_TipoRicetta.Standard_Destinazioni & "," & enum_TipoRicetta.PianoDistribuzioneConcimi & ") ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND Ricette_Operazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Operazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            Else
                StrSQL.Append(" AND reg_impianti.Cul_Cod = 0")
                If Id_Cod <> 0 Then
                    'StrSQL.Append(" AND reg_impianti.Cul_Cod = 0")
                    StrSQL.Append(" AND Reg_Impianti_Codici.id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & "   ")
                End If
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Ricette_Operazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Ricette_Operazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            StrSQL.Append(" )")

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Ricetta_Operazione_Des Asc ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '################################################################################
    Public Function CostiAccessori_from_Ricetta_Operazione_Cod(ByVal Ricetta_Cod As Int32,
                                                               ByVal Ricetta_Operazione_Cod As Int32,
                                                               ByVal xOrderBy As String,
                                                               ByRef objParametri As AgronicaCoreParametri
                                                               ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Operazioni_R.CostiAccessori_from_Ricetta_Operazione_Cod()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append("  SELECT DISTINCT Ricette_Dettagli.Ricetta_Dettaglio_Cod, Ricette_Dettagli.Elem_Cod, ")
            StrSQL.Append("  Risorse_Umane.Cod_RisUm, Contatti.Cod_Contatto, Contatti.Rag_Soc, ")
            StrSQL.Append("  Risorse_Umane.Patentino, Risorse_Umane.Data_Rilascio_Patentino, Risorse_Umane.Data_Scadenza_Patentino, ")
            StrSQL.Append("  '' AS Mac_Des, 0 AS Mac_Cod, '' AS Class_Code, '' AS Class_Desc, '' AS Modello, '' AS Ultima_Manutenzione ")
            StrSQL.Append("  FROM Ricette_Dettagli INNER JOIN ")
            StrSQL.Append("  Contatti INNER JOIN ")
            StrSQL.Append("  Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ON ")
            StrSQL.Append("  Ricette_Dettagli.Mat_Cod = Risorse_Umane.Cod_RisUm ")
            StrSQL.Append("  WHERE Ricette_Dettagli.Cau_Mov IN ('6800','6850') ")
            StrSQL.Append("  AND Ricette_Dettagli.Elem_Cod = 0 ")
            StrSQL.Append("  AND Ricette_Dettagli.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & " ")

            StrSQL.Append("  UNION ")

            StrSQL.Append("  SELECT DISTINCT Ricette_Dettagli.Ricetta_Dettaglio_Cod, Ricette_Dettagli.Elem_Cod, ")
            StrSQL.Append("  0, '', '', '', '', '', ")
            StrSQL.Append("  Parco_Macchine.Mac_Des, Parco_Macchine.Mac_Cod, Parco_Macchine.Class_Code, Macchine.Class_Desc, Parco_Macchine.Modello, Parco_Macchine.Ultima_Manutenzione ")
            StrSQL.Append("  FROM Ricette_Dettagli INNER JOIN ")
            StrSQL.Append("  Macchine INNER JOIN ")
            StrSQL.Append("  Parco_Macchine ON Macchine.CLASS_CODE = Parco_Macchine.Class_Code ON ")
            StrSQL.Append("  Ricette_Dettagli.Mat_Cod = Parco_Macchine.Mac_Cod ")
            StrSQL.Append("  WHERE Ricette_Dettagli.Cau_Mov = '8100' ")
            StrSQL.Append("  AND Ricette_Dettagli.Elem_Cod = 1 ")
            StrSQL.Append("  AND Ricette_Dettagli.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & " ")


            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Ricette_Dettagli.Elem_Cod ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function VegCod_From_RicetteOperazioni(ByVal Ricetta_Operazione_Cod As Integer,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As Integer

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Operazioni_R.VegCod_From_RicetteOperazioni()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable
        Dim res As Integer = -1

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT DISTINCT Ricette_Destinazioni.Ricetta_Operazione_Cod, SpecieVegetali.veg_cod, SpecieVegetali.veg_des  ")
            strSql.AppendLine(" FROM Reg_Impianti ")
            strSql.AppendLine(" INNER JOIN Ricette_Destinazioni ON Reg_Impianti.APPEZZA = Ricette_Destinazioni.Appezza AND Reg_Impianti.APPEZZA = Ricette_Destinazioni.Appezza AND Reg_Impianti.ID_REG = Ricette_Destinazioni.Id_Reg AND Reg_Impianti.PIVA = Ricette_Destinazioni.Piva AND Reg_Impianti.SA_COD = Ricette_Destinazioni.Sa_Cod ")
            strSql.AppendLine(" INNER JOIN Cultivar ON Reg_Impianti.cul_cod = cultivar.cul_cod ")
            strSql.AppendLine(" INNER JOIN SpecieVegetali ON cultivar.veg_cod = SpecieVegetali.veg_cod ")
            strSql.AppendLine(" WHERE Ricette_Destinazioni.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod))

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Ricette_Destinazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Ricette_Destinazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 AndAlso
               IsNumeric(dt.Rows(0).Item("veg_cod")) Then
                res = dt.Rows(0).Item("veg_cod")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return res

    End Function

    Public Function Leggi_DataUsernameCreazione(Ricetta_Cod As Integer,
                                                Ricetta_Operazione_Cod As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Operazioni_R.Leggi_DataUsernameCreazione()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT Data_Creazione, Username_Creazione")
            strSql.AppendLine(" FROM Ricette_Operazioni ")

            strSql.AppendLine(" WHERE 1=1")

            If Ricetta_Cod <> 0 Then
                strSql.AppendLine(" AND Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                strSql.AppendLine(" AND Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Ricette_Operazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Ricette_Operazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Ricetta_Operazione_Des Asc ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_NumProtocollo(ByVal Ricetta_Cod As Integer,
                                        ByVal Ricetta__Operazione_Cod As Integer,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Integer

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Operazioni_R.Leggi_NumProtocollo()"

        Dim regolamentoCod As Integer = 0

        Try

            Dim dt = Leggi(Ricetta_Cod, Ricetta__Operazione_Cod, 0, 0,
                           AGRODATAINIZIO, AGRODATAFINE,
                           enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                           "", "", objParametri)

            If dt.Rows.Count > 0 Then
                regolamentoCod = dt.Rows(0).Item("Num_Protocollo")
            End If

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return regolamentoCod

    End Function

    Public Function LeggiLavCodFromRicettaOperazioneCod(ricettaOperazioneCod As Integer,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        ) As Int32

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dettagli_R.LeggiLavCodFromRicettaOperazioneCod()"

        Dim messaggioErrore
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            strSql.AppendLine(" SELECT Lav_Cod ")
            strSql.AppendLine(" FROM Ricette_Operazioni ")
            strSql.AppendLine(" WHERE Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(ricettaOperazioneCod) & "   ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Return CInt(dt.Rows(0).Item("Lav_Cod"))
        Else
            Return 0
        End If

    End Function

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Ricette_Operazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Update_APP_Ricetta_Operazione_ID_And_Ricetta_Operazione_Cod_RIF(Ricetta_Cod As Integer,
                                                     APP_Ricetta_Operazione_ID As String,
                                                     Ricetta_Operazione_Cod_RIF As Integer,
                                                     objParametri_Server As AgronicaCoreParametri
                                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Operazioni_W.Update_APP_Ricetta_Operazione_ID()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri_Server.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            If Ricetta_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_Cod = 0)")
            End If

            '---------------------------------------------

            strSql.Length = 0

            strSql.AppendLine(" UPDATE Ricette_Operazioni SET ")
            If Ricetta_Operazione_Cod_RIF > 0 Then
                strSql.AppendLine("    Ricetta_Operazione_Cod_RIF = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod_RIF) & ",")
            End If
            strSql.AppendLine("    APP_Ricetta_Operazione_ID  =  '" & Agro_SQL_SaveText(APP_Ricetta_Operazione_ID) & "'")

            strSql.AppendLine(" WHERE  Ricetta_SuperUser  = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "'   ")
            strSql.AppendLine(" AND Ricetta_Cod  = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function ImpostaStatoEseguitoPerGias_APP(xFiltroAggiuntivo As String,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    Optional dbContext As Gias_DeveloperServer_Entities = Nothing
                                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Operazioni_W.ImpostaStatoEseguitoPerGias_APP()"

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            stb.Length = 0
            stb.AppendLine(" update opWeb ")
            stb.AppendLine(" set W_Anagrafica_Stati_Cod = " & enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita)
            stb.AppendLine(" From APP_Ricette_Operazioni opAPP ")
            stb.AppendLine(" inner Join Ricette_Operazioni opWeb ")
            stb.AppendLine("     On opAPP.Ricetta_Operazione_Cod_RIF = opWeb.Ricetta_Operazione_Cod ")
            stb.AppendLine(" where opWeb.W_Anagrafica_Stati_Cod = " & enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Da_Eseguire)

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If dbContext Is Nothing Then
                xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            Else
                dbContext.Database.ExecuteSqlCommand(stb.ToOrigin(objParametri))
            End If

        Catch ex As Exception
            messaggioErrore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)
        End Try

        Return xRisp

    End Function


    '##############################################################################################
    Public Function Scrivi(ByVal Ricetta_Cod As Int32,
                           ByVal Ricetta_Operazione_Cod As Int32,
                           ByVal Lav_Cod As Int32,
                           ByVal Ricetta_Operazione_Des As String,
                           ByVal Note As String,
                           ByVal Num_Protocollo As Decimal,
                           ByVal Id_Rcdpi As Int32,
                           ByVal Extra_Int As Int32,
                           ByVal Mezzo As Int32,
                           ByVal Gru_Op As Int32,
                           ByVal Costo As Decimal,
                           ByVal Noleggio_Passivo As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Ora As Date? = Nothing
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Operazioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0

            strSql.Append(" INSERT INTO Ricette_Operazioni ")
            strSql.Append("         ( ")
            strSql.Append("          Ricetta_SuperUser,      Ricetta_Cod,       Ricetta_Operazione_Cod, ")
            strSql.Append("          Lav_Cod,                Ricetta_Operazione_Des,       ")
            strSql.Append("          Num_Protocollo,         Id_Rcdpi,   Extra_Int,       Mezzo,         ")
            strSql.Append("          Gru_Op,                 Costo,      Noleggio_Passivo              ")
            strSql.Append("          Note,                   DataLock,  ")

            strSql.Append("          Inviato,            DataInvio, ")
            strSql.Append("          Data_Creazione,     Data_Modifica, ")
            strSql.Append("          UserName_Creazione, UserName_Modifica, ")
            strSql.Append("          Validita_Inizio,    Validita_Fine, Ora ")
            strSql.Append("         ) ")

            strSql.Append(" VALUES ( ")
            strSql.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Ricetta_Cod))
            strSql.Append("         , " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            strSql.Append("         , '" & Agro_SQL_SaveText(Ricetta_Operazione_Des) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Num_Protocollo) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Rcdpi) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Int) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Mezzo) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Gru_Op) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Costo) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Noleggio_Passivo) & " ")
            strSql.Append("         , '" & Agro_SQL_SaveText(Note) & "'  ")
            strSql.Append("         , 0 ")

            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")
            strSql.Append("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            If (Ora IsNot Nothing) Then
                strSql.Append("         , " & Agro_SQL_SaveDateTime(Ora) & "  ")
            Else
                strSql.Append("         , " & Agro_SQL_SaveDateTime(Validita_Inizio) & "  ")
            End If

            strSql.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    ' L'overload ha 3 campi in più: Id_Tp_Fer, EM_Cod, Eff_Perc 
    '##############################################################################################
    Public Function Scrivi(ByVal Ricetta_Cod As Int32,
                           ByVal Ricetta_Operazione_Cod As Int32,
                           ByVal Lav_Cod As Int32,
                           ByVal Ricetta_Operazione_Des As String,
                           ByVal Note As String,
                           ByVal Num_Protocollo As Decimal,
                           ByVal Id_Rcdpi As Int32,
                           ByVal Extra_Int As Int32,
                           ByVal Mezzo As Int32,
                           ByVal Gru_Op As Int32,
                           ByVal Costo As Decimal,
                           ByVal Noleggio_Passivo As Int32,
                           ByVal Id_Tp_Fer As Int32,
                           ByVal EM_Cod As Int32,
                           ByVal Eff_Perc As Decimal,
                           ByVal Disciplinare_PubblicoPrivato As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal W_Anagrafica_Stati_Cod As Integer? = Nothing,
                           Optional ByVal Ricetta_Operazione_Cod_RIF As Integer? = Nothing,
                           Optional ByVal APP_Ricetta_Operazione_ID As String = Nothing,
                           Optional ByVal Invia_App As Integer? = 0,
                           Optional ByVal Raccoglitore_Cod As Integer = 0,
                           Optional ByVal Invia_HubIoT As Integer = 0,
                           Optional ByVal Ora As Date? = Nothing
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Operazioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Invia_App Is Nothing Then
                Invia_App = 0
            End If

            If Data_creazione = #2/1/1900# Then
                Data_creazione = DateTime.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" INSERT INTO Ricette_Operazioni ")
            strSql.Append("         ( ")
            strSql.Append("          Ricetta_SuperUser,      Ricetta_Cod,               Ricetta_Operazione_Cod, ")
            strSql.Append("          Lav_Cod,                Ricetta_Operazione_Des, ")
            strSql.Append("          Num_Protocollo,         Id_Rcdpi,                  Extra_Int,                      Mezzo, ")
            strSql.Append("          Gru_Op,                 Costo,                     Noleggio_Passivo, ")
            strSql.Append("          Id_Tp_Fer,              EM_Cod,                    Eff_Perc,                       Disciplinare_PubblicoPrivato, ")
            strSql.Append("          Note,                   DataLock,                  W_Anagrafica_Stati_Cod,         Ricetta_Operazione_Cod_RIF, APP_Ricetta_Operazione_ID, ")
            strSql.Append("          Invia_App, Raccoglitore_Cod, Invia_HubIoT, ")
            strSql.Append("          Inviato,            DataInvio, ")
            strSql.Append("          Data_Creazione,     Data_Modifica, ")
            strSql.Append("          UserName_Creazione, UserName_Modifica, ")
            strSql.Append("          Validita_Inizio,    Validita_Fine, Ora ")
            strSql.Append("         ) ")

            strSql.Append(" VALUES ( ")
            strSql.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Ricetta_Cod))
            strSql.Append("         , " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            strSql.Append("         , '" & Agro_SQL_SaveText(Ricetta_Operazione_Des) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Num_Protocollo) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Rcdpi) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Int) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Mezzo) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Gru_Op) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Costo) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Noleggio_Passivo) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Tp_Fer) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(EM_Cod) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Eff_Perc) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Disciplinare_PubblicoPrivato) & " ")
            strSql.Append("         , '" & Agro_SQL_SaveText(Note) & "'  ")
            strSql.Append("         , 0 ")
            strSql.Append("         , " & Agro_SQL_SaveNum_NULL(W_Anagrafica_Stati_Cod) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum_NULL(Ricetta_Operazione_Cod_RIF) & " ")
            strSql.Append("         , " & Agro_SQL_SaveText_NULL(APP_Ricetta_Operazione_ID) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum_NULL(Invia_App) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Raccoglitore_Cod) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum_NULL(Invia_HubIoT) & " ")
            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")
            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            If (Ora IsNot Nothing) Then
                strSql.Append("         , " & Agro_SQL_SaveDateTime(Ora) & "  ")
            Else
                strSql.Append("         , " & Agro_SQL_SaveDateTime(Validita_Inizio) & "  ")
            End If

            strSql.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    '##############################################################################################
    Public Function Modifica(ByVal Ricetta_Cod As Int32,
                             ByVal Ricetta_Operazione_Cod As Int32,
                             ByVal Lav_Cod As Int32,
                             ByVal Ricetta_Operazione_Des As String,
                             ByVal Note As String,
                             ByVal Num_Protocollo As Decimal,
                             ByVal Id_Rcdpi As Int32,
                             ByVal Extra_Int As Int32,
                             ByVal Mezzo As Int32,
                             ByVal Gru_Op As Int32,
                             ByVal Costo As Decimal,
                             ByVal Noleggio_Passivo As Int32,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal Ora As Date? = Nothing
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Operazioni_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            '---------------------------------------------

            strSql.Length = 0

            strSql.Append(" UPDATE Ricette_Operazioni SET ")
            strSql.Append("    Lav_Cod                  =  " & Agro_SQL_SaveNum(Lav_Cod) & "  ")
            strSql.Append("   ,Ricetta_Operazione_Des   =  '" & Agro_SQL_SaveText(Ricetta_Operazione_Des) & "'  ")
            strSql.Append("   ,Note                     =  '" & Agro_SQL_SaveText(Note) & "' ")
            strSql.Append("   ,Num_Protocollo           =  " & Agro_SQL_SaveNum(Num_Protocollo) & "  ")
            strSql.Append("   ,Id_Rcdpi                 =  " & Agro_SQL_SaveNum(Id_Rcdpi) & "  ")
            strSql.Append("   ,Extra_Int                =  " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            strSql.Append("   ,Mezzo                    =  " & Agro_SQL_SaveNum(Mezzo) & "  ")
            strSql.Append("   ,Gru_Op                   =  " & Agro_SQL_SaveNum(Gru_Op) & "  ")
            strSql.Append("   ,Costo                    =  " & Agro_SQL_SaveNum(Costo) & "  ")
            strSql.Append("   ,Noleggio_Passivo         =  " & Agro_SQL_SaveNum(Noleggio_Passivo) & "  ")

            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            If Ora IsNot Nothing Then
                strSql.Append("   ,Ora     =  " & Agro_SQL_SaveDateTime(Ora))
            Else
                strSql.Append("   ,Ora     =  " & Agro_SQL_SaveDateTime(Validita_Inizio))
            End If


            strSql.Append(" WHERE  Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Ricetta_Cod <> 0 Then
                strSql.Append(" AND Ricette_Operazioni.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                strSql.Append(" AND Ricette_Operazioni.Ricetta_operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    ' L'overload ha 3 campi in più: Id_Tp_Fer, EM_Cod, Eff_Perc
    '##############################################################################################
    Public Function Modifica(ByVal Ricetta_Cod As Int32,
                             ByVal Ricetta_Operazione_Cod As Int32,
                             ByVal Lav_Cod As Int32,
                             ByVal Ricetta_Operazione_Des As String,
                             ByVal Note As String,
                             ByVal Num_Protocollo As Decimal,
                             ByVal Id_Rcdpi As Int32,
                             ByVal Extra_Int As Int32,
                             ByVal Mezzo As Int32,
                             ByVal Gru_Op As Int32,
                             ByVal Costo As Decimal,
                             ByVal Noleggio_Passivo As Int32,
                             ByVal Id_Tp_Fer As Int32,
                             ByVal EM_Cod As Int32,
                             ByVal Eff_Perc As Decimal,
                             ByVal Disciplinare_PubblicoPrivato As Int32,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal Ora As Date? = Nothing
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Operazioni_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            '---------------------------------------------

            strSql.Length = 0

            strSql.Append(" UPDATE Ricette_Operazioni SET ")
            strSql.Append("    Lav_Cod                  =  " & Agro_SQL_SaveNum(Lav_Cod) & "  ")
            strSql.Append("   ,Ricetta_Operazione_Des   =  '" & Agro_SQL_SaveText(Ricetta_Operazione_Des) & "'  ")
            strSql.Append("   ,Note                     =  '" & Agro_SQL_SaveText(Note) & "' ")
            strSql.Append("   ,Num_Protocollo           =  " & Agro_SQL_SaveNum(Num_Protocollo) & "  ")
            strSql.Append("   ,Id_Rcdpi                 =  " & Agro_SQL_SaveNum(Id_Rcdpi) & "  ")
            strSql.Append("   ,Extra_Int                =  " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            strSql.Append("   ,Mezzo                    =  " & Agro_SQL_SaveNum(Mezzo) & "  ")
            strSql.Append("   ,Gru_Op                   =  " & Agro_SQL_SaveNum(Gru_Op) & "  ")
            strSql.Append("   ,Costo                    =  " & Agro_SQL_SaveNum(Costo) & "  ")
            strSql.Append("   ,Noleggio_Passivo         =  " & Agro_SQL_SaveNum(Noleggio_Passivo) & "  ")
            strSql.Append("   ,Id_Tp_Fer                =  " & Agro_SQL_SaveNum(Id_Tp_Fer) & "  ")
            strSql.Append("   ,EM_Cod                   =  " & Agro_SQL_SaveNum(EM_Cod) & "  ")
            strSql.Append("   ,Eff_Perc                 =  " & Agro_SQL_SaveNum(Eff_Perc) & "  ")
            strSql.Append("   ,Disciplinare_PubblicoPrivato         =  " & Agro_SQL_SaveNum(Disciplinare_PubblicoPrivato) & "  ")

            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            If Ora IsNot Nothing Then
                strSql.Append("   ,Ora     =  " & Agro_SQL_SaveDateTime(Ora))
            Else
                strSql.Append("   ,Ora     =  " & Agro_SQL_SaveDateTime(Validita_Inizio))
            End If

            strSql.Append(" WHERE  Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Ricetta_Cod <> 0 Then
                strSql.Append(" AND Ricette_Operazioni.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                strSql.Append(" AND Ricette_Operazioni.Ricetta_operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Cancella(ByVal Ricetta_Cod As Int32,
                             ByVal Ricetta_Operazione_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Operazioni_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_SuperUser = ""
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'If Piva = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            'End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                strSql.Length = 0
                strSql.Append(" UPDATE Ricette_Operazioni ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("      ,Inviato = -1 ")
                strSql.Append(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM Ricette_Operazioni ")
                strSql.Append(" WHERE  1=1 ")

            End If

            If objParametri.PivaSuperUser <> "" Then
                strSql.Append(" AND Ricette_Operazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Ricetta_Cod <> 0 Then
                strSql.Append(" AND Ricette_Operazioni.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                strSql.Append(" AND Ricette_Operazioni.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
