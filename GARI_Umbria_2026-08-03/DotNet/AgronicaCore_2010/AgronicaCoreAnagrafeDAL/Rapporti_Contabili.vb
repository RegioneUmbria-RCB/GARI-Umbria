Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class Rapporti_Contabili_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function RapportiContabili_X_Documenti_Leggi(ByVal Piva As String,
                                                        ByVal rapporto_Attivo As Boolean,
                                                        ByVal tipo_Rapporto As enum_Rapporti_Contabili_Standard,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R.RapportiContabili_X_Documenti_Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" select distinct  ")
            StrSQL.Append(" ru.Cod_RisUm,  c.Rag_Soc + c.Cognome + ' ' + c.Nome AS Rag_Soc ")
            StrSQL.Append(" From Risorse_Umane ru ")
            StrSQL.Append(" Left join Contatti c ")
            StrSQL.Append(" On ru.Cod_Contatto = c.Cod_Contatto and c.Piva = ru.Piva ")
            StrSQL.Append(" where (ru.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' or c.Sa_Cod = -1) ")

            If rapporto_Attivo Then
                StrSQL.Append(" AND ru.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                StrSQL.Append(" AND   ru.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            End If

            If tipo_Rapporto <> 0 Then
                StrSQL.Append(" AND ru.Cod_Rapporto = " & Agro_SQL_SaveNum(tipo_Rapporto))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY c.Rag_Soc + c.Cognome + ' ' + c.Nome Asc ")
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

    Public Function RapportiContabili_X_Contatto_Leggi(ByVal Piva As String,
                                                       ByVal Cod_Contatto As String,
                                                       ByVal Cod_Rapporto As Integer,
                                                       ByVal Cod_RisUm As Integer,
                                                       ByVal xOrderBy As String,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R.RapportiContabili_X_Contatto_Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  ru.Cod_RisUm, ru.piva, ru.sa_cod ")
            StrSQL.Append("  ,COALESCE(ru.Cod_Contatto, '') as Cod_Contatto, ru.Cod_Rapporto, ru.Settore_Des ")
            StrSQL.Append(" ,rc.Rapporto_Des ")
            StrSQL.Append(" ,ru.Qualifica_Cod ")
            StrSQL.Append(" ,q.Qualifica_Des ")
            StrSQL.Append(" ,ru.Mansione_Cod ")
            StrSQL.Append(" ,m.Mansione_Des ")
            StrSQL.Append(" ,COALESCE(ru.Validita_Inizio, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & ") as  Validita_Inizio ")
            StrSQL.Append(" ,COALESCE(ru.Validita_Fine , " & Agro_SQL_SaveDate(AGRODATAFINE) & ") as Validita_Fine")
            StrSQL.Append(" ,ru.Attivita_Des ")
            StrSQL.Append(" ,ru.occasionale ")
            StrSQL.Append(" ,ru.Ore_Settimanali ")
            StrSQL.Append(" ,ru.Info_Famiglia ")
            StrSQL.Append(" ,ru.Classificazione_Cod ")
            StrSQL.Append(" ,ruc.Descrizione as Classificazione_Des")
            StrSQL.Append(" ,ru.occasionale as TipoRapporto_Cod ")
            StrSQL.Append(" ,ru.Cod_Iva_Contatto, ru.Cod_Conto_Econ, ru.Cod_Conto_Pat ")
            StrSQL.Append(" ,iv.Descrizione, c.Conto_Descr, cp.Conto_Pat_Descr As Descr_Conto_Pat, ")
            StrSQL.Append(" case when ru.occasionale = 0 then 'Continuativo' 	else 	'Occasionale' end as TipoRapporto_Des ")
            StrSQL.Append(" ,ru.Patentino ")
            StrSQL.Append(" ,Ente_di_rilascio ")
            StrSQL.Append(" ,ru.Data_Rilascio_Patentino ")
            StrSQL.Append(" ,ru.Data_Scadenza_Patentino, ")
            StrSQL.Append(" ru.piva + '_' + convert(nvarchar(10), ru.sa_cod) + '_' + convert(nvarchar(10), ru.Cod_RisUm) + '_' + ru.Cod_Contatto as key_rap_cont, ")
            StrSQL.Append(" rc.Dipendente, rc.Terzista, rc.Legale ")


            'Left Join Qualifiche q on ru.Piva = q.Piva And ru.Qualifica_Cod = q.Qualifica_Cod
            'Left Join Mansioni m on ru.Piva = m.Piva And ru.Mansione_Cod = m.Mansione_Cod
            'Left Join Risorse_Umane_Classificazioni ruc on ru.Piva = ruc.Piva And ru.Classificazione_Cod = ruc.id_risum_cl
            StrSQL.Append(" FROM    Risorse_Umane ru ")
            StrSQL.Append(" inner Join Rapporti_Contabili rc")
            StrSQL.Append(" On ru.Cod_Rapporto = rc.Cod_Rapporto ")
            StrSQL.Append(" And (rc.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' or rc.Piva = '" & Agro_SQL_SaveText(Piva) & "' )")
            StrSQL.Append(" left join Qualifiche q on ru.Qualifica_Cod = q.Qualifica_Cod And (q.Piva = '" & Agro_SQL_SaveText(Piva) & "' or q.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' )")
            StrSQL.Append(" Left Join Risorse_Umane_Classificazioni ruc on ru.Piva = ruc.Piva And ru.Classificazione_Cod = ruc.id_risum_cl ")
            StrSQL.Append(" left join Mansioni m on ru.Piva = m.Piva and ru.Mansione_Cod = m.Mansione_Cod")
            StrSQL.Append(" left join IVA_Aliquote iv on Cod_Iva_Contatto = iv.Codice ")
            StrSQL.Append(" Left join Conti c on Cod_Conto_Econ = c.Cod_Conto ")
            StrSQL.Append(" Left join Conti_Patrimonio cp on ru.Cod_Conto_Pat = cp.Cod_Conto_Pat ")
            StrSQL.Append(" WHERE     ru.Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append(" AND ru.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'")
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            If Cod_Rapporto <> 0 Then
                StrSQL.Append(" AND  ru.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " ")
            End If
            If Cod_RisUm <> 0 Then
                StrSQL.Append(" AND  ru.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY rc.Rapporto_Des, ru.Cod_Rapporto Asc ")

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
    'il default sul sa_cod è SACOD_CONTATTO_NONDEFINITO, perché 0 ha significato
    Public Function Contatti_RapportiContabili_Leggi(ByVal Sa_Cod As Integer,
                                                     ByVal Cod_Rapporto As Integer,
                                                     ByVal Cliente As Boolean,
                                                     ByVal Fornitore As Boolean,
                                                     ByVal Dipendente As Boolean,
                                                     ByVal Terzista As Boolean,
                                                     ByVal Legale As Boolean,
                                                     ByVal Agente As Boolean,
                                                     ByVal Consulente As Boolean,
                                                     ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByVal xOrderBy As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R.Contatti_RapportiContabili_Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  *   ")
                    StrSQL.Append(" FROM    Rapporti_Contabili ")
                    StrSQL.Append(" WHERE     (Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")

                    If Sa_Cod <> SACOD_CONTATTO_NONDEFINITO Then
                        StrSQL.Append(" AND     (Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")   ")
                    End If

                    If Cod_Rapporto <> 0 Then
                        StrSQL.Append(" AND     (Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & ")   ")
                    End If

                    If Cliente Then
                        StrSQL.Append(" AND     (Cliente = " & Agro_SQL_SaveNum(1) & ")   ")
                    End If

                    If Fornitore Then
                        StrSQL.Append(" AND     (Fornitore = " & Agro_SQL_SaveNum(1) & ")   ")
                    End If

                    If Dipendente Then
                        StrSQL.Append(" AND     (Dipendente = " & Agro_SQL_SaveNum(1) & ")   ")
                    End If

                    If Terzista Then
                        StrSQL.Append(" AND     (Terzista = " & Agro_SQL_SaveNum(1) & ")   ")
                    End If

                    If Legale Then
                        StrSQL.Append(" AND     (Legale = " & Agro_SQL_SaveNum(1) & ")   ")
                    End If

                    If Agente Then
                        StrSQL.Append(" AND     (Agente = " & Agro_SQL_SaveNum(1) & ")   ")
                    End If

                    If Consulente Then
                        StrSQL.Append(" AND     (Consulente = " & Agro_SQL_SaveNum(1) & ")   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rapporto_Des, Cod_Rapporto Asc ")

                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

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

    Public Function RapportiContabili_Leggi(ByVal Piva As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R.RapportiContabili_Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" select * from rapporti_contabili  ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" WHERE " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Rapporto_Des Asc ")
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

    ''#############################################################################
    'Public Function RapportiContabili_Verifica_TipoPersona(ByRef Errore As String,
    '                                                      ByVal cod_Rapporto As Integer,
    '                                                      ByVal rapporto_des As String,
    '                                                      ByVal id_cf As Integer) As Boolean

    '    Dim risp As Boolean = False

    '    Select Case cod_Rapporto

    '        Case COD_LAB_ANALISI, COD_CENTRO_MACCHINE, COD_SPEDIZIONIERE

    '            If id_cf = PERSONA_FISICA Then
    '                Errore += vbCrLf & "Il rapporto contabile " & rapporto_des & " può essere associato solo a persone giuridiche."
    '            End If

    '        Case COD_REFERENTEAZIENDALE, COD_TECNICORESPONSABILE, COD_TECNICO, COD_SOCIO, COD_LEGALE, COD_LAB_CQ

    '            If id_cf = PERSONA_GIURIDICA Then
    '                Errore += vbCrLf & "Il rapporto contabile " & rapporto_des & " può essere associato solo a persone fisiche."
    '            End If

    '        Case Else
    '            risp = True

    '    End Select

    '    Return risp

    'End Function

    '#############################################################################
    Public Function VerificaCompatibilita_RapportoContabile_TipoPersona(ByRef Errore As String,
                                                                        ByVal cod_Rapporto As Integer,
                                                                        ByVal rapporto_des As String,
                                                                        ByVal id_cf As Integer,
                                                                        ByRef objParametri As AgronicaCoreParametri
                                                                        ) As Boolean

        Dim flagOk As Boolean = True
        Dim dt As DataTable
        Dim saCod As Integer

        dt = Contatti_RapportiContabili_Leggi(SACOD_CONTATTO_NONDEFINITO,
                                              cod_Rapporto,
                                              False,
                                              False,
                                              False,
                                              False,
                                              False,
                                              False,
                                              False,
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "", "",
                                              objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            saCod = dt.Rows(0).Item("sa_cod")

            Select Case saCod

                Case enum_RapportiContabili_SaCod.PersoneGiuridiche

                    If id_cf = PERSONA_FISICA Then
                        Errore &= vbCrLf & "Il rapporto contabile " & rapporto_des & " può essere associato solo a persone giuridiche."
                        flagOk = False
                    End If

                Case enum_RapportiContabili_SaCod.PersoneFisiche

                    If id_cf = PERSONA_GIURIDICA Then
                        Errore &= vbCrLf & "Il rapporto contabile " & rapporto_des & " può essere associato solo a persone fisiche."
                        flagOk = False
                    End If

            End Select

        Else
            Errore &= vbCrLf & "cod_Rapporto " & CStr(cod_Rapporto) & " non trovato."
            flagOk = False
        End If

        Return flagOk

    End Function


    '#############################################################################
    'Verifica se sono presenti tutti i rapporti contabili di base.
    'aggiunge quelli mancanti
    Public Sub RapportiContabiliDiBase_Verifica(ByRef Errore As String,
                                                ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R.RapportiContabiliDiBase_Verifica()"

        Dim dtRappCont As DataTable
        Dim numRappCont As Integer = 0

        'leggo i rapporti contabili di base (cod_rapporto negativo)
        dtRappCont = Contatti_RapportiContabili_Leggi(SACOD_CONTATTO_NONDEFINITO,
                                                      0,
                                                      False,
                                                      False,
                                                      False,
                                                      False,
                                                      False,
                                                      False,
                                                      False,
                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                      " (Rapporti_Contabili.Cod_Rapporto < 0) ",
                                                      "",
                                                      objParametri)

        If Not IsNothing(dtRappCont) Then

            numRappCont = dtRappCont.Rows.Count

            Dim objRapportiContabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W

            Select Case numRappCont

                Case 0 'non c'è alcun rapporto contabile -> li inserisco tutti

                    objRapportiContabili.RapportiContabiliDiBase_Inserisci(Errore, objParametri)

                    '----------------------------

                Case Else

                    'manca qualche rapporto contabile

                    Dim numeroRecordInteressati As Integer = 0
                    Dim i, codRapporto As Integer

                    Dim Flag_Legale As Boolean = False
                    Dim Flag_Cliente As Boolean = False
                    Dim Flag_Fornitore As Boolean = False
                    Dim Flag_Dipendente As Boolean = False
                    Dim Flag_Terzista As Boolean = False
                    Dim Flag_Tecnico As Boolean = False
                    Dim Flag_CentroMacchine As Boolean = False
                    Dim Flag_LabAnalisi As Boolean = False
                    Dim Flag_Socio As Boolean = False
                    Dim Flag_Trasportatore As Boolean = False
                    Dim Flag_SocioConferente As Boolean = False
                    Dim Flag_CoopConferente As Boolean = False
                    Dim Flag_ProdAssociatoCoop As Boolean = False
                    Dim Flag_ProdIndivisuale As Boolean = False
                    Dim Flag_TecnicoResponsabile As Boolean = False
                    Dim Flag_Agente As Boolean = False
                    Dim Flag_ReferenteAziendale As Boolean = False
                    Dim Flag_Consulente As Boolean = False
                    Dim Flag_Spedizioniere As Boolean = False
                    Dim Flag_Vivaio As Boolean = False
                    Dim Flag_Conferente As Boolean = False
                    Dim Flag_Lab_CQ As Boolean = False
                    Dim Flag_CapoArea As Boolean = False
                    Dim Flag_FornOrtofrutta As Boolean = False


                    'scorro i rapporti contabili e verifico quelli che mancano
                    For i = 0 To dtRappCont.Rows.Count - 1

                        codRapporto = dtRappCont.Rows(i).Item("Cod_Rapporto")

                        Select Case codRapporto
                            Case COD_LEGALE
                                Flag_Legale = True
                            Case COD_CLIENTE
                                Flag_Cliente = True
                            Case COD_FORNITORE
                                Flag_Fornitore = True
                            Case COD_DIPENDENTE
                                Flag_Dipendente = True
                            Case COD_TERZISTA
                                Flag_Terzista = True
                            Case COD_TECNICO
                                Flag_Tecnico = True
                            Case COD_CENTRO_MACCHINE
                                Flag_CentroMacchine = True
                            Case COD_LAB_ANALISI
                                Flag_LabAnalisi = True
                            Case COD_SOCIO
                                Flag_Socio = True
                            Case COD_TRASPORTATORE
                                Flag_Trasportatore = True
                                'DA ABILITARE QUANDO CI SARA' IL CONFERIMENTO/ACCETTAZIONE
                                'Case COD_CLIENTE_FORNITORE
                                '    Flag_ClienteFornitore = True
                            Case COD_TECNICORESPONSABILE
                                Flag_TecnicoResponsabile = True
                            Case COD_AGENTE
                                Flag_Agente = True
                            Case COD_REFERENTEAZIENDALE
                                Flag_ReferenteAziendale = True
                            Case COD_CONSULENTE
                                Flag_Consulente = True
                            Case COD_SPEDIZIONIERE
                                Flag_Spedizioniere = True
                            Case COD_VIVAIO
                                Flag_Vivaio = True
                            Case COD_CONFERENTE
                                Flag_Conferente = True
                            Case COD_LAB_CQ
                                Flag_Lab_CQ = True
                            Case COD_CAPO_AREA
                                Flag_CapoArea = True
                            Case COD_FORNITORE_ORTOFRUTTA
                                Flag_FornOrtofrutta = True
                        End Select

                    Next

                    '==========================================================================

                    'inserisco i rapporti contabili mancanti
                    If Not Flag_Legale Then
                        objRapportiContabili.RapportiContabili_Legale_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_Cliente Then
                        objRapportiContabili.RapportiContabili_Cliente_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_Fornitore Then
                        objRapportiContabili.RapportiContabili_Fornitore_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_Dipendente Then
                        objRapportiContabili.RapportiContabili_Dipendente_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_Terzista Then
                        objRapportiContabili.RapportiContabili_Terzista_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_Tecnico Then
                        objRapportiContabili.RapportiContabili_Tecnico_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_CentroMacchine Then
                        objRapportiContabili.RapportiContabili_CentroMacchine_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_LabAnalisi Then
                        objRapportiContabili.RapportiContabili_LaboratorioAnalisi_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_Socio Then
                        objRapportiContabili.RapportiContabili_Socio_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_Trasportatore Then
                        objRapportiContabili.RapportiContabili_Trasportatore_Inserisci(Errore, objParametri)
                    End If

                    'DA ABILITARE QUANDO CI SARA' IL CONFERIMENTO/ACCETTAZIONE
                    'If Not Flag_ClienteFornitore Then
                    '    RapportiContabili_ClienteFornitore_Inserisci(objServer, objSession, objPage, Errore)
                    'End If

                    If Not Flag_TecnicoResponsabile Then
                        objRapportiContabili.RapportiContabili_TecnicoResponsabile_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_Agente Then
                        objRapportiContabili.RapportiContabili_Agente_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_ReferenteAziendale Then
                        objRapportiContabili.RapportiContabili_ReferenteAziendale_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_Consulente Then
                        objRapportiContabili.RapportiContabili_Consulente_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_Spedizioniere Then
                        objRapportiContabili.RapportiContabili_Spedizioniere_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_Vivaio Then
                        objRapportiContabili.RapportiContabili_Vivaio_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_Conferente Then
                        objRapportiContabili.RapportiContabili_Conferente_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_Lab_CQ Then
                        objRapportiContabili.RapportiContabili_Operatore_Lab_CQ_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_CapoArea Then
                        objRapportiContabili.RapportiContabili_CapoArea_Inserisci(Errore, objParametri)
                    End If
                    If Not Flag_FornOrtofrutta Then
                        objRapportiContabili.RapportiContabili_Fornitore_Ortofrutta_Inserisci(Errore, objParametri)
                    End If

            End Select

        End If

    End Sub


    Public Function RapportiContabilixCostiAccessori(ByVal Piva As String,
                                                     ByVal Dipendente As Boolean,
                                                     ByVal Terzista As Boolean,
                                                     ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByVal xOrderBy As String,
                                                     ByRef objParametri As AgronicaCoreParametri,
                                                     Optional isModificaMultipla As Boolean = False,
                                                     Optional FlagPubblico As Boolean = True
                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R.RapportiContabilixCostiAccessori()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim i As Integer
        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Rapporti_Contabili.Rapporto_Des, Risorse_Umane.Settore_Des, ")
                    StrSQL.AppendLine(" CASE WHEN (Contatti.Rag_Soc is null or Contatti.Rag_Soc = '') then Contatti.Cognome  + ' ' +  Contatti.Nome ")
                    StrSQL.AppendLine("      ELSE Contatti.Rag_Soc ")
                    StrSQL.AppendLine(" END AS Col_1 , ")
                    StrSQL.AppendLine(" Risorse_Umane.Cod_RisUm as Col_8, ISNULL(Prodotti_Costi.Mezzo, 2) as Col_5, ISNULL(Prodotti_Costi.Prezzo_Unitario, 0) as Col_9 , Contatti.Nome, Contatti.Cognome, Risorse_Umane.Qualifica_Cod ,Risorse_Umane.Cod_Rapporto, Qualifiche.Qualifica_Des, ")
                    StrSQL.AppendLine(" Contatti.Cod_Contatto, Contatti.Piva")

                    If isModificaMultipla Then
                        StrSQL.AppendLine(" , Contatti.Sa_Cod ")
                        StrSQL.AppendLine(" , CASE WHEN (Contatti.Rag_Soc is null or Contatti.Rag_Soc = '') then Contatti.Cognome  + ' ' +  Contatti.Nome else Contatti.Rag_Soc End AS Rag_Soc_Nome_Cognome  ")
                        StrSQL.AppendLine(" , Risorse_Umane.Cod_RisUm as Cod_RisUm  ")
                        StrSQL.AppendLine(" , Imprese.Rag_Soc AS Impresa  ")
                        StrSQL.AppendLine(" , Rapporti_Contabili.Fornitore, Rapporti_Contabili.Dipendente, Rapporti_Contabili.Terzista  ")
                        StrSQL.AppendLine(" , ISNULL(Prodotti_Costi.Mezzo, 2) as Mezzo ")
                        StrSQL.AppendLine(" , ISNULL(Prodotti_Costi.Elem_Cod, 0) AS Elem_Cod ")
                        StrSQL.AppendLine(" , ISNULL(Prodotti_Costi.Prezzo_Unitario, 0) as Prezzo_Unitario    ")
                        StrSQL.AppendLine(" , ISNULL(Prodotti_Costi.Validita_Inizio, '01/01/1900') AS Costo_Inizio  ")
                        StrSQL.AppendLine(" , ISNULL(Prodotti_Costi.Validita_Fine, '31/12/2100') AS Costo_Fine")
                    End If


                    StrSQL.AppendLine(" FROM   Rapporti_Contabili ")
                    StrSQL.AppendLine(" INNER JOIN Risorse_Umane ON Rapporti_Contabili.Cod_Rapporto = Risorse_Umane.Cod_Rapporto  ")
                    StrSQL.AppendLine(" INNER JOIN Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto ")
                    StrSQL.AppendLine(" INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.PIVA AND Rapporti_Contabili.Piva = UtentiXImprese.[USER] ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Prodotti_Costi ON Risorse_Umane.Cod_RisUm = Prodotti_Costi.Mat_Cod AND Prodotti_Costi.Elem_Cod = 0 And Prodotti_Costi.Id_Budget = 0 ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Qualifiche ON Risorse_Umane.Qualifica_Cod = Qualifiche.Qualifica_Cod ")

                    If isModificaMultipla Then
                        StrSQL.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Contatti.Piva ")
                    End If
                    'StrSQL.AppendLine(" WHERE  (Contatti.Sa_Cod = - 1 or Risorse_Umane.Piva = '" & Agro_SQL_SaveText(Piva) & "') ")
                    StrSQL.AppendLine(" WHERE 1 = 1 ")

                    'StrSQL.AppendLine(" AND ( Prodotti_Costi.Elem_Cod = 0) ")
                    If Dipendente Then
                        StrSQL.AppendLine(" AND ( Rapporti_Contabili.Dipendente = 1 ) ")
                    End If
                    If Terzista Then
                        StrSQL.AppendLine(" AND (Rapporti_Contabili.Terzista = 1) ")
                    End If

                    StrSQL.AppendLine(" AND ( Risorse_Umane.Cod_RisUm_Origine = 0 ) ")
                    StrSQL.AppendLine(" AND UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    If isModificaMultipla AndAlso Not FlagPubblico  Then
                        StrSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)
                    Else

                        'pubbliche
                        StrSQL.AppendLine(" AND ( (Contatti.Sa_Cod = -1) ")

                        'visibilità centro
                        'leggo se ci sono filtri sui centri
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim filtroCentri As String = ""
                        Dim dtCentriVisibili As DataTable
                        dtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If dtCentriVisibili IsNot Nothing AndAlso dtCentriVisibili.Rows.Count > 0 Then
                            For i = 0 To dtCentriVisibili.Rows.Count - 1
                                filtroCentri &= " (Contatti.Piva = '" & dtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & dtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                            Next
                            If filtroCentri <> "" Then
                                StrSQL.AppendLine(" OR (" & Left(filtroCentri, filtroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                            End If
                        End If

                        'aziendali
                        If filtroCentri = "" Then
                            StrSQL.AppendLine("          OR  (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
                        End If

                        StrSQL.AppendLine("        ) ")
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Col_1 ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

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

    Public Function RapportiContabilixCostiAccessori_ImpreseGias(ByVal Piva As String,
                                                                 ByVal Dipendente As Boolean,
                                                                 ByVal Terzista As Boolean,
                                                                 ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                                 ByVal xFiltroAggiuntivo As String,
                                                                 ByVal xOrderBy As String,
                                                                 ByRef objParametri As AgronicaCoreParametri
                                                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R.RapportiContabilixCostiAccessori_ImpreseGias()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim i As Integer
        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Rapporti_Contabili.Rapporto_Des, Risorse_Umane.Settore_Des,     ")
                    StrSQL.Append(" CASE WHEN (Contatti.Rag_Soc is null or Contatti.Rag_Soc = '') then Contatti.Cognome  + ' ' +  Contatti.Nome ")
                    StrSQL.Append(" else Contatti.Rag_Soc                    End    AS Col_1 , ")
                    StrSQL.Append(" Risorse_Umane.Cod_RisUm as Col_8, ISNULL(Prodotti_Costi.Mezzo, 2) as Col_5, ISNULL(Prodotti_Costi.Prezzo_Unitario, 0) as Col_9 , Contatti.Nome, Contatti.Cognome  ")
                    StrSQL.Append(" , Contatti.Cod_Contatto, ISNULL(Imprese.rag_soc,'') As rag_soc  ")
                    StrSQL.Append(" FROM   Rapporti_Contabili ")
                    StrSQL.Append(" INNER JOIN Risorse_Umane ON Rapporti_Contabili.Cod_Rapporto = Risorse_Umane.Cod_Rapporto  ")
                    StrSQL.Append(" INNER JOIN Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto ")
                    StrSQL.Append(" INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.PIVA AND Rapporti_Contabili.Piva = UtentiXImprese.[USER] ")
                    StrSQL.Append(" LEFT OUTER JOIN Imprese ON Contatti.Cod_Contatto = Imprese.PIVA ")
                    StrSQL.Append(" LEFT OUTER JOIN Prodotti_Costi ON Risorse_Umane.Cod_RisUm = Prodotti_Costi.Mat_Cod AND Prodotti_Costi.Elem_Cod = 0 And Prodotti_Costi.Id_Budget = 0 ")

                    'StrSQL.Append(" WHERE  (Contatti.Sa_Cod = - 1 or Risorse_Umane.Piva = '" & Agro_SQL_SaveText(Piva) & "') ")
                    StrSQL.Append(" WHERE 1=1 ")

                    'StrSQL.Append(" AND ( Prodotti_Costi.Elem_Cod = 0) ")
                    If Dipendente Then
                        StrSQL.Append(" AND ( Rapporti_Contabili.Dipendente = 1 ) ")
                    End If
                    If Terzista Then
                        StrSQL.Append(" AND (Rapporti_Contabili.Terzista = 1) ")
                    End If
                    StrSQL.Append(" AND ( Risorse_Umane.Cod_RisUm_Origine = 0 ) ")
                    StrSQL.Append(" AND    UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


                    'pubbliche
                    StrSQL.Append(" AND    ( (Contatti.Sa_Cod = -1) ")

                    'visibilità centro
                    'leggo se ci sono filtri sui centri
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim filtroCentri As String = ""
                    Dim dtCentriVisibili As DataTable
                    dtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                    If dtCentriVisibili IsNot Nothing AndAlso dtCentriVisibili.Rows.Count > 0 Then
                        For i = 0 To dtCentriVisibili.Rows.Count - 1
                            filtroCentri &= " (Contatti.Piva = '" & dtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & dtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                        Next
                        If filtroCentri <> "" Then
                            StrSQL.Append(" OR (" & Left(filtroCentri, filtroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                        End If
                    End If

                    'aziendali
                    If filtroCentri = "" Then
                        StrSQL.Append("          OR  (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
                    End If

                    StrSQL.Append("        ) ")



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Col_1 ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

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

    'solo x terzisti
    'carica i contatti terzisti + contatti privati del terzista 
    Public Function RapportiContabilixCostiAccessori_Terzisti(ByVal strPive As String,
                                                              ByVal Dipendente As Boolean,
                                                              ByVal Terzista As Boolean,
                                                              ByVal xFiltroAggiuntivo As String,
                                                              ByVal xOrderBy As String,
                                                              ByRef objParametri As AgronicaCoreParametri
                                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R.RapportiContabilixCostiAccessori_Terzisti()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim i As Integer

        Try

            '--------------------------------------------------------------------------
            ' TERZISTI
            '--------------------------------------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT Rapporti_Contabili.Rapporto_Des, Risorse_Umane.Settore_Des,     ")
            StrSQL.Append(" CASE WHEN (Contatti.Rag_Soc is null or Contatti.Rag_Soc = '') then Contatti.Cognome  + ' ' +  Contatti.Nome ")
            StrSQL.Append(" else Contatti.Rag_Soc                    End    AS Col_1 , ")
            StrSQL.Append(" Risorse_Umane.Cod_RisUm as Col_8, ISNULL(Prodotti_Costi.Mezzo, 2) as Col_5, ISNULL(Prodotti_Costi.Prezzo_Unitario, 0) as Col_9 , Contatti.Nome, Contatti.Cognome  ")
            StrSQL.Append(" , Contatti.Cod_Contatto, Risorse_Umane.piva  ")
            StrSQL.Append(" FROM   Rapporti_Contabili ")
            StrSQL.Append(" INNER JOIN Risorse_Umane ON Rapporti_Contabili.Cod_Rapporto = Risorse_Umane.Cod_Rapporto  ")
            StrSQL.Append(" INNER JOIN Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto ")
            StrSQL.Append(" INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.PIVA AND Rapporti_Contabili.Piva = UtentiXImprese.[USER] ")
            StrSQL.Append(" LEFT OUTER JOIN Prodotti_Costi ON Risorse_Umane.Cod_RisUm = Prodotti_Costi.Mat_Cod AND Prodotti_Costi.Elem_Cod = 0 And Prodotti_Costi.Id_Budget = 0  ")

            StrSQL.Append(" WHERE 1=1 ")

            If Dipendente Then
                StrSQL.Append(" AND ( Rapporti_Contabili.Dipendente = 1 ) ")
            End If
            If Terzista Then
                StrSQL.Append(" AND (Rapporti_Contabili.Terzista = 1) ")
            End If
            StrSQL.Append(" AND ( Risorse_Umane.Cod_RisUm_Origine = 0 ) ")
            StrSQL.Append(" AND    UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            'pubbliche
            'StrSQL.Append(" AND    ( (Contatti.Sa_Cod = -1) ")

            StrSQL.Append(" AND    ( ")

            'visibilità centro
            'leggo se ci sono filtri sui centri
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim filtroCentri As String = ""
            Dim dtCentriVisibili As DataTable
            dtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva IN (" & Agro_SQL_Save_Clausola_IN(strPive, True) & ")", "", objParametri)
            If dtCentriVisibili IsNot Nothing AndAlso dtCentriVisibili.Rows.Count > 0 Then
                For i = 0 To dtCentriVisibili.Rows.Count - 1
                    filtroCentri &= " (Contatti.Piva = '" & dtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & dtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                Next
                If filtroCentri <> "" Then
                    StrSQL.Append(" (" & Left(filtroCentri, filtroCentri.Length - 3) & " OR (Contatti.Piva IN (" & Agro_SQL_Save_Clausola_IN(strPive, True) & ") AND Contatti.Sa_Cod = 0) ) ")
                End If
            End If

            'aziendali
            If filtroCentri = "" Then
                StrSQL.Append("  (Contatti.Piva IN (" & Agro_SQL_Save_Clausola_IN(strPive, True) & "))  ")
            End If

            StrSQL.Append("        ) ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------


            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Col_1 ")
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


    Public Function RapportiContabilixCostiAccessoribyCod_RisUm(ByVal Piva As String,
                                                                ByVal Cod_RisUm As Integer,
                                                                ByVal Dipendente As Boolean,
                                                                ByVal Tersista As Boolean,
                                                                ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                                ByVal xFiltroAggiuntivo As String,
                                                                ByVal xOrderBy As String,
                                                                ByRef objParametri As AgronicaCoreParametri
                                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R.RapportiContabilixCostiAccessoribyCod_RisUm()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Rapporti_Contabili.Rapporto_Des, Risorse_Umane.Settore_Des, Rapporti_Contabili.Terzista, Rapporti_Contabili.Dipendente,  ")
                    StrSQL.Append("    CASE WHEN (Contatti.Rag_Soc is null or Contatti.Rag_Soc = '') then Contatti.Cognome  + ' ' +  Contatti.Nome       else Contatti.Rag_Soc End as Rag_Soc,   ")
                    StrSQL.Append("     Risorse_Umane.Cod_RisUm, ISNULL(Prodotti_Costi.Prezzo_Unitario,0) AS Prezzo_Unitario,  Rapporti_Contabili.Cod_Rapporto, ")
                    StrSQL.Append("     ISNULL(Prodotti_Costi.Mezzo,0) AS Mezzo, ISNULL(Prodotti_Costi.Elem_Cod,0) AS Elem_Cod , Risorse_Umane.Qualifica_Cod, Qualifiche.Qualifica_Des,  ")
                    StrSQL.Append("     Contatti.Cod_Contatto, Contatti.Piva  ")

                    StrSQL.Append(" FROM         Rapporti_Contabili ")
                    StrSQL.Append(" INNER JOIN  Risorse_Umane ON Rapporti_Contabili.Cod_Rapporto = Risorse_Umane.Cod_Rapporto  ")
                    StrSQL.Append(" INNER JOIN  Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  ")
                    StrSQL.Append(" INNER JOIN   UtentiXImprese ON Contatti.Piva = UtentiXImprese.PIVA AND Rapporti_Contabili.Piva = UtentiXImprese.[USER]  ")
                    StrSQL.Append(" LEFT OUTER JOIN Qualifiche ON Risorse_Umane.Qualifica_Cod = Qualifiche.Qualifica_Cod ")
                    StrSQL.Append(" LEFT Outer JOIN  Prodotti_Costi ON Risorse_Umane.Cod_RisUm = Prodotti_Costi.Mat_Cod And Prodotti_Costi.Id_Budget = 0 ")
                    StrSQL.Append(" AND Prodotti_Costi.Elem_Cod = 0 ")
                    StrSQL.Append(" WHERE  (Contatti.Sa_Cod = - 1 or Risorse_Umane.Piva = '" & Agro_SQL_SaveText(Piva) & "') ")
                    ' StrSQL.Append(" AND ( Prodotti_Costi.Elem_Cod = 0) ")
                    If Cod_RisUm <> 0 Then
                        StrSQL.Append(" AND Risorse_Umane.Cod_RisUm  = " & Cod_RisUm & " ")
                    End If

                    If Dipendente Then
                        StrSQL.Append(" AND ( Rapporti_Contabili.Dipendente = 1 ) ")
                    End If
                    If Tersista Then
                        StrSQL.Append(" AND (Rapporti_Contabili.Terzista = 1) ")
                    End If
                    StrSQL.Append(" AND ( Risorse_Umane.Cod_RisUm_Origine = 0 ) ")
                    StrSQL.Append(" AND    UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


                    'pubbliche
                    StrSQL.Append(" AND    ( (Contatti.Sa_Cod = -1) ")

                    'visibilità centro
                    'leggo se ci sono filtri sui centri
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim filtroCentri As String = ""
                    Dim dtCentriVisibili As DataTable
                    dtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                    If dtCentriVisibili IsNot Nothing AndAlso dtCentriVisibili.Rows.Count > 0 Then
                        For i = 0 To dtCentriVisibili.Rows.Count - 1
                            filtroCentri &= " (Contatti.Piva = '" & dtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & dtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                        Next
                        If filtroCentri <> "" Then
                            StrSQL.Append(" OR (" & Left(filtroCentri, filtroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                        End If
                    End If

                    'aziendali
                    If filtroCentri = "" Then
                        StrSQL.Append("          OR  (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
                    End If

                    StrSQL.Append("        ) ")

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rapporti_Contabili.Rapporto_Des ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

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

    Public Function RapportiContabilixCostiAccessori_Flag_Data_Scadenza_Patentino(ByVal Piva As String,
                                                                                  ByVal Dipendente As Boolean,
                                                                                  ByVal Terzista As Boolean,
                                                                                  ByVal Data As Date,
                                                                                  ByVal Flag_Data_Scadenza_Patentino As Boolean,
                                                                                  ByVal xFiltroAggiuntivo As String,
                                                                                  ByVal xOrderBy As String,
                                                                                  ByRef objParametri As AgronicaCoreParametri
                                                                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R.RapportiContabilixCostiAccessori_Flag_Data_Scadenza_Patentino()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim i As Integer
        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Rapporti_Contabili.Rapporto_Des, Risorse_Umane.Settore_Des, ")
            StrSQL.AppendLine(" CASE WHEN (Contatti.Rag_Soc is null or Contatti.Rag_Soc = '') then Contatti.Cognome  + ' ' +  Contatti.Nome ")
            StrSQL.AppendLine(" else Contatti.Rag_Soc                    End    AS Rag_Soc , ")
            StrSQL.AppendLine(" Risorse_Umane.Cod_RisUm, Contatti.Nome, Contatti.Cognome, Risorse_Umane.Qualifica_Cod ,Risorse_Umane.Cod_Rapporto,")
            StrSQL.AppendLine(" REPLACE(LTRIM(RTRIM(Contatti.Cod_Contatto)), char(9), '') AS Cod_Contatto, Contatti.Piva,")

            If Flag_Data_Scadenza_Patentino Then
                StrSQL.AppendLine("ISNULL((SELECT top 1 Alert_Elenco.Data_Scadenza  ")
                StrSQL.AppendLine("From alert_entita  ")
                StrSQL.AppendLine("INNER JOIN Alert_Elenco ON Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser And Alert_Elenco.ID_Alert_Entita = Alert_Entita.ID_Alert_Entita")
                StrSQL.AppendLine("INNER JOIN Allegati_Documenti ON Allegati_Documenti.Allegati_Documenti_SuperUser = Alert_Entita.PivaSuperUser and Allegati_Documenti.Allegati_Documenti_Piva = Alert_Entita.piva  and")
                StrSQL.AppendLine("Allegati_Documenti.Allegati_Documenti_Cod = Alert_Entita.Allegati_Documenti_Cod")
                StrSQL.AppendLine("WHERE tipoentita_cod=8")
                StrSQL.AppendLine("And Allegati_Documenti.Allegati_documenti_CatCod=2  ")
                StrSQL.AppendLine("And Allegati_Documenti.Validazione_Data <= " & Agro_SQL_SaveDate(Data))
                StrSQL.AppendLine("And Alert_Elenco.Data_Scadenza >=  Alert_Elenco.Data_Scadenza")
                StrSQL.AppendLine("And   REPLACE(LTRIM(RTRIM(alert_entita.Cod_Contatto)), char(9), '') = REPLACE(LTRIM(RTRIM(Contatti.Cod_Contatto)), char(9), '') and alert_entita.piva = Contatti.piva")
                StrSQL.AppendLine("order by Alert_Elenco.Data_Scadenza desc),'31/12/2100') AS Data_Scadenza_Patentino")
            Else
                StrSQL.AppendLine("     '31/12/2100' AS Data_Scadenza_Patentino  ")
            End If

            StrSQL.AppendLine(" FROM   Rapporti_Contabili ")
            StrSQL.AppendLine(" INNER JOIN Risorse_Umane ON Rapporti_Contabili.Cod_Rapporto = Risorse_Umane.Cod_Rapporto  ")
            StrSQL.AppendLine(" INNER JOIN Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto ")
            StrSQL.AppendLine(" INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.PIVA AND Rapporti_Contabili.Piva = UtentiXImprese.[USER] ")

            'StrSQL.AppendLine(" WHERE  (Contatti.Sa_Cod = - 1 or Risorse_Umane.Piva = '" & Agro_SQL_SaveText(Piva) & "') ")
            StrSQL.AppendLine(" WHERE 1=1 ")

            'StrSQL.AppendLine(" AND ( Prodotti_Costi.Elem_Cod = 0) ")
            If Dipendente Then
                StrSQL.AppendLine(" AND ( Rapporti_Contabili.Dipendente = 1 ) ")
            End If
            If Terzista Then
                StrSQL.AppendLine(" AND (Rapporti_Contabili.Terzista = 1) ")
            End If
            StrSQL.AppendLine(" AND ( Risorse_Umane.Cod_RisUm_Origine = 0 ) ")
            StrSQL.AppendLine(" AND    UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            'pubbliche
            StrSQL.AppendLine(" AND    ( (Contatti.Sa_Cod = -1) ")

            'visibilità centro
            'leggo se ci sono filtri sui centri
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim filtroCentri As String = ""
            Dim dtCentriVisibili As DataTable
            dtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
            If dtCentriVisibili IsNot Nothing AndAlso dtCentriVisibili.Rows.Count > 0 Then
                For i = 0 To dtCentriVisibili.Rows.Count - 1
                    filtroCentri &= " (Contatti.Piva = '" & dtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & dtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                Next
                If filtroCentri <> "" Then
                    StrSQL.AppendLine(" OR (" & Left(filtroCentri, filtroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                End If
            End If

            'aziendali
            If filtroCentri = "" Then
                StrSQL.AppendLine("          OR  (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
            End If

            StrSQL.AppendLine("        ) ")



            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Rag_Soc ")
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

    Public Function RapportiContabilixCostiAccessoribyCod_RisUm_Flag_Data_Scadenza_Patentino(ByVal Piva As String,
                                                                                             ByVal Cod_RisUm As Integer,
                                                                                             ByVal Dipendente As Boolean,
                                                                                             ByVal Terzista As Boolean,
                                                                                             ByVal Data As Date,
                                                                                             ByVal Flag_Data_Scadenza_Patentino As Boolean,
                                                                                             ByVal xFiltroAggiuntivo As String,
                                                                                             ByVal xOrderBy As String,
                                                                                             ByRef objParametri As AgronicaCoreParametri
                                                                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R.RapportiContabilixCostiAccessoribyCod_RisUm_Flag_Data_Scadenza_Patentino()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Rapporti_Contabili.Rapporto_Des, Risorse_Umane.Settore_Des, Rapporti_Contabili.Terzista, Rapporti_Contabili.Dipendente,  ")
            StrSQL.AppendLine("    CASE WHEN (Contatti.Rag_Soc is null or Contatti.Rag_Soc = '') then Contatti.Cognome  + ' ' +  Contatti.Nome       else Contatti.Rag_Soc End as Rag_Soc,   ")
            StrSQL.AppendLine("     Risorse_Umane.Cod_RisUm,  Rapporti_Contabili.Cod_Rapporto, ")
            StrSQL.AppendLine("     Risorse_Umane.Qualifica_Cod,")
            StrSQL.AppendLine("     Contatti.Cod_Contatto, Contatti.Piva,  Contatti.Cognome, Contatti.Nome,")

            If Flag_Data_Scadenza_Patentino Then
                StrSQL.AppendLine("ISNULL((SELECT top 1 Alert_Elenco.Data_Scadenza  ")
                StrSQL.AppendLine("From alert_entita  ")
                StrSQL.AppendLine("INNER JOIN Alert_Elenco ON Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser And Alert_Elenco.ID_Alert_Entita = Alert_Entita.ID_Alert_Entita")
                StrSQL.AppendLine("INNER JOIN Allegati_Documenti ON Allegati_Documenti.Allegati_Documenti_SuperUser = Alert_Entita.PivaSuperUser and Allegati_Documenti.Allegati_Documenti_Piva = Alert_Entita.piva  and")
                StrSQL.AppendLine("Allegati_Documenti.Allegati_Documenti_Cod = Alert_Entita.Allegati_Documenti_Cod")
                StrSQL.AppendLine("WHERE tipoentita_cod=8")
                StrSQL.AppendLine("And Allegati_Documenti.Allegati_documenti_CatCod=2  ")
                StrSQL.AppendLine("And Allegati_Documenti.Validazione_Data <= " & Agro_SQL_SaveDate(Data))
                StrSQL.AppendLine("And Alert_Elenco.Data_Scadenza >=  Alert_Elenco.Data_Scadenza")
                StrSQL.AppendLine("And   alert_entita.Cod_Contatto=Contatti.Cod_Contatto and alert_entita.piva = Contatti.piva")
                StrSQL.AppendLine("order by Alert_Elenco.Data_Scadenza desc),'31/12/2100') AS Data_Scadenza_Patentino")
            Else
                StrSQL.AppendLine("     '31/12/2100' AS Data_Scadenza_Patentino  ")
            End If

            StrSQL.AppendLine(" FROM         Rapporti_Contabili ")
            StrSQL.AppendLine(" INNER JOIN  Risorse_Umane ON Rapporti_Contabili.Cod_Rapporto = Risorse_Umane.Cod_Rapporto  ")
            StrSQL.AppendLine(" INNER JOIN  Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  ")
            StrSQL.AppendLine(" INNER JOIN   UtentiXImprese ON Contatti.Piva = UtentiXImprese.PIVA AND Rapporti_Contabili.Piva = UtentiXImprese.[USER]  ")

            StrSQL.AppendLine(" WHERE  (Contatti.Sa_Cod = - 1 or Risorse_Umane.Piva = '" & Agro_SQL_SaveText(Piva) & "') ")

            If Cod_RisUm <> 0 Then
                StrSQL.AppendLine(" AND Risorse_Umane.Cod_RisUm  = " & Cod_RisUm & " ")
            End If

            If Dipendente Then
                StrSQL.AppendLine(" AND ( Rapporti_Contabili.Dipendente = 1 ) ")
            End If
            If Terzista Then
                StrSQL.AppendLine(" AND (Rapporti_Contabili.Terzista = 1) ")
            End If
            StrSQL.AppendLine(" AND ( Risorse_Umane.Cod_RisUm_Origine = 0 ) ")
            StrSQL.AppendLine(" AND    UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            'pubbliche
            StrSQL.AppendLine(" AND    ( (Contatti.Sa_Cod = -1) ")

            'visibilità centro
            'leggo se ci sono filtri sui centri
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim filtroCentri As String = ""
            Dim dtCentriVisibili As DataTable
            dtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
            If dtCentriVisibili IsNot Nothing AndAlso dtCentriVisibili.Rows.Count > 0 Then
                For i = 0 To dtCentriVisibili.Rows.Count - 1
                    filtroCentri &= " (Contatti.Piva = '" & dtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & dtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                Next
                If filtroCentri <> "" Then
                    StrSQL.AppendLine(" OR (" & Left(filtroCentri, filtroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                End If
            End If

            'aziendali
            If filtroCentri = "" Then
                StrSQL.AppendLine("          OR  (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
            End If

            StrSQL.AppendLine("        ) ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Rapporti_Contabili.Rapporto_Des ")
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

End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Rapporti_Contabili_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Cancella(ByVal Cod_Rapporto As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Cod_Rapporto = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Rapporto obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Rapporti_Contabili ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " ")
                StrSQL.Append(" AND     Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    Rapporti_Contabili ")
                StrSQL.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " ")

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
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
    'La Piva è quella del SuperUser
    Public Function Scrivi(ByVal Sa_Cod As Int32,
                           ByVal Cod_Rapporto As Int32,
                           ByVal Rapporto_Des As String,
                           ByVal Cliente As Int32,
                           ByVal Fornitore As Int32,
                           ByVal Dipendente As Int32,
                           ByVal Terzista As Int32,
                           ByVal Legale As Int32,
                           ByVal Agente As Int32,
                           ByVal Consulente As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO Rapporti_Contabili ")
            StrSQL.Append("             (Piva, Sa_Cod, Cod_Rapporto, Rapporto_Des,  ")
            StrSQL.Append("              Cliente, Fornitore, Dipendente, Terzista, Legale, Agente, Consulente, ")

            StrSQL.Append("              Inviato,            DataInvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine ")

            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("         '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Rapporto) & " ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Rapporto_Des) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cliente) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Fornitore) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Dipendente) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Terzista) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Legale) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Agente) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Consulente) & " ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
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
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' La Piva è quella del SuperUser
    ''' </summary>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Cod_Rapporto"></param>
    ''' <param name="Rapporto_Des"></param>
    ''' <param name="Cliente"></param>
    ''' <param name="Fornitore"></param>
    ''' <param name="Dipendente"></param>
    ''' <param name="Terzista"></param>
    ''' <param name="Legale"></param>
    ''' <param name="Agente"></param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	15/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Modifica(ByVal Sa_Cod As Int32,
                             ByVal Cod_Rapporto As Int32,
                             ByVal Rapporto_Des As String,
                             ByVal Cliente As Int32,
                             ByVal Fornitore As Int32,
                             ByVal Dipendente As Int32,
                             ByVal Terzista As Int32,
                             ByVal Legale As Int32,
                             ByVal Agente As Int32,
                             ByVal Consulente As Int32,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            
            StrSQL.Length = 0

            StrSQL.Append("UPDATE Rapporti_Contabili SET ")
            StrSQL.Append("    Sa_Cod           =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("   ,Rapporto_Des     = '" & Agro_SQL_SaveText(Rapporto_Des) & "'")
            StrSQL.Append("   ,Cliente          =  " & Agro_SQL_SaveNum(Cliente) & "  ")
            StrSQL.Append("   ,Fornitore        =  " & Agro_SQL_SaveNum(Fornitore) & "  ")
            StrSQL.Append("   ,Dipendente       =  " & Agro_SQL_SaveNum(Dipendente) & "  ")
            StrSQL.Append("   ,Terzista         =  " & Agro_SQL_SaveNum(Terzista) & "  ")
            StrSQL.Append("   ,Legale           =  " & Agro_SQL_SaveNum(Legale) & "  ")
            StrSQL.Append("   ,Agente           =  " & Agro_SQL_SaveNum(Agente) & "  ")
            StrSQL.Append("   ,Consulente       =  " & Agro_SQL_SaveNum(Consulente) & "  ")

            StrSQL.Append("   ,Inviato           = 0 ")
            StrSQL.Append("   ,DataInvio         = Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE  Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
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
    Public Sub RapportiContabiliDiBase_Inserisci(ByRef Errore As String,
                                                 ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabiliDiBase_Inserisci()"

        RapportiContabili_Legale_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_Cliente_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_Fornitore_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_Dipendente_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_Terzista_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_Tecnico_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_CentroMacchine_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_LaboratorioAnalisi_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_Socio_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_Trasportatore_Inserisci(Errore, objParametri)

        '==========================================================================

        'DA ABILITARE QUANDO CI SARA' IL CONFERIMENTO/ACCETTAZIONE

        'RapportiContabili_ClienteFornitore_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_TecnicoResponsabile_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_Agente_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_ReferenteAziendale_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_Consulente_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_Spedizioniere_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_Vivaio_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_Conferente_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_Operatore_Lab_CQ_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_CapoArea_Inserisci(Errore, objParametri)

        '==========================================================================

        RapportiContabili_Fornitore_Ortofrutta_Inserisci(Errore, objParametri)

        '==========================================================================

    End Sub



    '##############################################################################################
    Public Sub RapportiContabili_Legale_Inserisci(ByRef Errore As String, ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_Legale_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.PersoneFisiche,
                      CInt(COD_LEGALE),
                      CStr("Legale Rappresentante"),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Legale Rappresentante. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_Cliente_Inserisci(ByRef Errore As String, ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_Cliente_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.Tutti,
                      CInt(COD_CLIENTE),
                      CStr("Cliente"),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Cliente. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_Fornitore_Inserisci(ByRef Errore As String, ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_Fornitore_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.Tutti,
                      CInt(COD_FORNITORE),
                      CStr("Fornitore"),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Fornitore. Contattare l'amministratore." & vbCrLf
        End If

    End Sub


    '##############################################################################################
    Public Sub RapportiContabili_Dipendente_Inserisci(ByRef Errore As String, ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_Dipendente_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.Tutti,
                      CInt(COD_DIPENDENTE),
                      CStr("Dipendente"),
                      CInt(0),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Dipendente. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_Terzista_Inserisci(ByRef Errore As String, ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_Terzista_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.Tutti,
                      CInt(COD_TERZISTA),
                      CStr("Terzista"),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Terzista. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_Tecnico_Inserisci(ByRef Errore As String, ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_Tecnico_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.PersoneFisiche,
                      CInt(COD_TECNICO),
                      CStr("Tecnico"),
                      CInt(0),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Tecnico. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_CentroMacchine_Inserisci(ByRef Errore As String,
                                                          ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_CentroMacchine_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.PersoneGiuridiche,
                      CInt(COD_CENTRO_MACCHINE),
                      CStr("Centro Revisione e Manutenzione Macchine"),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Centro Revisione e Manutenzione Macchine. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_LaboratorioAnalisi_Inserisci(ByRef Errore As String,
                                                              ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_LaboratorioAnalisi_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.PersoneGiuridiche,
                      CInt(COD_LAB_ANALISI),
                      CStr("Laboratorio Analisi"),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Laboratorio Analisi. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_Socio_Inserisci(ByRef Errore As String,
                                                 ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_Socio_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.PersoneFisiche,
                      CInt(COD_SOCIO),
                      CStr("Socio"),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Socio. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_Trasportatore_Inserisci(ByRef Errore As String,
                                                         ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_Trasportatore_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.Tutti,
                      CInt(COD_TRASPORTATORE),
                      CStr("Trasportatore"),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Trasportatore. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_ClienteFornitore_Inserisci(ByRef Errore As String,
                                                            ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_ClienteFornitore_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(CInt(0),
                      CInt(COD_CLIENTE_FORNITORE_BolleAccett),
                      CStr("Cliente-Fornitore"),
                      CInt(1),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Cliente-Fornitore. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_TecnicoResponsabile_Inserisci(ByRef Errore As String,
                                                               ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_TecnicoResponsabile_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.PersoneFisiche,
                      CInt(COD_TECNICORESPONSABILE),
                      CStr("Tecnico Responsabile"),
                      CInt(0),
                      CInt(0),
                      CInt(1),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Tecnico Responsabile. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_Agente_Inserisci(ByRef Errore As String,
                                                  ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_Agente_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.Tutti,
                      CInt(COD_AGENTE),
                      CStr("Agente"),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Agente. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_ReferenteAziendale_Inserisci(ByRef Errore As String,
                                                              ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_ReferenteAziendale_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.PersoneFisiche,
                      CInt(COD_REFERENTEAZIENDALE),
                      CStr("Referente Aziendale"),
                      CInt(0),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Referente Aziendale. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_Consulente_Inserisci(ByRef Errore As String,
                                                      ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_Agente_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.Tutti,
                      CInt(COD_CONSULENTE),
                      CStr("Consulente"),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(1),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Consulente. Contattare l'amministratore." & vbCrLf
        End If

    End Sub


    '##############################################################################################
    Public Sub RapportiContabili_Spedizioniere_Inserisci(ByRef Errore As String,
                                                         ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_Spedizioniere_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.PersoneGiuridiche,
                      CInt(COD_SPEDIZIONIERE),
                      CStr("Spedizioniere"),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Spedizioniere. Contattare l'amministratore." & vbCrLf
        End If

    End Sub


    '##############################################################################################
    Public Sub RapportiContabili_Vivaio_Inserisci(ByRef Errore As String, ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_Vivaio_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.PersoneGiuridiche,
                      CInt(COD_VIVAIO),
                      CStr("Vivaio"),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Vivaio. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_Conferente_Inserisci(ByRef Errore As String,
                                                      ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_Conferente_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.PersoneGiuridiche,
                      CInt(COD_CONFERENTE),
                      CStr("Conferente"),
                      CInt(1),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Conferente. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_Operatore_Lab_CQ_Inserisci(ByRef Errore As String,
                                                            ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_Operatore_Lab_CQ_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.PersoneFisiche,
                      CInt(COD_LAB_CQ),
                      CStr("Operatore Lab Controllo Qualità"),
                      CInt(0),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Operatore Lab Controllo Qualità. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_CapoArea_Inserisci(ByRef Errore As String,
                                                    ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_CapoArea_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.Tutti,
                      CInt(COD_CAPO_AREA),
                      CStr("Capo Area"),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Capo Area. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

    '##############################################################################################
    Public Sub RapportiContabili_Fornitore_Ortofrutta_Inserisci(ByRef Errore As String,
                                                                ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W.RapportiContabili_Operatore_Lab_CQ_Inserisci()"
        Dim risp As Boolean

        risp = Scrivi(enum_RapportiContabili_SaCod.PersoneFisiche,
                      CInt(COD_LAB_CQ),
                      CStr("Fornitore Ortofrutta"),
                      CInt(0),
                      CInt(1),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      CInt(0),
                      AGRODATAINIZIO, AGRODATAFINE,
                      objParametri)

        If Not risp Then
            Errore &= "Impossibile inserire il rapporto contabile Fornitore Ortofrutta. Contattare l'amministratore." & vbCrLf
        End If

    End Sub

End Class
