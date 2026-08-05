Imports System.Text
Imports System.Text.RegularExpressions
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreVisibilitaStd

Public Class Utenti_Visibilita

#Region "Lettura"

    Public Function LeggiAziendeVisibilita(utenti As List(Of UtenteDTO),
                                           imprese As List(Of ImpresaDto),
                                           objParametri_Server As AgronicaCoreParametri,
                                           objParametri_Utenti As AgronicaCoreParametri) As List(Of ImpresexUtentiVisibilita)

        Dim objGU As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
        Dim normalizeString = Function(s) s.ToString().Trim()

        Dim usernames As New List(Of String)
        If utenti.Any() Then
            usernames = utenti.Select(Function(u) u.UserName)
        End If
        Dim usersHash As New Dictionary(Of String, (nome as String, gruppo as String, qualifica as string))
        objGU.LeggiUtenti(UserName:="", Gruppi_Utente_cod:=0, xFiltroAggiuntivo:="", xOrderBy:="", ObjParametri_Utenti).
            Select.groupby(Function(r) r("UserName")).
            select(Of (username as String, nome as String, gruppo as String, qualifica as string))(Function(x) (
                x.first().Item("UserName"),
                x.first().Item("Dettagli"),
                x.Select(function(a) a.Item("Gruppi_Utente_Des").ToString).Aggregate(Function(a,b) a & ", " & b),
                x.first().Item("Qualifica")
            )).
            ToList.
            ForEach(Sub(r) usersHash.Add(
                normalizeString(r.username).ToLowerInvariant,
                (normalizeString(r.nome), normalizeString(r.gruppo), normalizeString(r.qualifica))
            ))

        If Not imprese.Any() Then
            'Limito la visibilità alle imprese visibili all'utente attuale
            imprese = LeggiPiveCapostipiti(objParametri_Utenti.UsernameOperazione, objParametri_Utenti).
                Select(Function(p) New ImpresaDto With {.piva = p}).ToList()
        End If

        Return LeggiAppoggioXImprese(utenti, imprese, objParametri_Server).
            Select.AsParallel.
            Select(Function(i) New ImpresexUtentiVisibilita With {
                .Username = i.Item("Username"),
                .DettagliUtente = usersHash(normalizeString(i.Item("Username")).ToLowerInvariant).nome,
                .Gruppo_Des = usersHash(normalizeString(i.Item("Username")).ToLowerInvariant).gruppo,
                .piva = i.Item("Piva"),
                .Cuaa = i.Item("cuaa"),
                .rag_soc = i.Item("rag_soc"),
                .Sa_Cod = i.Item("Sa_Cod"),
                .Sa_Nome = i.Item("Sa_Nome")
            }).ToList()
    End Function

    Public Function LeggiVisibilitaUtenti(
        utenti As List(Of UtenteDTO),
        objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri
    ) As List(Of ImpresexUtentiVisibilita)
        Dim restringiVisibilita As New List(Of ImpresaDto)
        Return LeggiVisibilitaUtenti(utenti, restringiVisibilita, objParametri_Server, objParametri_Utenti)
    End Function

    ''' <summary>
    ''' Carica la visibilità degli utenti specificati, tenendo in considerazione
    ''' la visibilità dell'utente che ha richiesto l'operazione.
    ''' </summary>
    ''' <param name="utenti">Utenti di cui si vuole conoscere la visibilità</param>
    ''' <param name="restringiVisibilita">Imprese da considerare nella lettura della visibilità.
    ''' Funge da filtro sulle aziende che si andranno a leggere, col risultato di mostrare la
    ''' visibilità degli utenti solo sul sottoinsieme di imprese indicato. Passare una lista
    ''' vuota per evitare di filtrare le aziende.</param>
    ''' <param name="leggiDettagli">Se impostato a True, carica i dettagli degli utenti (nome e cognome, gruppo)</param>
    ''' <returns>Una lista di ImpresexUtentiVisibilita con le visibilità degli utenti specificati</returns>
    Public Function LeggiVisibilitaUtenti(
        utenti As List(Of UtenteDTO), restringiVisibilita As List(Of ImpresaDto),
        objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri,
        Optional leggiDettagli As Boolean = False
    ) As List(Of ImpresexUtentiVisibilita)
        Const VISIBILITY_BATCH_SIZE As Integer = 1_000

        Dim listaVisibilita As New List(Of ImpresexUtentiVisibilita)
        Dim usernamesToProcess = utenti.Select(Function(u) u.UserName.ToLower).Distinct.ToList()

        ' Single appoggio read for the current operator — identical to original
        Dim currUserVisibility = LeggiVisibilitaUtente(
            objParametri_Utenti.UsernameOperazione,
            restringiVisibilita.Select(Function(i) i.piva).ToList(),
            objParametri_Utenti,
            objParametri_Server
        )
        Dim currUserPive = currUserVisibility.Select(Function(i) i.piva).ToHashSet(StringComparer.OrdinalIgnoreCase)
        Dim hasCurrFilter = currUserPive.Any()

        ' Read site config flags once — avoids repeated DB round-trips inside the batch loop
        Dim configs = LeggiVizConfigFlags(objParametri_Server)

        'Carico i dati degli utenti -- usata per ricerca visibilità
        Dim dettagli = New Dictionary(Of String, (nome As String, gruppo As String, qualifica As String))(StringComparer.OrdinalIgnoreCase)
        If leggiDettagli Then        
            Dim objGU As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
            Dim normalize = Function(s) s.ToString.Trim
            objGU.LeggiUtenti(UserName:="", Gruppi_Utente_cod:=0, xFiltroAggiuntivo:="", xOrderBy:="", ObjParametri_Utenti).
                Select.groupby(Function(r) r("UserName")).
                select(Of (username As String, nome As String, gruppo As String, qualifica As String))(Function(x) (
                    x.first().Item("UserName"),
                    x.first().Item("Dettagli"),
                    x.Select(Function(a) a.Item("Gruppi_Utente_Des").ToString).Aggregate(Function(a, b) a & ", " & b),
                    x.first().Item("Qualifica")
                )).
                ToList.
                ForEach(Sub(r)
                    Dim key = normalize(r.username).ToLowerInvariant
                    If Not dettagli.ContainsKey(key) Then
                        dettagli.Add(key, (normalize(r.nome), normalize(r.gruppo), normalize(r.qualifica)))
                    End If
                End Sub)
        End If

        Dim profileServiceFilter = " Utenti_Profili.Id_Servizio = " & CInt(enum_Id_Servizio.GiasOnline)
        Dim profiloR As New AgronicaCoreUtentiDAL.Utenti_Profili_Read

        ' Process in bounded batches — keeps IN-clause sizes manageable and limits memory pressure
        For Each batch In StringsInBatches(usernamesToProcess, VISIBILITY_BATCH_SIZE)
            Dim batchList = batch.ToList()

            ' Batch profile read: one query per batch instead of N per-user reads
            Dim profileByUser = profiloR.LeggiMassivo(batchList, profileServiceFilter, String.Empty, objParametri_Utenti).
                AsEnumerable().
                GroupBy(Function(dr) If(IsDBNull(dr("Utente")), "", CStr(dr("Utente"))).ToLower()).
                ToDictionary(Function(g) g.Key, Function(g) g.First())

            Dim freshUsers As New List(Of String)
            Dim staleUsers As New List(Of String)
            For Each username In batchList
                If Not profileByUser.ContainsKey(username) OrElse
                   Not IsProfiloStale(profileByUser(username), configs) Then
                    freshUsers.Add(username)
                Else
                    staleUsers.Add(username)
                End If
            Next

            ' ---- Fresh path: single batch appoggio read for all fresh users ----
            If freshUsers.Any() Then
                Dim freshDTOs = freshUsers.Select(Function(u) New UtenteDTO With {.UserName = u}).ToList()
                Dim freshResult = LeggiAppoggioXImprese(freshDTOs, restringiVisibilita, objParametri_Server)
                If freshResult IsNot Nothing Then
                    For Each row As DataRow In freshResult.Rows
                        Dim uname = If(IsDBNull(row("Username")), "", CStr(row("Username"))).ToLower()
                        Dim piva = If(IsDBNull(row("PIVA")), "", CStr(row("PIVA"))).Trim()
                        If hasCurrFilter AndAlso Not currUserPive.Contains(piva) Then Continue For
                        Dim nomeUtente = If(dettagli.ContainsKey(uname), dettagli(uname).nome, String.Empty)
                        Dim gruppoUtente = If(dettagli.ContainsKey(uname), dettagli(uname).gruppo, String.Empty)
                        listaVisibilita.Add(New ImpresexUtentiVisibilita With {
                            .Username = uname,
                            .DettagliUtente = nomeUtente,
                            .Gruppo_Des = gruppoUtente,
                            .piva = piva,
                            .Cuaa = If(IsDBNull(row("cuaa")), "", CStr(row("cuaa"))),
                            .rag_soc = If(IsDBNull(row("rag_soc")), "", CStr(row("rag_soc"))),
                            .Sa_Cod = If(IsDBNull(row("sa_cod")), 0, CInt(row("sa_cod"))),
                            .partitaIvaReale = If(IsDBNull(row("partitaIvaReale")), piva, CStr(row("partitaIvaReale"))),
                            .Sa_Nome = If(IsDBNull(row("sa_nome")), "", CStr(row("sa_nome")))
                        })
                    Next
                End If
            End If

            ' ---- Stale path: on-the-fly combined compute + batch enrichment ----
            If staleUsers.Any() Then
                Dim bizUtenti As New Utenti

                ' Compute combined visibility per stale user (profile changed or procedures active on legacy path)
                Dim staleUserPiveMap As New Dictionary(Of String, HashSet(Of String))(StringComparer.OrdinalIgnoreCase)
                Dim staleUserCentriMap As New Dictionary(Of String, List(Of Tuple(Of String, Integer)))(StringComparer.OrdinalIgnoreCase)

                For Each username In staleUsers
                    Dim computed = bizUtenti.CalcolaVisibilitaCombinata_Std(
                        username, CInt(enum_Id_Servizio.GiasOnline),
                        configs.DaCapostipiti, configs.DaCombinatoPratiche,
                        objParametri_Server, objParametri_Utenti
                    )
                    Dim piveSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                    For Each dr As DataRow In computed.Item1.Rows
                        Dim p = If(IsDBNull(dr("Piva")), "", CStr(dr("Piva"))).Trim()
                        If Not String.IsNullOrEmpty(p) Then piveSet.Add(p)
                    Next
                    staleUserPiveMap(username) = piveSet

                    Dim centriList As New List(Of Tuple(Of String, Integer))
                    For Each dr As DataRow In computed.Item2.Rows
                        Dim p = If(IsDBNull(dr("Piva")), "", CStr(dr("Piva"))).Trim()
                        Dim sa = If(IsDBNull(dr("Sa_Cod")), 0, CInt(dr("Sa_Cod")))
                        If Not String.IsNullOrEmpty(p) AndAlso sa > 0 Then
                            centriList.Add(Tuple.Create(p, sa))
                        End If
                    Next
                    staleUserCentriMap(username) = centriList
                Next

                ' Batch-enrich all unique PIVAs from stale users — one enrichment query for the whole stale cohort
                Dim allStalePive = staleUserPiveMap.Values.
                    SelectMany(Function(s) s).
                    Distinct(StringComparer.OrdinalIgnoreCase).
                    Select(Of IImpresaDto)(Function(p) New ImpresaDto With {.piva = p}).
                    ToList()
                Dim enrichedByPiva As New Dictionary(Of String, DataRow)(StringComparer.OrdinalIgnoreCase)
                If allStalePive.Any() Then
                    Dim enrichParams As New ObjParams With {
                        .ObjParametri_Server = objParametri_Server,
                        .ObjParametri_Utenti = objParametri_Utenti
                    }
                    Dim enrichedDT = GetImpreseFromVisibilita(allStalePive, enrichParams)
                    If enrichedDT IsNot Nothing Then
                        For Each dr As DataRow In enrichedDT.Rows
                            Dim p = If(IsDBNull(dr("Piva")), "", CStr(dr("Piva"))).Trim()
                            If Not String.IsNullOrEmpty(p) Then enrichedByPiva(p) = dr
                        Next
                    End If
                End If

                ' Restrict to current-user scope and requested imprese; project to output DTOs
                Dim restringiPive As HashSet(Of String) = Nothing
                If restringiVisibilita.Any() Then
                    restringiPive = restringiVisibilita.Select(Function(r) r.piva).ToHashSet(StringComparer.OrdinalIgnoreCase)
                End If

                For Each username In staleUsers
                    Dim nomeUtente = If(dettagli.ContainsKey(username), dettagli(username).nome, String.Empty)
                    Dim gruppoUtente = If(dettagli.ContainsKey(username), dettagli(username).gruppo, String.Empty)

                    ' Company-level records (Sa_Cod = 0)
                    For Each piva In staleUserPiveMap(username)
                        If hasCurrFilter AndAlso Not currUserPive.Contains(piva) Then Continue For
                        If restringiPive IsNot Nothing AndAlso Not restringiPive.Contains(piva) Then Continue For
                        Dim enriched As DataRow = Nothing
                        enrichedByPiva.TryGetValue(piva, enriched)
                        listaVisibilita.Add(New ImpresexUtentiVisibilita With {
                            .Username = username,
                            .DettagliUtente = nomeUtente,
                            .Gruppo_Des = gruppoUtente,
                            .piva = piva,
                            .Cuaa = If(enriched IsNot Nothing AndAlso Not IsDBNull(enriched("Codice_Cuaa")), CStr(enriched("Codice_Cuaa")), ""),
                            .rag_soc = If(enriched IsNot Nothing AndAlso Not IsDBNull(enriched("Rag_Soc")), CStr(enriched("Rag_Soc")), ""),
                            .Sa_Cod = 0,
                            .partitaIvaReale = If(enriched IsNot Nothing AndAlso Not IsDBNull(enriched("partitaIvaReale")), CStr(enriched("partitaIvaReale")), piva),
                            .Sa_Nome = String.Empty
                        })
                    Next

                    ' Center-level records (Sa_Cod > 0)
                    For Each centri In staleUserCentriMap(username)
                        Dim centriPiva = centri.Item1
                        If hasCurrFilter AndAlso Not currUserPive.Contains(centriPiva) Then Continue For
                        If restringiPive IsNot Nothing AndAlso Not restringiPive.Contains(centriPiva) Then Continue For
                        Dim enriched As DataRow = Nothing
                        enrichedByPiva.TryGetValue(centriPiva, enriched)
                        listaVisibilita.Add(New ImpresexUtentiVisibilita With {
                            .Username = username,
                            .DettagliUtente = nomeUtente,
                            .Gruppo_Des = gruppoUtente,
                            .piva = centriPiva,
                            .Cuaa = If(enriched IsNot Nothing AndAlso Not IsDBNull(enriched("Codice_Cuaa")), CStr(enriched("Codice_Cuaa")), ""),
                            .rag_soc = If(enriched IsNot Nothing AndAlso Not IsDBNull(enriched("Rag_Soc")), CStr(enriched("Rag_Soc")), ""),
                            .Sa_Cod = centri.Item2,
                            .partitaIvaReale = If(enriched IsNot Nothing AndAlso Not IsDBNull(enriched("partitaIvaReale")), CStr(enriched("partitaIvaReale")), centriPiva),
                            .Sa_Nome = String.Empty
                        })
                    Next
                Next
            End If
        Next

        Return listaVisibilita
    End Function

    Public Function LeggiVisibilitaGruppi(gruppi As IEnumerable(Of Integer), restringiVisibilita As List(Of ImpresaDto), params As ObjParams)
        Dim gropupsReader As New Gruppi_UtenteBiz(params.ObjParametri_Server, params.ObjParametri_Utenti)
        Dim users = gropupsReader.GetUtentiFromGruppoUtenti(gruppi).Select.AsParallel.
            Select(Function(r) If(IsDBNull(r("UserName")), "", r("UserName"))).
            Where(Function(username) Not String.IsNullOrWhiteSpace(username)).
            Select(Function(username) New UtenteDTO With {.UserName = username}).
            ToList
        Return LeggiVisibilitaUtenti(
            users, restringiVisibilita.ToList,
            params.ObjParametri_Server, params.ObjParametri_Utenti, True
        )
    End Function

    Private Sub ReplaceWithBasicData(
        ByRef users As IEnumerable(Of UtenteDTO),
        objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri
    )
        Dim objUtenti As New AgronicaCoreUtentiBIZ.Utenti
        Dim usernames = users.AsParallel.Select(Function(u) u.UserName.ToLower).ToHashSet
        Dim listaUtenti = objUtenti.Carica_Utenti_Dati_Base(enum_Id_Servizio.GiasOnline, objParametri_Server, objParametri_Utenti)
        If IsNothing(users) OrElse users.Count = 0 Then
            users = listaUtenti.ListaDatiBaseUtente.AsParallel.
                Select(Function(u) New UtenteDTO With {
                    .UserName = u.UserName,
                    .Nome = u.Nome,
                    .Cognome = u.Cognome
                }).ToList()
        Else
            users = listaUtenti.ListaDatiBaseUtente.AsParallel.
                Where(Function(u) usernames.Contains(u.UserName.ToLower)).
                Select(Function(u) New UtenteDTO With {
                    .UserName = u.UserName,
                    .Nome = u.Nome,
                    .Cognome = u.Cognome
                }).ToList()
        End If
    End Sub

    ''' <summary>
    ''' Legge alcuni dati aggiuntivi in riferimento alle aziende specificate.
    ''' </summary>
    ''' <param name="toRead">Collezione di imprese da leggere. (Basta che sia valorizzato il campo piva)</param>
    ''' <returns>Una DataTable con i dati riferiti alle aziende o Nothing se non sono state specificate aziende da leggere</returns>
    Public Function GetImpreseFromVisibilita(toRead As IEnumerable(Of IImpresaDto), params As ObjParams) As DataTable
        If toRead.Any Then
            Dim anagrafe As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim pive = toRead.AsParallel.
                Select(Function(impresa) "'" & impresa.piva & "'").
                Distinct
            'If pive.Count > 10_000 Then
            '    Return Nothing
            'End If

            Dim xIn = "i.piva in (" & pive.Aggregate(Function(acc, piva) acc & ", " & piva) & ")"
            Dim imprese As DataTable = anagrafe.Leggi_x_anagraficaVisibilita_Utente_NG(
                String.Empty, xIn, "i.piva ASC, i.rag_soc ASC",
                params.ObjParametri_Server, params.ObjParametri_Utenti,
                False, False
            )
            Return imprese
        End If
        Return Nothing
    End Function

    Public Function HaVisibilitaTotale(username As String, objParametri_utenti As AgronicaCoreParametri) As Boolean
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Return objProfilo.HasFullVisibility(username, objParametri_utenti)
    End Function

    Public Function HannoVisibilitaTotale(username As IEnumerable(Of String), objParametri_utenti As AgronicaCoreParametri) As IEnumerable(Of UtenteVisibilitaTotale)
        Dim xFilter = " Utenti_Profili.Id_Servizio = " & enum_Id_Servizio.GiasOnline
        Dim prepareFilter = Function(acc, u)
                                If u.index Mod 10 = 0 Then
                                    acc.name &= ", " & vbNewLine & u.name
                                Else
                                    acc.name &= ", " & u.name
                                End If
                                Return acc
                            End Function
        Dim uu = username.AsParallel.
            DefaultIfEmpty(String.Empty).
            Select(Function(str, i) New With {.name = "'" & str & "'", .index = i}).
            GroupBy(Function(u) u.index \ 10_000).
            Select(Function(batch) batch.Aggregate(prepareFilter).name).
            ToList ' lista di stringhe aggregate

        Dim result As New List(Of UtenteVisibilitaTotale)

        For Each group In uu
            Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            Dim pt = objProfilo.LeggiMassivo(group, xFilter, String.Empty, objParametri_utenti).
                Select.AsParallel.
                Select(Function(dr) 
                            Dim filtroSQL = dr("Descrizione_2")
                            Dim filtroPratiche = dr("filtro_pratiche_attivo")
                            Dim operatoreStr = dr("Operatore_Filtri")
                            Dim operatore = If(operatoreStr.Equals("AND", StringComparison.OrdinalIgnoreCase), PraticheFilterOperator.And, PraticheFilterOperator.Or)
                           Return New UtenteVisibilitaTotale(dr("Utente"), objProfilo.HasFullVisibility(filtroSQL, filtroPratiche, operatore))
                       End Function)
            result.AddRange(pt)
        Next
        Return result
    End Function

    Public Function HannoVisibilitaTotaleAggregato(username As IEnumerable(Of String), objParametri_utenti As AgronicaCoreParametri) As Boolean
        return HannoVisibilitaTotale(username, objParametri_utenti).
            All(Function(u) u.VisibilitaTotale)
    End Function

    Public Function ComparaVisibilitaUtenti(utenti As List(Of String), objParametri_utenti As AgronicaCoreParametri, objParametri_server As AgronicaCoreParametri) As Boolean
        If Not utenti.Any() Then
            Return 0
        ElseIf utenti.Count = 1 Then
            Return ComparaVisibilitaUtenti(
                    {utenti(0), objParametri_utenti.UsernameOperazione}.ToList,
                    objParametri_utenti, objParametri_server
                )
        End If

        Dim same = 0
        Dim A = LeggiVisibilitaUtente(utenti(0), New List(Of String), objParametri_utenti, objParametri_server).
                            Select(Function(i) i.piva).ToList()
        For i As Integer = 1 To utenti.Count - 1
            Dim B = LeggiVisibilitaUtente(utenti(i), New List(Of String), objParametri_utenti, objParametri_server).
                            Select(Function(j) j.piva).ToList()

            Dim AnB = A.Intersect(B)
            If AnB.Count = A.Count AndAlso AnB.Count = B.Count Then
                'Intersezione ha lo stesso numero di elementi dei due insiemi in verifica
                ' ==> gli insiemi sono uguali
                same = 0
            Else
                Dim AIB = A.Except(B)
                Dim BIA = B.Except(A)

                If AIB.Count > BIA.Count Then
                    'Togliendo l'intersezione da A ho più elementi che togliendola da B
                    ' ==> A è più grande
                    Return 1
                Else
                    'Togliendo l'intersezione da A ho meno o lo stesso numero di elementi che togliendola da B
                    ' ==> A è minore o uguale a B
                    Return -1
                End If
            End If
        Next

        Return same
    End Function

    ''' <param name="imprese">Collezione di imprese per cui voglio sapere se sono padri o meno</param>
    ''' <returns>Le pive delle imprese padre tra quelle specificate</returns>
    Public Function CheckIfAnyHasSons(imprese As IEnumerable(Of IImpresaDto), params As ObjParams)
        Dim hierarchy As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
        Dim fathers = hierarchy.FilterBusinessWithSons(
            imprese.AsParallel.Select(Function(b) b.piva),
            params.ObjParametri_Server
        )
        Return fathers
    End Function

