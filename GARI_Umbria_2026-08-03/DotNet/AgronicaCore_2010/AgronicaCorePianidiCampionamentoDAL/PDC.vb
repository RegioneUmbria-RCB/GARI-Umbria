Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.My.Resources

Public Class PDC_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Shared Function GetStato(ByVal PDC_Stato As Integer,
                                    Optional Da_Zoo As Boolean = False
                                    ) As String

        Select Case PDC_Stato
            Case enum_PDC_Stato_Testata.Nessun_Impianto_Selezionato
                Return If(Da_Zoo, Gias.NessunCapoSelezionato, Gias.NessunImpiantoSelezionato)
            Case enum_PDC_Stato_Testata.Impianti_Selezionati
                Return If(Da_Zoo, Gias.CapiSelezionati, Gias.ImpiantiSelezionati)
            Case enum_PDC_Stato_Testata.Lotti_Fitosanitari_Selezionati
                Return Gias.LottiFitosanitariSel
            Case enum_PDC_Stato_Testata.Impianti_Da_Campionare_Selezionati
                Return If(Da_Zoo, Gias.CapiDaCampionareSelezionati, Gias.ImpiantiDaCampionareSelezionati)
            Case enum_PDC_Stato_Testata.Impianti_Da_Campionare_E_Lotti_Selezionati
                Return Gias.ImpiantiDaCampionareLottiSelezionati
            Case enum_PDC_Stato_Testata.Workflow_Analisi
                Return Gias.WorkFlowAnalisi
            Case enum_PDC_Stato_Testata.Richiesta_Analisi_Zoo_Inviata
                Return Gias.RichiestaAnalisiInviata
            Case enum_PDC_Stato_Testata.Analizzato
                Return Gias.Analizzato
            Case Else
                Return ""
        End Select

    End Function

    Public Shared Function GetStatoCampione(ByVal PDC_Stato As Integer) As String

        Select Case PDC_Stato
            Case enum_PDC_Stato_Campione.Da_Campionare
                Return Gias.AncoraDaCampionare
            Case enum_PDC_Stato_Campione.Campione_Eseguito
                Return Gias.CampioneEseguito
            Case enum_PDC_Stato_Campione.Campione_Inviato_A_Laboratorio
                Return Gias.CampioneInviatoLaboratorio
            Case enum_PDC_Stato_Campione.Analisi_Campione_Eseguita
                Return Gias.AnalisiEseguita
            Case Else
                Return ""
        End Select

    End Function

    Public Function Leggi(ByVal ID_PDC_Testata As Integer,
                          ByVal Testata_Des As String,
                          ByVal PDC_Stato As Integer,
                          ByVal Da_Campagna As Integer,
                          ByVal CodiceStabilimento As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCorePianidiCampionamentoDAL.PDC_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.AppendLine(" SELECT ID_PDC_Testata , PDC_Testata_Des ")
                    StrSQL.AppendLine(" FROM  PDC_Testata")
                    StrSQL.AppendLine(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    If ID_PDC_Testata <> 0 Then
                        StrSQL.AppendLine(" AND Id_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
                    End If

                    If Testata_Des <> "" Then
                        StrSQL.AppendLine(" AND PDC_Testata_Des = '" & Agro_SQL_SaveText(Testata_Des) & "'")
                    End If

                    If PDC_Stato <> 0 Then
                        StrSQL.AppendLine(" AND PDC_Stato = " & Agro_SQL_SaveNum(PDC_Stato))
                    End If

                    If Da_Campagna = -99 Then
                    Else
                        If Da_Campagna = 0 Then
                            StrSQL.AppendLine(" AND (Da_Campagna = 0 Or Da_Campagna Is null) ")
                        End If
                        If Da_Campagna = 1 Then
                            StrSQL.AppendLine(" AND Da_Campagna = 1 ")
                        End If
                        If Da_Campagna = 2 Then
                            StrSQL.AppendLine(" AND Da_Campagna = 2 ")
                        End If
                    End If


                    If CodiceStabilimento <> "" Then
                        StrSQL.AppendLine(" AND CodiceStabilimento = '" & Agro_SQL_SaveText(CodiceStabilimento) & "'")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Data_Creazione, PDC_Testata_Des ASC")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.AppendLine(" SELECT PivaSuperUser , Id_PDC_Testata , PDC_Testata_Des, ")
                    StrSQL.AppendLine(" PDC_Data_Istantanea, Data_Creazione , Data_Modifica , Username_Creazione, ")
                    StrSQL.AppendLine(" Username_Modifica, convert(VARCHAR(10), Validita_Inizio, 103) as Validita_Inizio, Validita_Inizio as v_i, Validita_Fine as v_f, convert(VARCHAR(10), Validita_Fine, 103)  as Validita_Fine, PDC_Stato, Da_Campagna , CodiceStabilimento  ")

                    StrSQL.AppendLine(" , ISNULL(PivaOwner,'') As PivaOwner, Sa_CodOwner, Fabbricato_CodOwner, Da_Zoo  ")

                    StrSQL.AppendLine(" FROM  PDC_Testata")
                    StrSQL.AppendLine(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    If ID_PDC_Testata <> 0 Then
                        StrSQL.AppendLine(" AND Id_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
                    End If

                    If Testata_Des <> "" Then
                        StrSQL.AppendLine(" AND PDC_Testata_Des = '" & Agro_SQL_SaveText(Testata_Des) & "'")
                    End If

                    If PDC_Stato <> 0 Then
                        StrSQL.AppendLine(" AND PDC_Stato = " & Agro_SQL_SaveNum(PDC_Stato))
                    End If

                    If Da_Campagna = -99 Then
                    Else
                        If Da_Campagna = 0 Then
                            StrSQL.AppendLine(" AND (Da_Campagna = 0 Or Da_Campagna Is null) ")
                        End If
                        If Da_Campagna = 1 Then
                            StrSQL.AppendLine(" AND Da_Campagna = 1 ")
                        End If
                        If Da_Campagna = 2 Then
                            StrSQL.AppendLine(" AND Da_Campagna = 2 ")
                        End If
                    End If

                    If CodiceStabilimento <> "" Then
                        StrSQL.AppendLine(" AND CodiceStabilimento = '" & Agro_SQL_SaveText(CodiceStabilimento) & "'")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Data_Creazione desc, PDC_Testata_Des DESC")
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

    Public Function Leggi2(ByVal ID_PDC_Testata As Integer,
                           ByVal Testata_Des As String,
                           ByVal PDC_Stato As Integer,
                           ByVal Da_Campagna As Integer,
                           ByVal CodiceStabilimento As String,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional Da_Zoo As Boolean = False
                           ) As DataTable

        Const nomeRoutine = "AgronicaCorePianidiCampionamentoDAL.PDC_R.Leggi2()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT PivaSuperUser , Id_PDC_Testata , PDC_Testata_Des, ")
            StrSQL.AppendLine(" PDC_Data_Istantanea, PDC_Testata.Data_Creazione , PDC_Testata.Data_Modifica , PDC_Testata.Username_Creazione, PDC_Testata.Username_Modifica, ")
            StrSQL.AppendLine(" convert(VARCHAR(10), PDC_Testata.Validita_Inizio, 103) as Validita_Inizio, PDC_Testata.Validita_Inizio as v_i, ")
            StrSQL.AppendLine(" convert(VARCHAR(10), PDC_Testata.Validita_Fine, 103) as Validita_Fine, PDC_Testata.Validita_Fine as v_f, ")
            StrSQL.AppendLine(" PDC_Stato, Da_Campagna, CodiceStabilimento, PivaOwner, Imprese.Rag_Soc As ImpresaOwner ")

            If Da_Zoo Then
                StrSQL.AppendLine("  ,  PDC_Testata.Sa_CodOwner, Centri_Aziendali.sa_nome as CentroAziendaleOwner")
                StrSQL.AppendLine("  ,  PDC_Testata.Fabbricato_CodOwner, Fabbricati.Fabbricato_Des as StallaOwner")
            End If

            StrSQL.AppendLine(" FROM  PDC_Testata")
            StrSQL.AppendLine(" LEFT JOIN Imprese ON PDC_Testata.PivaOwner = Imprese.Piva ")

            If Da_Zoo Then
                StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali ON PDC_Testata.PivaOwner = Centri_Aziendali.Piva AND PDC_Testata.Sa_CodOwner = Centri_Aziendali.sa_cod")
                StrSQL.AppendLine(" LEFT JOIN Fabbricati ON PDC_Testata.PivaOwner = Fabbricati.Piva AND PDC_Testata.Sa_CodOwner = Fabbricati.sa_cod AND PDC_Testata.Fabbricato_CodOwner = Fabbricati.Fabbricato_Cod")
            End If

            StrSQL.AppendLine(" WHERE PDC_Testata.Validita_inizio <=  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" And   PDC_Testata.Validita_Fine >=  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" And   PDC_Testata.PivaSuperUser  =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND Id_PDC_Testata  =  " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If Testata_Des <> "" Then
                StrSQL.AppendLine(" AND PDC_Testata_Des  =  '" & Agro_SQL_SaveText(Testata_Des) & "'")
            End If

            If PDC_Stato <> 0 Then
                StrSQL.AppendLine(" AND PDC_Stato  =  " & Agro_SQL_SaveNum(PDC_Stato))
            End If


            If Da_Campagna = -99 Then
            Else
                If Da_Campagna = 0 Then
                    StrSQL.AppendLine(" AND (Da_Campagna  =  0 Or Da_Campagna Is null) ")
                End If
                If Da_Campagna = 1 Then
                    StrSQL.AppendLine(" AND Da_Campagna  =  1 ")
                End If
                If Da_Campagna = 2 Then
                    StrSQL.AppendLine(" AND Da_Campagna  =  2 ")
                End If
            End If

            If CodiceStabilimento <> "" Then
                StrSQL.AppendLine(" AND CodiceStabilimento  =  '" & Agro_SQL_SaveText(CodiceStabilimento) & "'")
            End If

            '----------------------------------------------------------------
            '--- Filtro associato all'utente 
            '----------------------------------------------------------------
            Dim UtenteFiltro As String = ""
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)

            If DtImpreseVisibili IsNot Nothing Then
                For i = 0 To DtImpreseVisibili.Rows.Count - 1
                    UtenteFiltro &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                Next
                If UtenteFiltro <> "" Then
                    ' aggiungo pivasuperuser allora per visibilità globale
                    UtenteFiltro &= "'" & objParametri.PivaSuperUser & "',"
                    StrSQL.AppendLine(" AND (ISNULL(PivaOwner,'')='' OR Imprese.piva IN (" & Agro_SQL_Save_Clausola_IN(Left(UtenteFiltro, UtenteFiltro.Length - 1), True) & ")) ")
                End If
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY PDC_Testata.Data_Creazione desc, PDC_Testata_Des DESC")
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

    Public Function Leggi_PDC_Zoo(ByVal Piva As String,
                                  ByVal Sa_Cod As Integer,
                                  ByVal Fabbricato_Cod As Integer,
                                  ByVal PDC_Stato As Integer,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As DataTable

        Const nomeRoutine = "AgronicaCorePianidiCampionamentoDAL.PDC_R.Leggi_PDC_Zoo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            'StrSQL.AppendLine(" SELECT PDC_Testata.*, Imprese.Rag_Soc As ImpresaOwner ")
            'StrSQL.AppendLine(" FROM  PDC_Testata LEFT JOIN Imprese ON PDC_Testata.PivaOwner=Imprese.Piva ")
            StrSQL.AppendLine(" SELECT * FROM PDC_Testata ")
            StrSQL.AppendLine(" WHERE PDC_Testata.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND PDC_Testata.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND PDC_Testata.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND PivaOwner = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sa_CodOwner = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.AppendLine(" AND Fabbricato_CodOwner = " & Agro_SQL_SaveNum(Fabbricato_Cod))
            End If

            If PDC_Stato <> -1 Then
                StrSQL.AppendLine(" AND PDC_Stato = " & Agro_SQL_SaveNum(PDC_Stato))
            End If

            StrSQL.AppendLine(" AND Da_Zoo = 1 ")

            '----------------------------------------------------------------
            '--- Filtro associato all'utente 
            '----------------------------------------------------------------
            Dim UtenteFiltro As String = ""
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)

            If DtImpreseVisibili IsNot Nothing Then
                For i = 0 To DtImpreseVisibili.Rows.Count - 1
                    UtenteFiltro &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                Next
                If UtenteFiltro <> "" Then
                    ' aggiungo pivasuperuser allora per visibilità globale
                    UtenteFiltro &= "'" & objParametri.PivaSuperUser & "',"
                    StrSQL.AppendLine(" AND (PivaOwner = '' OR PivaOwner IN (" & Agro_SQL_Save_Clausola_IN(Left(UtenteFiltro, UtenteFiltro.Length - 1), True) & ")) ")
                End If
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY PDC_Testata.Data_Creazione desc, PDC_Testata_Des DESC")
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

    Public Function Leggi_PDC_ZooApp(ByVal Piva As String,
                                  ByVal Sa_Cod As Integer,
                                  ByVal Fabbricato_Cod As Integer,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As DataTable

        Const nomeRoutine = "AgronicaCorePianidiCampionamentoDAL.PDC_R.Leggi_PDC_Zoo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            'StrSQL.AppendLine(" SELECT PDC_Testata.*, Imprese.Rag_Soc As ImpresaOwner ")
            'StrSQL.AppendLine(" FROM  PDC_Testata LEFT JOIN Imprese ON PDC_Testata.PivaOwner=Imprese.Piva ")
            StrSQL.AppendLine(" SELECT * FROM PDC_Testata ")
            StrSQL.AppendLine(" WHERE PDC_Testata.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND PDC_Testata.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND PDC_Testata.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND PivaOwner = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sa_CodOwner = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.AppendLine(" AND Fabbricato_CodOwner = " & Agro_SQL_SaveNum(Fabbricato_Cod))
            End If

            StrSQL.AppendLine(" AND PDC_Stato IN " & Agro_SQL_Save_Clausola_IN("(0, 1, 3)"))

            StrSQL.AppendLine(" AND Da_Zoo = 1 ")

            '----------------------------------------------------------------
            '--- Filtro associato all'utente 
            '----------------------------------------------------------------
            Dim UtenteFiltro As String = ""
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)

            If DtImpreseVisibili IsNot Nothing Then
                For i = 0 To DtImpreseVisibili.Rows.Count - 1
                    UtenteFiltro &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                Next
                If UtenteFiltro <> "" Then
                    ' aggiungo pivasuperuser allora per visibilità globale
                    UtenteFiltro &= "'" & objParametri.PivaSuperUser & "',"
                    StrSQL.AppendLine(" AND (PivaOwner = '' OR PivaOwner IN (" & Agro_SQL_Save_Clausola_IN(Left(UtenteFiltro, UtenteFiltro.Length - 1), True) & ")) ")
                End If
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY PDC_Testata.Data_Creazione desc, PDC_Testata_Des DESC")
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



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class PDC_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal ID_PDC_Testata As Integer,
                           ByVal PDC_Testata_Des As String,
                           ByVal PDC_Data_Istantanea As Date,
                           ByVal PDC_Stato As Integer,
                           ByVal Da_Campagna As Integer,
                           ByVal CodiceStabilimento As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal PivaOwner As String = "",
                           Optional ByVal Sa_CodOwner As Integer = 0,
                           Optional ByVal Fabbricato_CodOwner As Integer = 0,
                           Optional ByVal Da_Zoo As Integer = 0
                           ) As Boolean

        Const nomeRoutine = "AgronicaCorePianidiCampionamentoDAL.PDC_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False


        If Data_creazione = #2/1/1900# Then
            Data_creazione = Date.Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Date.Now
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
            StrSQL.AppendLine("INSERT INTO PDC_Testata(PivaSuperUser , ID_PDC_Testata, PDC_Testata_Des, ")
            StrSQL.AppendLine("                                 PDC_Data_Istantanea, Data_Creazione, Data_Modifica, ")
            StrSQL.AppendLine("                                 Username_Creazione, Username_Modifica, ")
            StrSQL.AppendLine("                                 Validita_Inizio, Validita_Fine, ")
            StrSQL.AppendLine("                                 PDC_Stato, Da_Campagna, CodiceStabilimento")

            StrSQL.AppendLine(", PivaOwner")
            StrSQL.AppendLine(", Sa_CodOwner")
            StrSQL.AppendLine(", Fabbricato_CodOwner")
            StrSQL.AppendLine(", Da_Zoo")

            StrSQL.AppendLine(")   VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Testata))
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(PDC_Testata_Des) & "'")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveDate(PDC_Data_Istantanea) & " ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveDateTime(Data_creazione) & " ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveDateTime(Data_modifica) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(PDC_Stato) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Da_Campagna) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(CodiceStabilimento) & "' ")

            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(PivaOwner) & "' ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(Sa_CodOwner) & " ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(Fabbricato_CodOwner) & " ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(Da_Zoo) & " ")

            StrSQL.AppendLine(")")

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

#Region "Modifica"

    Public Function Modifica(ByVal ID_PDC_Testata As Integer,
                             ByVal PDC_Testata_Des As String,
                             ByVal PDC_Data_Istantanea As Date,
                             ByVal PDC_Stato As Integer,
                             ByVal CodiceStabilimento As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal PivaOwner As String = "",
                             Optional ByVal Sa_CodOwner As Integer = 0,
                             Optional ByVal Fabbricato_CodOwner As Integer = 0,
                             Optional ByVal Da_Zoo As Integer = 0
                             ) As Boolean

        Const nomeRoutine = "AgronicaPianidiCampionamentoDAL.PDC_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Testata SET ")
            StrSQL.AppendLine("   PDC_Testata_Des         =  '" & Agro_SQL_SaveText(PDC_Testata_Des) & "'   ")
            StrSQL.AppendLine("   ,PDC_Data_Istantanea  =  " & Agro_SQL_SaveDate(PDC_Data_Istantanea) & "   ")
            StrSQL.AppendLine("   ,PDC_Stato =  " & Agro_SQL_SaveNum(PDC_Stato) & "   ")
            StrSQL.AppendLine("   ,CodiceStabilimento = '" & Agro_SQL_SaveText(CodiceStabilimento) & "'   ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine("   ,PivaOwner = '" & Agro_SQL_SaveText(PivaOwner) & "'   ")
            StrSQL.AppendLine("   ,Sa_CodOwner = " & Agro_SQL_SaveNum(Sa_CodOwner) & "  ")
            StrSQL.AppendLine("   ,Fabbricato_CodOwner = " & Agro_SQL_SaveNum(Fabbricato_CodOwner) & "  ")
            StrSQL.AppendLine("   ,Da_Zoo = " & Agro_SQL_SaveNum(Da_Zoo) & "  ")

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")

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

    Public Function Aggiorna_Stato(ByVal ID_PDC_Testata As Integer,
                                   ByVal PDC_Stato As Integer,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Const nomeRoutine = "AgronicaPianidiCampionamentoDAL.PDC_W.Aggiorna_Stato()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Testata SET ")
            StrSQL.AppendLine("   PDC_Stato =  " & Agro_SQL_SaveNum(PDC_Stato) & "   ")

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")

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
#End Region

#Region "Cancellazione"

    Public Function Cancella(ByVal ID_PDC_Testata As Integer, ByRef objParametri As AgronicaCoreParametri) As Boolean

        Const nomeRoutine = "AgronicaCorePianidiCampionamentoDAL.PDC_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If ID_PDC_Testata = 0 Then
                Return False
            End If
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE FROM PDC_Testata ")

            StrSQL.AppendLine("  WHERE ")
            StrSQL.AppendLine("         PivaSuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         AND ID_PDC_Testata =  " & Agro_SQL_SaveNum(ID_PDC_Testata))

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

#End Region

End Class
 