#End Region

#Region "Scrittura"

    Public Sub ModificaVisibilitaAziendaUtenti(
        utenti As IEnumerable(Of IUtente), imprese As List(Of ImpresaDto),
        sovrascrivi As Boolean, bloccaSeHaVisibilita As Boolean,
        objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri
    )
        Try
            Const noUsersWithVisibility = -1
            Dim params As New ObjParams With {
                .ObjParametri_Server = objParametri_Server,
                .ObjParametri_Utenti = objParametri_Utenti
            }

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Utenti)

            Dim usernames = utenti.AsParallel.Select(Function(u) u.UserName)
            If Not VerificaEsistenzaUtenti(usernames, objParametri_Utenti) Then
                Throw New Exception("One or more users do not exist.")
            End If
            If bloccaSeHaVisibilita AndAlso CheckUsersVisibility(usernames, objParametri_Utenti) > noUsersWithVisibility Then
                Throw New Exception("Visibility has already been set for one or more users.")
            End If
            Dim usernamesBatch = utenti.AsParallel.
                  Select(Function(u, i) New With {.user = u, .index = i}).
                  GroupBy(Function(u) u.index \ 10_000).ToList

            If sovrascrivi Then
                usernamesBatch.ForEach(Sub(batch) OverwriteVisibility(batch.Select(Function(a) a.user), imprese, params))
            Else
                usernamesBatch.ForEach(Sub(batch) AddToVisibility(batch.Select(Function(a) a.user), imprese, params))
                'For Each utente In utenti
                '    Dim inVisibilita = LeggiVisibilitaUtente(utente.UserName, New List(Of String), objParametri_Utenti, objParametri_Server)
                '    Dim nuovaVisibilita = imprese.Union(inVisibilita).ToList()
                '    AssegnaVisibilita(utente.UserName, nuovaVisibilita, objParametri_Server, objParametri_Utenti)
                'Next
            End If

            UpdateAppoggioIfNotTooMany(utenti, params)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Utenti)
        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Utenti)
            Throw ex
        End Try
    End Sub

    Private Sub UpdateAppoggioIfNotTooMany(users As IEnumerable(Of IUtente), params As ObjParams)
        Dim xVisibAppoggio As New Utenti
        Dim leggiUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim configSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim chiaveStr = configSiti.Leggi_Valore(
            Sito_Cod:=0, Chiave:="AgroProfilazione_MaxUtentiCaricatiDefault",
            xFiltroAggiuntivo:=String.Empty, xOrderBy:=String.Empty, params.ObjParametri_Server
        )
        Dim smallMediumBusiness = If(String.IsNullOrWhiteSpace(chiaveStr), 200, CInt(chiaveStr))
        If users.Count <= smallMediumBusiness Then
            For Each user In users
                xVisibAppoggio.InizializzaTabellaUtentiVisibilitaAppoggio(
                    user.UserName, CInt(enum_Id_Servizio.GiasOnline),
                    params.ObjParametri_Server, params.ObjParametri_Utenti
                )
            Next
        End If
    End Sub

    ''' <param name="imprese">Pass an empty list to assign full visibility</param>
    Public Sub OverwriteVisibility(utenti As IEnumerable(Of IUtente), imprese As IEnumerable(Of IImpresaDto), params As ObjParams)
        Dim usernames = utenti.AsParallel.Select(Function(u) u.UserName)
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Write

        If imprese.Any Then
            Dim listaPive = CaricaGerarchia(imprese.ToList, params.ObjParametri_Server)
            objProfilo.CancellaMassivo(usernames, String.Empty, params.ObjParametri_Utenti)
            objProfilo.ScriviMassivo(
                usernames,
                CreaFiltroXMLPermessi(listaPive),
                CreaFiltroSQLPermessi(listaPive),
                params.ObjParametri_Utenti
            )
        Else 'Assign visibility over every business
            objProfilo.CancellaMassivo(usernames, String.Empty, params.ObjParametri_Utenti)
            objProfilo.ScriviMassivo(usernames, String.Empty, String.Empty, params.ObjParametri_Utenti)
        End If
    End Sub

    Public Sub AddToVisibility(utenti As IEnumerable(Of IUtente), imprese As IEnumerable(Of IImpresaDto), params As ObjParams)
        Dim profili As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim profiliW As New AgronicaCoreUtentiDAL.Utenti_Profili_Write
        Dim usernames = utenti.AsParallel.Select(Function(u) u.UserName).ToHashSet
        Dim pive = imprese.AsParallel.Select(Function(i) i.piva).ToHashSet
        Dim getStringOrDefault = Function(r As DataRow, field As String) If(IsDBNull(r(field)), String.Empty, r(field))

        Dim dt = profili.LeggiMassivo(usernames, String.Empty, String.Empty, params.ObjParametri_Utenti)
        Dim sameVisGr = dt.Select.AsParallel.GroupBy(Function(row) getStringOrDefault(row, "Descrizione_1"))
        For Each vis In sameVisGr
            Dim ancestors = LeggiPiveCapostipiti(vis.Key).Except({"###########"}).ToList
            Dim newVis = ancestors.Union(pive).Distinct.Select(Function(piva) New ImpresaDto With {.piva = piva})

            Dim listaPive = CaricaGerarchia(newVis, params.ObjParametri_Server)
            profiliW.CancellaMassivo(usernames, String.Empty, params.ObjParametri_Utenti)
            profiliW.ScriviMassivo(
                usernames,
                CreaFiltroXMLPermessi(listaPive),
                CreaFiltroSQLPermessi(listaPive),
                params.ObjParametri_Utenti
            )
        Next
    End Sub

    Public Sub ImpostaVisibilitaAzienda(
        utente As UtenteDTO,
        imprese As List(Of ImpresaDto),
        bloccaSeUtenteEsiste As Boolean,
        bloccaSeHaVisibilita As Boolean,
        objParametri_Server As AgronicaCoreParametri,
        objParametri_Utenti As AgronicaCoreParametri,
        Optional initTransition As Boolean = True
    )
        Dim xVisibAppoggio As New Utenti
        'Dim xVisibAppoggio_R As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim esisteUtente As Boolean = VerificaEsistenzaUtente(utente.UserName, objParametri_Utenti)
        Dim username = utente.UserName

        Try
            If initTransition Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Utenti)
            End If

            If bloccaSeUtenteEsiste AndAlso esisteUtente Then
                Throw New Exception("User already exists.")
            End If
            If bloccaSeHaVisibilita AndAlso VerificaVisibilitaUtente(utente.UserName, objParametri_Utenti) Then
                Throw New Exception("Visibility has already been set for user " & utente.UserName)
            End If

            If Not esisteUtente Then
                username = creaUtenteBase(utente, objParametri_Server, objParametri_Utenti)
            End If

            AssegnaVisibilita(username, imprese, objParametri_Server, objParametri_Utenti)
            If initTransition Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Utenti)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Utenti)
            End If
        Catch ex As Exception
            If initTransition Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Utenti)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Utenti)
            End If
            Throw ex
        End Try

    End Sub

    Public Sub AssegnaVisibilitaNulla(
        utente As IUtente,
        objParametri_Server As AgronicaCoreParametri,
        objParametri_Utenti As AgronicaCoreParametri
    )
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Write
        'Dim xVisibAppoggio_R As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim esisteUtente As Boolean = VerificaEsistenzaUtente(utente.UserName, objParametri_Utenti)
        If Not esisteUtente Then
            Throw New Exception("User does not exist!")
        End If
        Dim fittizia As List(Of ImpresaDto) = {New ImpresaDto("###########")}.ToList

        If VerificaVisibilitaUtente(utente.UserName, objParametri_Utenti) Then
            objProfilo.Cancella(utente.UserName, CInt(enum_Id_Servizio.GiasOnline), "", objParametri_Utenti)
        End If
        AssegnaVisibilita(utente.UserName, fittizia, objParametri_Server, objParametri_Utenti)
        'xVisibAppoggio.InizializzaTabellaUtentiVisibilitaAppoggio(
        '    username, CInt(enum_Id_Servizio.GiasOnline),
        '    objParametri_Server, objParametri_Utenti
        ')
    End Sub

    <Obsolete("La funzione implementa la copia di solo la gerarchia imprese in Utenti_Profili.
        Usare CopiaVisibilitaUtentiEstesa per gestire la nuova logica di visibilità che comprende anche le pratiche")>
    Public Sub CopiaVisibilitaUtenti(base As IEnumerable(Of String), template As String, objParametri_utenti As AgronicaCoreParametri)
        Dim objProfilo_R As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim objProfilo_W As New AgronicaCoreUtentiDAL.Utenti_Profili_Write
        Dim DTProfilo As DataTable = objProfilo_R.Leggi(
            template, CInt(enum_Id_Servizio.GiasOnline),
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            "", "",
            objParametri_utenti
        )

        If DTProfilo.Rows.Count <> 1 Then
            Throw New ArgumentOutOfRangeException()
        End If

        Dim enu = base.GetEnumerator
        While enu.MoveNext
            objProfilo_W.Cancella(enu.Current, enum_Id_Servizio.GiasOnline, "", objParametri_utenti)
            objProfilo_W.Scrivi(enu.Current, enum_Id_Servizio.GiasOnline,
                DTProfilo.Rows(0).Item("Descrizione_1"),
                DTProfilo.Rows(0).Item("Descrizione_2"),
                0, 0,
                AGRODATAINIZIO, AGRODATAFINE,
                objParametri_utenti
            )
        End While

    End Sub

    Public Sub CopiaVisibilitaUtentiEstesa(
        template As String, targets As IEnumerable(Of String),
        copyHierarchy As Boolean, copyProcedures As Boolean,
        objParametri_utenti As AgronicaCoreParametri
    )
        VerifyParamsForCopy(template, targets, copyHierarchy, copyProcedures, objParametri_utenti)

        Dim profileWriter As New AgronicaCoreUtentiDAL.Utenti_Profili_Write
        Dim profileProcedures As New AgronicaCoreUtentiDAL.Utenti_Profili_Pratiche
        Dim innerTargets = targets.Distinct().ToList()
        innerTargets.Remove(template)
        Dim batches = StringsInBatches(innerTargets)

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_utenti)

            For Each batch In batches
                profileWriter.Copia(template, batch, copyHierarchy, copyProcedures, objParametri_utenti)
                If copyProcedures Then
                    profileProcedures.Copia(template, batch, objParametri_utenti)
                End If
            Next

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_utenti)
        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_utenti)
            Throw ex
        End Try
    End Sub

    Public Sub RimuoviImpreseDaVisibilitaUtenti(
        utenti As IEnumerable(Of IUtente),
        imprese As IEnumerable(Of IImpresaDto),
        objParametri_server As AgronicaCoreParametri,
        objParametri_utenti As AgronicaCoreParametri
    )
        Dim enu = utenti.GetEnumerator
        Dim toRemove As IEnumerable(Of ImpresaDto)
        If imprese.Any() Then
            toRemove = imprese
        Else
            toRemove = LeggiVisibilitaUtente(objParametri_utenti.UtenteUsername, New List(Of String), objParametri_utenti, objParametri_server)
        End If

        While enu.MoveNext
            Dim visibilita As IEnumerable(Of ImpresaDto) = LeggiVisibilitaUtente(
                enu.Current.UserName, New List(Of String),
                objParametri_utenti, objParametri_server
            )
            Dim newVisibility = visibilita.ToList()
            If HaVisibilitaTotale(objParametri_utenti.UtenteUsername, objParametri_utenti) AndAlso Not imprese.Any() Then
                newVisibility = New List(Of ImpresaDto)()
            Else
                newVisibility.RemoveAll(Function(i) toRemove.Any(Function(tr) tr.piva = i.piva))
            End If

            If newVisibility.Any Then
                AssegnaVisibilita(
                    enu.Current.UserName, newVisibility,
                    objParametri_server, objParametri_utenti
                )
            Else
                AssegnaVisibilitaNulla(enu.Current, objParametri_server, objParametri_utenti)
            End If

        End While

    End Sub


#End Region

#Region "Lettura"
    ''' <param name="listaUtenti">Utenti di cui si vuole controllare la visibilità</param>
    ''' <returns>Ritorna true se gli utenti specificati hanno tutti stessa visibilità, false in caso contrario.</returns>
    Public Function ControllaStessaVisibilita(ByVal listaUtenti As List(Of String),
                                             ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             ByVal objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        If (listaUtenti.Count > 1) Then
            Dim impreseVisibiliConfronto = LeggiVisibilitaUtente(listaUtenti(0), New List(Of String), objParametriUtenti, objParametriServer)

            listaUtenti.Remove(listaUtenti(0))

            For Each utente In listaUtenti
                Dim impreseVisibili = LeggiVisibilitaUtente(utente, New List(Of String), objParametriUtenti, objParametriServer)
                Dim common = impreseVisibili.Intersect(impreseVisibiliConfronto)
                If common.Count <> impreseVisibili.Count OrElse common.Count <> impreseVisibiliConfronto.Count Then
                    Return False
                End If
            Next
        End If

        Return True

    End Function


    Public Function ControllaStessaVisibilitaLite(ByVal listaUtenti As List(Of String), params As ObjParams) As Boolean
        If listaUtenti.Count <= 1 Then Return True

        ' One batch profile read covers hierarchy AND procedure flags — avoids separate DB round-trips
        Dim profiloR As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim xFilter = " Utenti_Profili.Id_Servizio = " & CInt(enum_Id_Servizio.GiasOnline)
        Dim DTProfilo As DataTable = profiloR.LeggiMassivo(listaUtenti, xFilter, String.Empty, params.ObjParametri_Utenti)

        If DTProfilo Is Nothing OrElse DTProfilo.Rows.Count = 0 Then Return False

        ' Users with no profile have full visibility; mixing profiled and non-profiled means different visibility
        Dim distinctProfiles = DTProfilo.AsEnumerable().
            Select(Function(dr) If(IsDBNull(dr("Utente")), "", CStr(dr("Utente"))).ToLower()).
            Distinct().Count()
        If distinctProfiles <> listaUtenti.Select(Function(u) u.ToLower()).Distinct().Count() Then Return False

        ' Fast-fail: if any user differs in procedure filter flags, visibility is different
        Dim refPratiche = Not DTProfilo.Rows(0).IsNull("filtro_pratiche_attivo") AndAlso CBool(DTProfilo.Rows(0)("filtro_pratiche_attivo"))
        Dim refOperatore = If(DTProfilo.Rows(0).IsNull("Operatore_Filtri"), "OR", CStr(DTProfilo.Rows(0)("Operatore_Filtri")).Trim().ToUpperInvariant())
        For Each row As DataRow In DTProfilo.Rows
            Dim pratiche = Not row.IsNull("filtro_pratiche_attivo") AndAlso CBool(row("filtro_pratiche_attivo"))
            Dim operatore = If(row.IsNull("Operatore_Filtri"), "OR", CStr(row("Operatore_Filtri")).Trim().ToUpperInvariant())
            If pratiche <> refPratiche OrElse operatore <> refOperatore Then Return False
        Next

        ' Check hierarchy: expand distinct Descrizione_1 XML values and compare PIVA sets
        Dim distinctDescr1 = DTProfilo.AsEnumerable().
            Select(Function(dr) If(dr.IsNull("Descrizione_1"), String.Empty, CStr(dr("Descrizione_1")))).
            Distinct().
            ToList()

        If distinctDescr1.Count = 1 Then Return True

        Dim ref = GetPiveHierarchyFromXML(distinctDescr1.First(), params)
        For Each descr1 In distinctDescr1.Skip(1)
            Dim pive = GetPiveHierarchyFromXML(descr1, params)
            If Not ref.SetEquals(pive) Then
                Return False
            End If
        Next

        Return True
    End Function

    Private Function GetPiveHierarchyFromXML(xmlFilter As String, params As ObjParams) As HashSet(Of String)
        If String.IsNullOrEmpty(xmlFilter) Then
            Return New HashSet(Of String)
        End If

        Dim filter = String.Empty
        Dim objGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
        Dim ancestors = RegularExpressions.Regex.Matches(xmlFilter, "\""(([a-z0-9A-Z#]){11,})\""+", RegexOptions.None, TimeSpan.FromSeconds(3)).
                    Cast(Of RegularExpressions.Match)().
                    Select(Function(m) m.Value.Replace("""", "")).ToList()
        If ancestors.Any() Then
            filter = ancestors.Select(Function(p) "'" & p & "'").
                                Aggregate(Function(p1, p2) p1 & ", " & p2)
        End If
        Return objGerarchia.LeggixGerarchiaAlberoImprese(filter, "", "", params.ObjParametri_Server).
            Select.AsParallel.Select(Function(i) CStr(i.Item("PIVA"))).
            ToHashSet()
    End Function

    ''' <param name="utente">L'utente selezionato.</param>
    ''' <param name="utenteConfronto">L'utente la quale visibilità si vuole confrontare con l'utente selezionato.</param>
    ''' <returns>Ritorna -1 se l'utente selezionato vede meno imprese dell'utente di confronto, 0 se entrambi hanno uguale visibilità, 1 se l'utente selezionato vede più imprese dell'utente di confronto.</returns>
    Public Function ConfrontaVisibilitaUtenti(ByVal utente As String,
                                              ByVal utenteConfronto As String,
                                             ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             ByVal objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim hasFullViz1 = HaVisibilitaTotale(utente, objParametriUtenti)
        Dim hasFullViz2 = HaVisibilitaTotale(utenteConfronto, objParametriUtenti)
        If hasFullViz1 AndAlso hasFullViz2 Then
            Return 0
        ElseIf hasFullViz1 AndAlso Not hasFullViz2 Then
            Return 1
        ElseIf Not hasFullViz1 AndAlso hasFullViz2 Then
            Return -1
        End If

        Dim impreseVisibili = LeggiVisibilitaUtente(utente, New List(Of String), objParametriUtenti, objParametriServer)
        Dim impreseVisibiliConfronto = LeggiVisibilitaUtente(utenteConfronto, New List(Of String), objParametriUtenti, objParametriServer)

        Dim common = impreseVisibili.Intersect(impreseVisibiliConfronto)
        Dim diff = impreseVisibili.Except(impreseVisibiliConfronto)

        If common.Count = impreseVisibili.Count AndAlso common.Count = impreseVisibiliConfronto.Count Then
            Return 0
        End If

        Return IIf(diff.Any(), 1, -1)
    End Function

    Public Function LeggiPiveCapostipiti(username As String, objUtenti As AgronicaCoreParametri) As List(Of String)
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        'Return objProfilo.GetVisibilityAncestors(username, objUtenti)

        Dim DTProfilo As DataTable = objProfilo.Leggi(
            username, CInt(enum_Id_Servizio.GiasOnline),
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            "", "",
            objUtenti
        )
        If DTProfilo.Rows.Count = 1 AndAlso DTProfilo.Rows(0).Item("Descrizione_1") <> "" Then
            Dim descr1 = CType(DTProfilo.Rows(0).Item("Descrizione_1"), String)
            Return LeggiPiveCapostipiti(descr1)
        Else
            Return New List(Of String)
        End If
    End Function

    Public Function LeggiPiveCapostipiti(stringaXmlPermessi As String) As List(Of String)
        If Not String.IsNullOrEmpty(stringaXmlPermessi) Then
            Return RegularExpressions.Regex.Matches(stringaXmlPermessi, "\""(([a-z0-9A-Z#]){11,})\""+", RegexOptions.None, TimeSpan.FromSeconds(3)).
                    Cast(Of RegularExpressions.Match)().
                    Select(Function(m) m.Value.Replace("""", "")).ToList()
        Else
            Return New List(Of String)
        End If
    End Function

    'Private Function LeggiPiveVisibiliDaCapostipiti(username As String, objParametri_Utenti As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri) As HashSet(Of String)
    '    Dim objGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
    '    Dim listaPive As List(Of String) = LeggiPiveCapostipiti(username, objParametri_Utenti)
    '    Return objGerarchia.LeggiGerarchiaDaCapostipiti(listaPive, objParametri_Server).
    '        Select.AsParallel.
    '        Select(Function(i) CStr(i.Item("PIVA"))).ToHashSet
    'End Function
#End Region

#Region "Funzioni Private"

    Private Function creaUtenteBase(utente As UtenteDTO,
                                    objParametri_Server As AgronicaCoreParametri,
                                    objParametri_Utenti As AgronicaCoreParametri) As String

        Dim handleConfigSiti As New Configurazione_Siti_R
        Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Write
        Dim objDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W
        Dim objTipologiexPermessi_W As New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_W
        Dim objImpostazioniFiltroMono_W As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_W
        Dim esitoOperazioneOK As Boolean
        Dim tipologiaCod As Integer = 0
        If utente.Tipologia IsNot Nothing AndAlso utente.Tipologia.codice Then
            tipologiaCod = utente.Tipologia.codice
        End If

        Dim hashPasswordAbilitato As Boolean = False
        Dim dtConfigSiti As DataTable = handleConfigSiti.Leggi(0, "AbilitaHashPassword", "", "", objParametri_Server)
        If dtConfigSiti.Rows.Count > 0 Then
            hashPasswordAbilitato = dtConfigSiti.Rows(0)("Valore")
        End If

        'Scrivo record utente
        Dim username = Pulisci_Username(utente.UserName)
        Dim password = Pulisci_Username(utente.Password)

        'se la password è blank viene generata una random
        If password = "" Then
            password = AgronicaCoreUtentiBIZ.Utenti.GeneraPasswordRequisiti()
        End If

        If Not Utenti.ValidaComplessitaPassword(password, gestioneHashAbilitata:=hashPasswordAbilitato) Then
            Dim strErr = Utenti.MessaggioRequisitiPassword(gestioneHashAbilitata:=hashPasswordAbilitato)
            Throw New Exception(strErr)
        End If

        esitoOperazioneOK = objUtente.Scrivi(username, password,
                         enum_AgroLingue.Italiano_it,
                         hashPasswordAbilitato, False,
                         objParametri_Utenti, tipologiaCod)
        If Not esitoOperazioneOK Then
            Throw New Exception("Error occurred in user creation.")
        End If

        'Scrivo record dettagli utente
        'TODO: imposta check tipo utente
        If utente.flag_azienda_persona = 2 Then
            esitoOperazioneOK = objDettagli.Scrivi(username, utente.Cognome, utente.Nome,
                           Tel:="", utente.Email, utente.piva, utente.codice_fiscale,
                           utente.Rag_Soc, utente.flag_azienda_persona,
                           utente.username_commerciale, objParametri_Utenti)
        Else
            'TODO: fix valorizzazione dati
            esitoOperazioneOK = objDettagli.Scrivi(username, Cognome:="", Nome:="",
                           Tel:="", utente.Email, utente.piva, utente.codice_fiscale,
                           utente.Rag_Soc, utente.flag_azienda_persona,
                           utente.username_commerciale, objParametri_Utenti)
        End If
        If Not esitoOperazioneOK Then
            Throw New Exception("Error occurred in user creation.")
        End If

        If tipologiaCod <> 0 Then
            'Assegno permessi da tipologia utente
            objTipologiexPermessi_W.GeneraPermessiUtenteDaTipologia(
                utente.Tipologia.codice, username,
                AGRODATAINIZIO, AGRODATAFINE,
                objParametri_Utenti
            )
            'Assegno impostazioni da tipologia utente
            objTipologiexPermessi_W.GeneraImpostazioniUtenteDaTipologia(
                utente.Tipologia.codice, username,
                AGRODATAINIZIO, AGRODATAFINE,
                xFiltroAggiuntivo:="", objParametri_Utenti
            )
            'Impostazioni filtromono da tipologia utente
            objImpostazioniFiltroMono_W.ApplicaProfilo(
                utente.Tipologia.codice,
                username,
                objParametri_Utenti
            )
        End If

        Return username

    End Function

    Private Function Pulisci_Username(ByVal Username As String) As String
        Username = Username.Replace("'", "")
        Username = Username.Replace(" ", "")
        Username = Username.Replace("à", "a")
        Username = Username.Replace("è", "e")
        Username = Username.Replace("é", "e")
        Username = Username.Replace("ù", "u")
        Username = Username.Replace("ò", "o")
        Username = Username.Replace("ì", "i")
        Return Username
    End Function

    Private Sub AssegnaVisibilita(
        username As String, imprese As IEnumerable(Of IImpresaDto),
        objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri
    )

        Dim xVisibAppoggio As New Utenti
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Write
        Dim listaPive = CaricaGerarchia(imprese, objParametri_Server)

        objProfilo.Cancella(username, CInt(enum_Id_Servizio.GiasOnline), "", objParametri_Utenti)

        objProfilo.Scrivi(username, CInt(enum_Id_Servizio.GiasOnline),
                          CreaFiltroXMLPermessi(listaPive),
                          CreaFiltroSQLPermessi(listaPive),
                          Codice_1:=0, Codice_2:=0,
                          CostantiPersonalizzate.AGRODATAINIZIO,
                          CostantiPersonalizzate.AGRODATAFINE,
                          objParametri_Utenti)

        xVisibAppoggio.InizializzaTabellaUtentiVisibilitaAppoggio(
            username, CInt(enum_Id_Servizio.GiasOnline),
            objParametri_Server, objParametri_Utenti
        )

    End Sub

    ''' <summary>
    ''' Carica la gerarchia di imprese visibili a partire da quelle specificate.
    ''' </summary>
    ''' <param name="imprese">Imprese da cui leggere la gerarchia</param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns>Lista di stringhe su cui creare i filtri visibilità</returns>
    Private Function CaricaGerarchia(imprese As IEnumerable(Of IImpresaDto), objParametri_Server As AgronicaCoreParametri) As List(Of String)
        Dim gerarchiaImprese As New GerarchiaImprese_R
        Dim initialPive = imprese.AsParallel.Select(Function(i) i.piva).Distinct.ToList
        Dim hierarchy = gerarchiaImprese.LeggiGerarchiaDaCapostipiti(initialPive, objParametri_Server, True).Select.AsParallel
        Dim sons = hierarchy.
            Where(Function(row) CInt(row("TipoImpresaGerarchia")) > enum_TipoImpresaGerarchia.Impresa).
            Select(Function(row) CStr(row("figlio"))).ToList
        Dim includedInHierarchy = hierarchy.Where(Function(row) initialPive.Contains(CStr(row("figlio"))) AndAlso initialPive.Contains(CStr(row("padre")))).
            Select(Function(row) CStr(row("figlio"))).ToList
        initialPive = initialPive.Except(includedInHierarchy).ToList
        Return initialPive.Union(sons).ToList
    End Function

    Private Function CreaFiltroXMLPermessi(piveImprese As List(Of String)) As String

        Dim xmlPermessi As New StringBuilder
        xmlPermessi.Append("<DatiFiltri><Filtro><DatiGerarchiaImprese>")
        For Each piva In piveImprese
            If piva <> "" Then
                xmlPermessi.Append("<GerarchiaImprese padre=""" & piva & """/>")
            End If
        Next
        xmlPermessi.Append("</DatiGerarchiaImprese><DatiPive/><Impresa><Struttura><Appezzamento><Impianto><Agenda><Contatto/></Agenda></Impianto></Appezzamento></Struttura></Impresa></Filtro></DatiFiltri>")

        If piveImprese.Count = 0 Then
            xmlPermessi.Clear()
        End If

        Return xmlPermessi.ToString()

    End Function

    Private Function CreaFiltroSQLPermessi(piveImprese As List(Of String)) As String

        Dim sqlPermessi As New StringBuilder
        For Each piva In piveImprese
            If piva <> "" Then
                sqlPermessi.Append(" ((GerarchiaImprese.Padre = '" & piva & "' and GerarchiaImprese.Foglia=1 ) OR Imprese.Piva = '" & piva & "') OR")
            End If
        Next
        If sqlPermessi.ToString() <> "" Then
            Return "AND (" & Left(sqlPermessi.ToString, sqlPermessi.Length - 2) & ")"
        End If
        Return sqlPermessi.ToString()

    End Function

    Private Function VerificaEsistenzaUtente(username As String, objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim objUtentiDettagliDAL As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim DtDettagliUtenti As DataTable
        DtDettagliUtenti = objUtentiDettagliDAL.Leggi(username, CInt(enum_Id_Servizio.Nessuno),
                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                      "", "",
                                                      objParametri_Utenti)

        Return (DtDettagliUtenti IsNot Nothing AndAlso DtDettagliUtenti.Rows.Count > 0)
    End Function

    ''' <returns>True if all the users exist, False otherwise.</returns>
    Private Function VerificaEsistenzaUtenti(username As IEnumerable(Of String), objParametri_Utenti As AgronicaCoreParametri) As Boolean
        If username Is Nothing OrElse username.Count = 0 Then
            Return True
        End If
        Dim objUtentiDettagliDAL As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim xFiltro = username.AsParallel.Select(Function(name) "'" & name & "'").
            Aggregate(Function(acc, str) acc & "," & str)
        xFiltro = " Utenti_Dettagli.UserName IN ( " & xFiltro & " ) "
        Dim DtDettagliUtenti = objUtentiDettagliDAL.Leggi(
            String.Empty, CInt(enum_Id_Servizio.Nessuno),
            enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
            xFiltro, String.Empty, objParametri_Utenti
        )
        Return (DtDettagliUtenti IsNot Nothing AndAlso DtDettagliUtenti.Rows.Count = username.Count)
    End Function

    Private Function VerificaVisibilitaUtente(username As String, objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim DTProfilo As DataTable
        DTProfilo = objProfilo.Leggi(username, CInt(enum_Id_Servizio.GiasOnline),
                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "", "",
                                     objParametri_Utenti)

        Return (DTProfilo IsNot Nothing AndAlso DTProfilo.Rows.Count > 0)
    End Function

    ''' <returns>1 if all users have visibility, -1 if none of the users have visibility, 0 otherwise.</returns>
    Private Function CheckUsersVisibility(username As IEnumerable(Of String), objParametri_Utenti As AgronicaCoreParametri) As Integer
        If username Is Nothing OrElse username.Count = 0 Then
            Return True
        End If
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim xFiltro = username.AsParallel.Select(Function(name) "'" & name & "'").
            Aggregate(Function(acc, str) acc & "," & str)
        xFiltro = " Utente IN ( " & xFiltro & " ) "
        Dim DTProfilo = objProfilo.Leggi(
            String.Empty, CInt(enum_Id_Servizio.GiasOnline),
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            xFiltro, String.Empty, objParametri_Utenti
        )
        Dim noVis = DTProfilo.Select(" Descrizione_1 like '%########%' ").Count

        If noVis = DTProfilo.Rows.Count Then
            Return -1
        ElseIf noVis = 0 AndAlso DTProfilo.Rows.Count = username.Count Then
            Return 1
        Else
            Return 0
        End If
    End Function

    Private Function LeggiAppoggioXImprese(utenti As List(Of UtenteDTO),
                                           imprese As List(Of ImpresaDto),
                                           objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim objUtentiAppoggio As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim xFiltroAggiuntivo As New StringBuilder With {.Length = 0}

        If imprese.Any Then
            xFiltroAggiuntivo.Append(" i.PIVA in ( ")
            xFiltroAggiuntivo.Append(
                imprese.Select(Function(i) "'" & i.piva & "'").
                    Aggregate(Function(p1, p2) p1 & ", " & p2)
            )
            xFiltroAggiuntivo.Append(" ) ")
        End If
        If xFiltroAggiuntivo.Length > 0 AndAlso utenti.Any() Then
            xFiltroAggiuntivo.Append(" AND ")
        End If
        If utenti.Any Then
            xFiltroAggiuntivo.Append(" va.Username in ( ")
            xFiltroAggiuntivo.Append(
                utenti.Select(Function(u) If(u.UserName.StartsWith("'"), u.UserName, $"'{u.UserName}'")).
                    Aggregate(Function(u1, u2) u1 & ", " & u2)
            )
            xFiltroAggiuntivo.Append(" ) ")
        End If

        Return objUtentiAppoggio.LeggiJoinImprese(xFiltroAggiuntivo.ToString(), "", objParametri_Server)
    End Function

    ''' <summary>
    ''' Reads the visibility of a user, optionally filtered by a list of PIVA (VAT numbers).
    ''' </summary>
    ''' <see cref="LeggiVisibilitaUtenti" />
    ''' <param name="utenteUsername">Utenti di cui si vuole conoscere la visibilità</param>
    ''' <param name="restringiVisibilita">Pive delle imprese da considerare nella lettura della visibilità.
    ''' Funge da filtro sulle aziende che si andranno a leggere, col risultato di mostrare la
    ''' visibilità dell'utente solo sul sottoinsieme di imprese indicato. Passare una lista
    ''' vuota per evitare di filtrare le aziende.</param>
    Private Function LeggiVisibilitaUtente(
        utenteUsername As String, restringiVisibilita As List(Of String),
        objParametri_Utenti As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri
    ) As List(Of ImpresaGerarchiaCuaaDto)
        'Dim objGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
        Dim uva As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R

        Dim listaPive As List(Of String) = uva.LeggiJoinImprese(objParametri_Server, objParametri_Server.UtenteUsername).
                Select.AsParallel.Select(Function(dr) dr.Field(Of String)("PIVA")).ToList
        If restringiVisibilita.Any() AndAlso listaPive.Any() Then
            listaPive = listaPive.Where(Function(i) restringiVisibilita.Contains(i))
        ElseIf restringiVisibilita.Any() AndAlso Not listaPive.Any() Then
            listaPive = restringiVisibilita
        End If

        Dim vizUser = uva.LeggiJoinImprese(objParametri_Server, utenteUsername).
            Select.AsParallel.Select(Function(dr) New ImpresaGerarchiaCuaaDto With {
                .piva = dr.Field(Of String)("PIVA"),
                .Cuaa = dr.Field(Of String)("CUAA"),
                .rag_soc = dr.Field(Of String)("Rag_Soc"),
                .Sa_Nome = dr.Field(Of String)("Sa_Nome"),
                .partitaIvaReale = If(IsDBNull(dr.Item("partitaIvaReale")), If(IsDBNull(dr.Item("figlio")), String.Empty, dr.Item("figlio")), dr.Item("partitaIvaReale"))
            }).ToList()

        If restringiVisibilita.Any() Then
            Return vizUser.Where(Function(i) restringiVisibilita.Contains(i.piva)).ToList()
        Else
            Return vizUser
        End If

        'Return objGerarchia.LeggiGerarchiaDaCapostipiti(
        '        listaPive, objParametri_Server,
        '        enumSelezioneVariabile.Selezione_JoinCompleta
        '    ).Select.AsParallel.
        '    Select(Function(i) New ImpresaGerarchiaCuaaDto With {
        '        .piva = If(IsDBNull(i.Item("figlio")), String.Empty, i.Item("figlio")),
        '        .Cuaa = If(IsDBNull(i.Item("cuaa")), String.Empty, i.Item("cuaa")),
        '        .Padre = If(IsDBNull(i.Item("Padre")), String.Empty, i.Item("Padre")),
        '        .rag_soc = If(IsDBNull(i.Item("rag_soc")), String.Empty, i.Item("rag_soc")),
        '        .IsFoglia = i.Item("Foglia"),
        '        .partitaIvaReale = If(IsDBNull(i.Item("partitaIvaReale")), If(IsDBNull(i.Item("figlio")), String.Empty, i.Item("figlio")), i.Item("partitaIvaReale")),
        '        .Sa_Nome = ""
        '    }).Distinct.ToList()
    End Function

    ''' <summary>
    ''' Groups the specified strings into batches.
    ''' </summary>
    ''' <param name="strCollection">The complete enumerable of strings to be grouped into batches.</param>
    ''' <param name="batchSize">The maximum number of strings in each batch.</param>
    ''' <returns>A list of enumerable batches, each containing up to the specified number of strings.</returns>
    Private function StringsInBatches(strCollection As IEnumerable(Of String), Optional batchSize As Integer = 10_000) As List(Of IEnumerable(Of String))
        Return strCollection.AsParallel.
            DefaultIfEmpty(String.Empty).
            Select(Function(str, i) New With {.name = str, .index = i}).
            GroupBy(Function(u) u.index \ batchSize).
            Select(Function(batch) batch.Select(Function(b) b.name)).
            ToList
    End Function

    ''' <summary>
    ''' Verifies that the parameters provided for copying visibility are valid.
    ''' Checks that at least one of copyHierarchy or copyProcedures is true,
    ''' that the template user exists, and that all target users exist.
    ''' </summary>
    ''' <param name="template">The username of the template user whose visibility settings will be copied.</param>
    ''' <param name="targets">A collection of usernames of the target users to which the visibility settings will be copied.</param>
    ''' <param name="copyHierarchy">Indicates whether to copy the hierarchy settings.</param>
    ''' <param name="copyProcedures">Indicates whether to copy the procedures settings.</param>
    ''' <param name="objParametri_utenti">The parameters object containing user-specific settings.</param>
    ''' <exception cref="ArgumentException">Thrown if the parameters are invalid.</exception>
    ''' <exception cref="ArgumentOutOfRangeException">Thrown if the template user does not exist or if any target user does not exist.</exception>
    Private Sub VerifyParamsForCopy(
        template As String, targets As IEnumerable(Of String),
        copyHierarchy As Boolean, copyProcedures As Boolean,
        objParametri_utenti As AgronicaCoreParametri
    )
        If (not copyHierarchy AndAlso not copyProcedures) Then
            Throw New ArgumentException("At least one of copyHierarchy or copyProcedures must be true and template must be provided.")
        End If

        Dim objProfilo_R As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim verified = objProfilo_R.Leggi(template, objParametri_utenti)
        If (verified.Rows.Count <> 1) Then
            Throw New ArgumentOutOfRangeException("Template user does not exist.")
        End If

        Dim batches = StringsInBatches(targets)
        For Each batch In batches
            verified = objProfilo_R.LeggiMassivo(batch, "", "", objParametri_utenti)
            If (verified.Rows.Count <> batch.Count()) Then
                Throw New ArgumentOutOfRangeException("One or more target users do not exist.")
            End If
        Next
    End Sub

    ''' <summary>
    ''' Site-level configuration flags for the combined visibility algorithm.
    ''' Read once per outer operation to avoid repeated DB round-trips to Configurazione_Siti.
    ''' </summary>
    Private Class VizConfigFlags
        Public Property DaCapostipiti As Boolean
        Public Property DaCombinatoPratiche As Boolean
    End Class

    ''' <summary>
    ''' Reads <see cref="VizConfigFlags"/> from Configurazione_Siti (site_cod = 0).
    ''' Mirrors the private helpers LeggiFlagCapostipiti_Std and UsaCalcoloCombinatoStd
    ''' in Utenti, which are inaccessible from this class.
    ''' </summary>
    Private Function LeggiVizConfigFlags(objParametri_server As AgronicaCoreParametri) As VizConfigFlags
        Dim cfg As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim rawCapostipiti = cfg.Leggi_Valore(0, "Utenti_Visibilia_Appoggio_Da_Capostipiti", "", "", objParametri_server)
        Dim rawCombinato = cfg.Leggi_Valore(0, "Utenti_Visibilita_Calcolo_Combinato_Pratiche", "", "", objParametri_server)
        Return New VizConfigFlags With {
            .DaCapostipiti = String.IsNullOrWhiteSpace(rawCapostipiti) OrElse rawCapostipiti.Trim() = "1",
            .DaCombinatoPratiche = rawCombinato.Equals("1")
        }
    End Function

    ''' <summary>
    ''' Returns True when the cached appoggio for this profile row is out of date or
    ''' was populated without the combined-pratiche algorithm, requiring on-the-fly computation.
    ''' </summary>
    Private Function IsProfiloStale(profiloRow As DataRow, configs As VizConfigFlags) As Boolean
        ' Timestamp missing or older than last profile change
        Dim dataModifica = If(profiloRow.IsNull("Data_Modifica"), DateTime.MinValue, CDate(profiloRow("Data_Modifica")))
        Dim dataUltimoRiporto As Nullable(Of DateTime)
        If profiloRow.IsNull("DataUltimoRiportoUtentiVisibilitaAppoggio") Then
            dataUltimoRiporto = Nothing
        Else
            dataUltimoRiporto = CDate(profiloRow("DataUltimoRiportoUtentiVisibilitaAppoggio"))
        End If
        If Not dataUltimoRiporto.HasValue OrElse dataUltimoRiporto.Value < dataModifica Then Return True

        ' Procedures are active but the combined write-path is disabled:
        ' appoggio was populated without procedure data, so it is stale for combined reads
        Dim filtroPraticheAttivo = Not profiloRow.IsNull("filtro_pratiche_attivo") AndAlso
                                   CBool(profiloRow("filtro_pratiche_attivo"))
        If filtroPraticheAttivo AndAlso Not configs.DaCombinatoPratiche Then Return True

        Return False
    End Function

#End Region

End Class
