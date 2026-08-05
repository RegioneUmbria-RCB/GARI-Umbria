Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Demetra
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreMapper.Utility
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreModello
Imports AgronicaCoreModello.AppHelper
Imports AgronicaCoreModello.Utility_Agenda
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo
Imports AgronicaCoreModelsSTD.attivita.dettagli
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreUtentiDAL
Imports Newtonsoft.Json
Imports Impianto = AgronicaCoreModelsSTD.anagrafiche.Impianto
Imports Prodotto = AgronicaCoreModelsSTD.attivita.risorse.Prodotto


Public Class ImportAttivita

    Public Shared Function GetListaAttivitaAmmissibili(ByVal tipo As enum_Dati_App) As List(Of Integer)
        Dim lstLavCod As New List(Of Integer)


        Select Case tipo
            Case enum_Dati_App.AttivitaDemetra, enum_Dati_App.RicetteDemetra
                lstLavCod.AddRange({
                    CostantiPersonalizzate.LAVCOD_ALTRE_OPERAZIONI,
                    CostantiPersonalizzate.LAVCOD_ARATURA,
                    CostantiPersonalizzate.LAVCOD_DISERBO,
                    CostantiPersonalizzate.LAVCOD_DISTRIBUZIONE_CONCIME,
                    CostantiPersonalizzate.LAVCOD_ERPICATURA,
                    CostantiPersonalizzate.LAVCOD_FALCIATURA_ERBAI,
                    CostantiPersonalizzate.LAVCOD_FERTIRRIGAZIONE,
                    CostantiPersonalizzate.LAVCOD_FRANGIZOLLATURA,
                    CostantiPersonalizzate.LAVCOD_FRESATURA,
                    CostantiPersonalizzate.LAVCOD_GEODISINFESTAZIONE,
                    CostantiPersonalizzate.LAVCOD_IRRIGAZIONE,
                    CostantiPersonalizzate.LAVCOD_MIETITREBBIATURA,
                    CostantiPersonalizzate.LAVCOD_POTATURA_SECCA,
                    CostantiPersonalizzate.LAVCOD_POTATURA_VERDE,
                    CostantiPersonalizzate.LAVCOD_RANGHINATURA,
                    CostantiPersonalizzate.LAVCOD_SEMINA,
                    CostantiPersonalizzate.LAVCOD_FALCIACONDIZIONATURA, 'Sfalcio
                    CostantiPersonalizzate.LAVCOD_TRAPIANTO,
                    CostantiPersonalizzate.LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                    CostantiPersonalizzate.LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                    CostantiPersonalizzate.LAVCOD_TRINCIATURA,
                    CostantiPersonalizzate.LAVCOD_RACCOLTA,
                    CostantiPersonalizzate.LAVCOD_ASPORTAZIONE_ORGANI_INFETTI,
                    CostantiPersonalizzate.LAVCOD_CONCIMAZIONE_FOGLIARE,
                    CostantiPersonalizzate.LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                    CostantiPersonalizzate.LAVCOD_MINIMUM_TILLAGE,
                    CostantiPersonalizzate.LAVCOD_PIRODISERBO,
                    CostantiPersonalizzate.LAVCOD_PASCOLAMENTO_PROPRIO,
                    CostantiPersonalizzate.LAVCOD_PASCOLAMENTO_TERZI,
                    CostantiPersonalizzate.LAVCOD_ANDANAMENTO,
                    CostantiPersonalizzate.LAVCOD_DEFOGLIAZIONE,
                    CostantiPersonalizzate.LAVCOD_ASSOLCATURA,
                    CostantiPersonalizzate.LAVCOD_CARICO_MANUALE_FRUTTA,
                    CostantiPersonalizzate.LAVCOD_CIMATURA,
                    CostantiPersonalizzate.LAVCOD_DIRADAMENTO_MANUALE,
                    CostantiPersonalizzate.LAVCOD_DISSODAMENTO,
                    CostantiPersonalizzate.LAVCOD_ESTIRPATURA,
                    CostantiPersonalizzate.LAVCOD_ESPIANTO,
                    CostantiPersonalizzate.LAVCOD_FORMAZIONE_ARGINELLI,
                    CostantiPersonalizzate.LAVCOD_IMBALLO_FIENO_ROTOLI,
                    CostantiPersonalizzate.LAVCOD_INTERRAMENTO_PAGLIE,
                    CostantiPersonalizzate.LAVCOD_LAVORAZIONE_TRA_FILA,
                    CostantiPersonalizzate.LAVCOD_LAVORAZIONE_SU_FILA,
                    CostantiPersonalizzate.LAVCOD_LEGATURA,
                    CostantiPersonalizzate.LAVCOD_LIVELLAMENTO,
                    CostantiPersonalizzate.LAVCOD_MANUTENZIONE_ARGINI,
                    CostantiPersonalizzate.LAVCOD_MESSA_DIMORA_PIANTE,
                    CostantiPersonalizzate.LAVCOD_PACCIAMATURA,
                    CostantiPersonalizzate.LAVCOD_PRESSATURA,
                    CostantiPersonalizzate.LAVCOD_RACCOLTA_LEGNA_POTATURA,
                    CostantiPersonalizzate.LAVCOD_RINCALZATURA,
                    CostantiPersonalizzate.LAVCOD_RIPPATURA,
                    CostantiPersonalizzate.LAVCOD_RIPUNTATURA,
                    CostantiPersonalizzate.LAVCOD_RIVOLTAMENTO_FORAGGIO,
                    CostantiPersonalizzate.LAVCOD_RULLATURA,
                    CostantiPersonalizzate.LAVCOD_SARCHIATURA,
                    CostantiPersonalizzate.LAVCOD_SCARIFICATURA,
                    CostantiPersonalizzate.LAVCOD_SCASSO,
                    CostantiPersonalizzate.LAVCOD_VANGATURA,
                    CostantiPersonalizzate.LAVCOD_ZAPPATURA,
                    CostantiPersonalizzate.LAVCOD_GEBIATURA,
                    CostantiPersonalizzate.LAVCOD_ROMPICROSTA,
                    CostantiPersonalizzate.LAVCOD_LAVORAZIONE_CONBINATA,
                    CostantiPersonalizzate.LAVCOD_ERPICATURA_ROTANTE,
                    CostantiPersonalizzate.LAVCOD_INTERVENTO_ANTIBRINA,
                    CostantiPersonalizzate.LAVCOD_STRIGLIATURA,
                    CostantiPersonalizzate.LAVCOD_ABBATTIMENTOIMPIANTI
                })
                '''TODO: PEr il moemnto le seguenti lavorazioni non sono ricettabili.
                '''Da capire come aggiungerle nella prossima sprint.
                'CostantiPersonalizzate.LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO,
                'CostantiPersonalizzate.LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO,

                '13, 'Concia del seme --> DT: non più sincronizzata, decisione di novembre 2024

            Case enum_Dati_App.AttivitaNewAgri
                lstLavCod.AddRange({
                    CostantiPersonalizzate.LAVCOD_DISTRIBUZIONE_AMMENDANTI
                })

        End Select

        Return lstLavCod

    End Function

    Public Sub ImportAttivitaMulti(tipo As enum_Dati_App,
                                   cuaa As String,
                                   attivitaMulti As List(Of AgronicaCoreDTOStd.InData.Demetra.Attivita),
                                   ByRef errorMessage As String,
                                   objParametri_Super_Server As AgronicaCoreParametri,
                                   objParametri_Server As AgronicaCoreParametri,
                                   objParametri_Utenti As AgronicaCoreParametri,
                                   user_Agent As String,
                                   importDemetraJson As String,
                                   ByRef erroreNonBloccante As String)

        Dim cancellato As Boolean = False
        Dim aggiornamento As Boolean = False
        Dim unid As String = ""
        Dim agronicaLogInvioChiamateWrite As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W
        Dim codice As String = ""
        Dim versione As String = ""

        Try

            If Not DictOrigineAttivitaToImport.ContainsKey(tipo) Then
                errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.TipoImportNonGestito, String.Join(",", DictOrigineAttivitaToImport.Keys))
                Exit Sub
            End If

            Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim piva = xImpCodR.Piva_from_CUAA(cuaa, objParametri_Server)

            If piva = "" Then
                errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.CuaaNonEsistente, cuaa)
                Exit Sub
            End If

            Dim importAgenda As String = ""
            Dim objAppHelper As New AppHelper
            Dim importazione = False

            'Per le attivita di NewAgri (PUA) le importo direttamente sul Brogliaccio senza fermarmi solo su frontiera (APP_Dati)
            If tipo = enum_Dati_App.AttivitaNewAgri Then
                importazione = True
            Else

                Dim importazioneDatiApp = objAppHelper.Leggi_ImportDati_APP(tipo, objParametri_Utenti, importAgenda)

                Select Case importazioneDatiApp
                    Case enum_Import_App.Nessuno 'si ferma su frontiera
                        importazione = False

                    Case enum_Import_App.Parziale 'TODO_DT: gsb, fare???
                        importazione = False

                    Case enum_Import_App.Completo 'arriva su brogliaccio, in base a importAgenda anche su agenda (tutti o in base al lav_cod)
                        importazione = True

                End Select

            End If

            Dim riferimento As String = ""

            'Da Demetra non possono arrivare operazioni multicentro perchè c'è un solo centro

            'In caso di update, le operazioni già importate sul db server vengono cancellate e reinserite, pertanto non c'è necessita di preservarne id_agenda e raccoglitore

            'L'update può riguardare solo agende il cui Master è Demetra, se sono già state editate lato Gias, Demetra non può più inviarle

            Dim codiceRaccoglitore As Integer = 0
            If attivitaMulti.Count > 1 Then
                Dim objSequenze As New Agro_Sequenze
                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                codiceRaccoglitore = objSequenze.NuovoId_Tabella("raccoglitore", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
                'codiceRaccoglitore = objSequenze.Agronica_SequenzaTabelle_NuovoID("raccoglitore", objParametri_Server)
            End If

            Dim listaAttivita As New List(Of AgronicaCoreModelsSTD.attivita.Attivita)

            Dim utente_ultima_modifica As String = ""

            Dim lstLavCod = GetListaAttivitaAmmissibili(tipo)

            Dim listLavCodMultipli As New List(Of Integer)
            For Each attivitaDemetra In attivitaMulti

                If Not attivitaDemetra.flag_cancellazione AndAlso Not lstLavCod.Contains(attivitaDemetra.tipo_operazione) Then
                    errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.AttivitaNonGestita, attivitaDemetra.tipo_operazione)
                    Exit Sub
                End If

                listLavCodMultipli.Add(attivitaDemetra.tipo_operazione)
            Next

            If listLavCodMultipli.Count > 1 Then
                Dim objOperazione As New Operazioni_Combinazioni_R
                Dim isAmmissibile = objOperazione.isAmmissibile(listLavCodMultipli.ToArray, objParametri_Server)
                If Not isAmmissibile Then
                    errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.AttivitaNonCompatibili, String.Join(",", listLavCodMultipli.ToArray))
                    Exit Sub
                End If
            End If

#Region "Controllo per specie"
            'Dim listaPlotId As New List(Of String)
            'For Each attivitaDemetra In attivitaMulti
            '    If attivitaDemetra.impianti IsNot Nothing Then
            '        For Each impiantoDemetra In attivitaDemetra.impianti
            '            If Not listaPlotId.Contains(impiantoDemetra.plot_id) Then
            '                listaPlotId.Add(impiantoDemetra.plot_id)
            '            End If
            '        Next
            '    End If
            'Next

            'If listaPlotId.Any() Then
            '    Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

            '    Dim dtSpecie = objImpianti.LeggiDistinctSpecieImpiantiConCodice(enum_CodiceAnagrafe_Clienti.Demetra, listaPlotId, objParametri_Server)

            '    If dtSpecie IsNot Nothing Then

            '        If dtSpecie.Rows.Count > 1 Then
            '            errorMessage = "L'operazione non può essere accettata perché contiene più di una specie"
            '            Exit Sub
            '        End If

            '        If dtSpecie.Rows.Count > 0 AndAlso CInt(dtSpecie.Rows(0)("Veg_Cod")) = 0 Then
            '            errorMessage = "Nessuna specie per l'impianto fornito"
            '            Exit Sub
            '        End If
            '    End If
            'End If
#End Region

            Dim isFirst As Boolean = True

            Dim ricetta_cod_esistente = 0
            Dim ricetta_operazione_cod_esistente = 0

            For Each attivitaDemetra In attivitaMulti

                If attivitaDemetra.codice = "" Then
                    errorMessage = My.Resources.AgronicaCoreDemetraBIZ.CodiceAttivitaObbligatorio
                    Exit Sub
                End If

                If Not String.IsNullOrEmpty(attivitaDemetra.codice_esterno) Then
                    erroreNonBloccante = String.Format(My.Resources.AgronicaCoreDemetraBIZ.AttivitaNonModificabile, attivitaDemetra.codice, attivitaDemetra.codice_esterno)
                    Exit Sub
                End If

                If (tipo = enum_Dati_App.AttivitaDemetra OrElse tipo = enum_Dati_App.RicetteDemetra) AndAlso String.IsNullOrWhiteSpace(attivitaDemetra.versione) Then
                    errorMessage = "È necessario fornire una versione per le attività"
                    Exit Sub
                End If

                If isFirst Then
                    If (tipo = enum_Dati_App.AttivitaDemetra OrElse tipo = enum_Dati_App.RicetteDemetra) Then
                        versione = attivitaDemetra.versione
                    End If
                    codice = attivitaDemetra.codice
                    cancellato = attivitaDemetra.flag_cancellazione
                    aggiornamento = False
                    riferimento = ""
                    utente_ultima_modifica = attivitaDemetra.utente_ultima_modifica

                    Dim idEVersione As (id As String, versione As String) = ("", "")

                    If tipo = enum_Dati_App.AttivitaDemetra Then
                        idEVersione = objAppHelper.Leggi_GUID_VERSIONE_APP(codice, $"{CInt(enum_Dati_App.Attivita)}, {CInt(enum_Dati_App.AttivitaDemetra)}", objParametri_Server)
                    ElseIf tipo = enum_Dati_App.RicetteDemetra Then
                        idEVersione = objAppHelper.Leggi_GUID_VERSIONE_APP(codice, $"{CInt(enum_Dati_App.Ricette)}, {CInt(enum_Dati_App.RicetteDemetra)}", objParametri_Server)
                    Else
                        idEVersione = objAppHelper.Leggi_GUID_VERSIONE_APP(codice, tipo, objParametri_Server)
                    End If

                    If attivitaMulti.Count > 1 AndAlso String.IsNullOrWhiteSpace(idEVersione.id) Then ' provo anche con gli altri codici per proteggermi da un eventuale cambio dell'ordine delle attività

                        For index = 1 To attivitaMulti.Count - 1
                            Dim altraAttivita = attivitaMulti(index)
                            If tipo = enum_Dati_App.AttivitaDemetra Then
                                idEVersione = objAppHelper.Leggi_GUID_VERSIONE_APP(altraAttivita.codice, $"{CInt(enum_Dati_App.Attivita)}, {CInt(enum_Dati_App.AttivitaDemetra)}", objParametri_Server)
                            ElseIf tipo = enum_Dati_App.RicetteDemetra Then
                                idEVersione = objAppHelper.Leggi_GUID_VERSIONE_APP(altraAttivita.codice, $"{CInt(enum_Dati_App.Ricette)}, {CInt(enum_Dati_App.RicetteDemetra)}", objParametri_Server)
                            Else
                                idEVersione = objAppHelper.Leggi_GUID_VERSIONE_APP(altraAttivita.codice, tipo, objParametri_Server)
                            End If

                            If Not String.IsNullOrWhiteSpace(idEVersione.id) Then
                                Exit For
                            End If
                        Next
                    End If

                    unid = idEVersione.id

                    If (tipo = enum_Dati_App.AttivitaDemetra OrElse tipo = enum_Dati_App.RicetteDemetra) AndAlso Not String.IsNullOrWhiteSpace(unid) Then 'l'attività esiste già
                        If idEVersione.versione <> attivitaDemetra.versione Then
                            erroreNonBloccante = "Attività non modificabile perché l'attività è già stata modificata"
                            Exit Sub
                        End If
                    End If

                    If (tipo = enum_Dati_App.AttivitaDemetra OrElse tipo = enum_Dati_App.RicetteDemetra) AndAlso String.IsNullOrWhiteSpace(unid) AndAlso Not String.IsNullOrWhiteSpace(attivitaDemetra.subcodice_esterno) Then

                        idEVersione = objAppHelper.LeggiGuidEVersioneAppDati(attivitaDemetra.subcodice_esterno, objParametri_Server)

                        If Not String.IsNullOrWhiteSpace(idEVersione.id) Then
                            If idEVersione.versione <> attivitaDemetra.versione Then
                                erroreNonBloccante = "Attività non modificabile perché l'attività è già stata modificata"
                                Exit Sub
                            End If

                            unid = attivitaDemetra.subcodice_esterno
                        End If

                    End If

                    If String.IsNullOrEmpty(unid) Then
                        unid = Guid.NewGuid().ToString()
                    Else
                        Dim appDatiCancellato As Boolean = False
                        Dim tipoEsistente As enum_Dati_App
                        riferimento = objAppHelper.Leggi_Riferimento_APP(unid, tipoEsistente, objParametri_Server, appDatiCancellato)
                        If (appDatiCancellato) Then
                            erroreNonBloccante = $"L'attività è già stata cancellata"
                            Exit Sub
                        End If

                        aggiornamento = True

                        ' riferimento a agenda|brogliaccio
                        Dim Id_Agenda As Integer = 0

                        Dim rif As String() = riferimento.Split("|")
                        If rif.Length > 0 AndAlso rif(0) <> "" Then
                            Id_Agenda = CInt(rif(0))

                            If Id_Agenda > 0 Then
                                erroreNonBloccante = String.Format(My.Resources.AgronicaCoreDemetraBIZ.AttivitaNonModificabile, attivitaDemetra.codice, Id_Agenda)
                                Exit Sub
                            End If

                        End If

                        If rif.Length > 1 AndAlso rif(1) <> "" Then

                            If tipoEsistente = enum_Dati_App.Attivita OrElse tipoEsistente = enum_Dati_App.Ricette Then

                                Dim ricetta_operazione_cod = CInt(rif(1))
                                ricetta_operazione_cod_esistente = ricetta_operazione_cod

                                If ricetta_operazione_cod > 0 Then
                                    Dim agronicaLogRicette_R As New AgronicaLogRicette_R
                                    Dim dt = agronicaLogRicette_R.Leggi_UltimaOperazione("", enum_TipoOperazioneDB.Cancellazione,
                                                     "Ricette_Operazioni", ricetta_operazione_cod.ToString(), "", "", "",
                                                     "", "", "", 0,
                                                     0, "", "", objParametri_Server)
                                    If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                                        errorMessage = $"L'attività è già stata cancellata"
                                        Exit Sub
                                    End If
                                End If

                            ElseIf tipoEsistente = enum_Dati_App.AttivitaDemetra OrElse tipoEsistente = enum_Dati_App.RicetteDemetra Then

                                Dim ricetta_cod = CInt(rif(1))
                                ricetta_cod_esistente = ricetta_cod

                                If ricetta_cod > 0 Then
                                    Dim agronicaLogRicette_R As New AgronicaLogRicette_R
                                    Dim dt = agronicaLogRicette_R.Leggi_UltimaOperazione("", enum_TipoOperazioneDB.Cancellazione,
                                                                                         "Ricette_Operazioni", "", ricetta_cod.ToString(), "", "",
                                                                                         "", "", "", 0,
                                                                                         0, "", "", objParametri_Server)
                                    If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                                        errorMessage = $"L'attività è già stata cancellata"
                                        Exit Sub
                                    End If
                                End If

                            End If

                        End If
                    End If

                    isFirst = False

                End If

                Dim attivita As AgronicaCoreModelsSTD.attivita.Attivita = Nothing

                If Not attivitaDemetra.flag_cancellazione Then
                    attivita = MappaAttivitaDemetraToAttivita(tipo, piva, attivitaDemetra, codiceRaccoglitore, errorMessage, objParametri_Server)
                    If errorMessage <> "" Then
                        Exit Sub
                    End If
                End If

                listaAttivita.Add(attivita)

            Next

            Dim dati As String = JsonConvert.SerializeObject(listaAttivita)

            Dim objDecodificaUtenti = New decodificaUtenti
            Dim utentePrecedente = objParametri_Server.UsernameOperazione
            objParametri_Server.UsernameOperazione = objDecodificaUtenti.decoficaUtenteGiasDaUtenteDemetra(utente_ultima_modifica, cuaa, objParametri_Utenti)

            Dim origine As enum_SistemiEsterni
            If tipo = enum_Dati_App.AttivitaDemetra OrElse tipo = enum_Dati_App.RicetteDemetra Then
                origine = enum_SistemiEsterni.demetra
            ElseIf tipo = enum_Dati_App.AttivitaNewAgri Then
                origine = enum_SistemiEsterni.NewAgri
            Else
                origine = enum_SistemiEsterni.gias
            End If

            Dim riferimentoPianificata = If(listaAttivita(0).riferimentoPianificata, "")
            Dim objSincroHelper As New AgronicaCoreMapper.SincroAppHelper(objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            objSincroHelper.SincroDatiApp(unid, tipo, dati, riferimento, aggiornamento, cancellato, importazione, piva, codice, agenda:=importAgenda, userAgent:=user_Agent,
                                          versione:=versione, origine:=origine, riferimentoPianificata:=riferimentoPianificata)

            If origine = enum_SistemiEsterni.demetra AndAlso Not cancellato Then 'solo se origine demetra
                Dim macchine = listaAttivita.First().risorse.OfType(Of RisorsaMacchina).ToList()
                Dim operatori = listaAttivita.First().risorse.OfType(Of RisorsaPersona).ToList()

                If macchine.Any() OrElse operatori.Any() Then
                    Dim ricetteOperazioneR As New Ricette_Operazioni_R
                    Dim dtRicettaOperazioneCod = ricetteOperazioneR.LeggiRicettaOperazioneCodConGuid(unid, objParametri_Server)

                    If dtRicettaOperazioneCod IsNot Nothing AndAlso dtRicettaOperazioneCod.Rows.Count > 0 Then

                        Dim ricette_dettagli_W As New Ricette_Dettagli_W
                        For Each dr As DataRow In dtRicettaOperazioneCod.Rows
                            Dim ricettaOperazioneCod = CInt(dr("Ricetta_Operazione_Cod"))

                            For Each macchina As RisorsaMacchina In macchine
                                ricette_dettagli_W.AggiornaOreMacchina(ricettaOperazioneCod, macchina.macchina.codice, CDec(macchina.totaleOre), objParametri_Server)
                            Next

                            For Each operatore As RisorsaPersona In operatori
                                ricette_dettagli_W.AggiornaOreOperatore(ricettaOperazioneCod, operatore.risorsaUmana.codice, CDec(operatore.totaleOre), objParametri_Server)
                            Next
                        Next
                    End If

                End If

            End If

            If cancellato AndAlso ricetta_cod_esistente = 0 AndAlso ricetta_operazione_cod_esistente <> 0 Then 'significa che è stata cancellata da Demetra un'attività proveniente da app
                Dim agronicaLogRicette_R As New AgronicaLogRicette_R
                Dim dtLog = agronicaLogRicette_R.Leggi_UltimaOperazione("", enum_TipoOperazioneDB.Cancellazione,
                                 "Ricette_Operazioni", ricetta_operazione_cod_esistente.ToString(), "", "", "",
                                 "", "", "", 0,
                                 0, "", "", objParametri_Server)

                If dtLog IsNot Nothing AndAlso dtLog.Rows.Count > 0 Then

                    ricetta_cod_esistente = CInt(dtLog.Rows(0)("Param1"))
                    Dim rifCorretto = $"0|{ricetta_cod_esistente}"

                    Dim appDatiScrivi As New APP_Dati_W
                    appDatiScrivi.Aggiorna_Riferimento_DatiAPP(unid, rifCorretto, objParametri_Server)

                End If
            End If

            If tipo = enum_Dati_App.AttivitaDemetra Then

                Dim tipoOperazioneDb = GetTipoOperazioneDb(cancellato, aggiornamento)
                agronicaLogInvioChiamateWrite.Scrivi_Log_Invio_Chiamate(enum_Esportazioni_Sistema_Cod.Demetra_Import_Attivita, importDemetraJson, Date.Now,
                                                                        0, tipoOperazioneDb, "OK", "", objParametri_Server,
                                                                        unid, codice, versione)
            ElseIf tipo = enum_Dati_App.RicetteDemetra Then

                Dim tipoOperazioneDb = GetTipoOperazioneDb(cancellato, aggiornamento)
                agronicaLogInvioChiamateWrite.Scrivi_Log_Invio_Chiamate(enum_Esportazioni_Sistema_Cod.Demetra_Import_Ricette, importDemetraJson, Date.Now,
                                                                        0, tipoOperazioneDb, "OK", "", objParametri_Server,
                                                                        unid, codice, versione)
            End If

            objParametri_Server.UsernameOperazione = utentePrecedente

        Catch ex As Exception

            If tipo = enum_Dati_App.AttivitaDemetra Then

                Dim tipoOperazioneDb = GetTipoOperazioneDb(cancellato, aggiornamento)
                agronicaLogInvioChiamateWrite.Scrivi_Log_Invio_Chiamate(enum_Esportazioni_Sistema_Cod.Demetra_Import_Attivita, importDemetraJson, Date.Now,
                                                                        0, tipoOperazioneDb, "KO", ex.Message, objParametri_Server,
                                                                        unid, codice, versione)
            ElseIf tipo = enum_Dati_App.RicetteDemetra Then

                Dim tipoOperazioneDb = GetTipoOperazioneDb(cancellato, aggiornamento)
                agronicaLogInvioChiamateWrite.Scrivi_Log_Invio_Chiamate(enum_Esportazioni_Sistema_Cod.Demetra_Import_Ricette, importDemetraJson, Date.Now,
                                                                        0, tipoOperazioneDb, "KO", ex.Message, objParametri_Server,
                                                                        unid, codice, versione)
            End If

            errorMessage = ex.Message
        End Try

    End Sub

    Private Function GetTipoOperazioneDb(cancellato As Boolean, aggiornamento As Boolean) As enum_TipoOperazioneDB

        Dim tipoOperazioneDb As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura

        If cancellato Then
            tipoOperazioneDb = enum_TipoOperazioneDB.Cancellazione
        ElseIf aggiornamento Then
            tipoOperazioneDb = enum_TipoOperazioneDB.Modifica
        End If

        Return tipoOperazioneDb

    End Function

    Private Function MappaAttivitaDemetraToAttivita(tipo As enum_Dati_App, piva As String, attivitaDemetra As AgronicaCoreDTOStd.InData.Demetra.Attivita, codiceRaccoglitore As Integer, ByRef errorMessage As String, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreModelsSTD.attivita.Attivita

        Dim attivita As New AgronicaCoreModelsSTD.attivita.Attivita

        Dim DtFertilizzanti As DataTable = Nothing

        attivita.tipo = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta
        If attivitaDemetra.flag_pianificata Then
            attivita.stato = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire
        Else
            attivita.stato = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Eseguita
            If attivitaDemetra.pianificata IsNot Nothing Then
                If Not String.IsNullOrEmpty(attivitaDemetra.pianificata.codice_esterno) Then
                    attivita.riferimentoPianificata = attivitaDemetra.pianificata.codice_esterno
                ElseIf Not String.IsNullOrEmpty(attivitaDemetra.pianificata.codice) Then
                    Dim appDatiR As New APP_Dati_R
                    Dim dtIds = appDatiR.LeggiIdDaCodice(attivitaDemetra.pianificata.codice, objParametri_Server)
                    If dtIds IsNot Nothing AndAlso dtIds.Rows.Count = 1 Then
                        attivita.riferimentoPianificata = CStr(dtIds.Rows(0)("Id"))
                    End If
                End If
            End If
        End If

        If DictOrigineAttivitaToImport.ContainsKey(tipo) Then
            attivita.origine = DictOrigineAttivitaToImport(tipo)
        End If

        attivita.disciplinare = SetDisciplinare(tipo, attivitaDemetra)
        attivita.raccoglitore = codiceRaccoglitore

        attivita.codice = "0" 'D2G-->inifluente, l'aggancio con l'eventuale attività esistente passa attraverso il codice Demetra

        attivita.cancellato = attivitaDemetra.flag_cancellazione

        'TODO_DT: FARE GESTIIONE A ORIGINE E A BLOCCHI
        attivita.inizio = attivitaDemetra.data.ToLocalTime()
        attivita.oraInizio = attivitaDemetra.data.ToLocalTime()
        attivita.note = attivitaDemetra.note

        Dim ObjOperazione As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim Lav_Des = ObjOperazione.LavDes_from_LavCod(attivitaDemetra.tipo_operazione, objParametri_Server)
        If Lav_Des = "" Then
            errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.TipoOperazioneNonEsistente, attivitaDemetra.tipo_operazione)
            Exit Function
        End If

        attivita.job = New AgronicaCoreModelsSTD.attivita.Lavorazione(attivitaDemetra.tipo_operazione)

        If attivitaDemetra.tipo_operazione = CostantiPersonalizzate.LAVCOD_ALTRE_OPERAZIONI Then
            attivita.attivitaPersonalizzata = New AttivitaPersonalizzata(1) 'DT: per ora fisso, valutare se sincronizzare le attività associate al lav_Cod 162
        End If

        ''''''''''' IMPIANTI
        Dim dictImpianti As Dictionary(Of String, (String, EsercizioCDC)) = New Dictionary(Of String, (String, EsercizioCDC))
        attivita.centriDiCosto = New List(Of CentroDiCosto)
        If attivitaDemetra.impianti IsNot Nothing Then

            For Each impiantoDemetra In attivitaDemetra.impianti

                Dim esercizioCDC As EsercizioCDC = DecodeImpianto(piva, attivita.inizio, impiantoDemetra, errorMessage, objParametri_Server)
                If errorMessage <> "" Then
                    Exit Function
                End If

                attivita.centriDiCosto.Add(esercizioCDC)

                If attivita.centroAziendale Is Nothing Then
                    attivita.centroAziendale = New CentroAziendale(esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK)
                End If

                If dictImpianti.ContainsKey(impiantoDemetra.plot_id) Then
                    errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.ImpiantoDuplicato, impiantoDemetra.plot_id)
                    Exit Function
                Else
                    Dim impiantoKey = esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva & "_" & esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice & "_" & esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice & "_" & esercizioCDC.esercizio.impiantoPK.codice
                    dictImpianti.Add(impiantoDemetra.plot_id, (impiantoKey, esercizioCDC))
                End If

                If attivita.utilizzoTerreno Is Nothing Then
                    attivita.utilizzoTerreno = BuildUtilizzoTerreno(esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                           esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                           esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice,
                                                                           esercizioCDC.esercizio.impiantoPK.codice,
                                                                           verbose:=False,
                                                                           objParametri_Server)

                    If attivita.utilizzoTerreno Is Nothing Then
                        errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.ImpiantoSenzaSpecie, impiantoDemetra.plot_id)
                        Exit Function
                    End If
                End If
            Next

            attivita.risorse = New List(Of Risorsa)
        End If

        Dim InfoOperazione As InfoOperazione = GetInfoOperazione(attivitaDemetra.tipo_operazione, attivita.Tipo_Attivita.QuadernoDiCampagna)

        ''''''''''' ACQUA
        If InfoOperazione.IsTrattamento OrElse InfoOperazione.IsFertilizzazione Then
            Dim risorsaAcqua = New RisorsaAcqua
            If attivitaDemetra.acqua IsNot Nothing Then

                risorsaAcqua.acqua = attivitaDemetra.acqua.quantita

                If attivitaDemetra.acqua.tipo = TipoAcqua.hl_totale Then
                    risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.TOTALE
                Else
                    risorsaAcqua.acqua = attivitaDemetra.acqua.quantita
                    risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.HA

                End If
            Else
                risorsaAcqua.acqua = 0
                risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.TOTALE
            End If
            attivita.risorse.Add(risorsaAcqua)
        End If

        ''''''''''' PRODOTTI
        If attivitaDemetra.prodotti IsNot Nothing AndAlso attivitaDemetra.prodotti.Count > 0 Then

            For Each prodottoDemetra In attivitaDemetra.prodotti

                If InfoOperazione.IsTrattamento Then
                    Dim dettaglioTrattamento As New DettaglioTrattamento

                    SetDatiProdotto(dettaglioTrattamento, attivitaDemetra, prodottoDemetra, InfoOperazione, errorMessage, objParametri_Server)
                    If errorMessage <> "" Then
                        Exit Function
                    End If

                    ''''''''''' AVVERSITA'
                    If attivitaDemetra.tipo_operazione = CostantiPersonalizzate.LAVCOD_TRATTAMENTO_ANTIPARASSITARIO OrElse
                       attivitaDemetra.tipo_operazione = CostantiPersonalizzate.LAVCOD_DISERBO OrElse
                       attivitaDemetra.tipo_operazione = CostantiPersonalizzate.LAVCOD_GEODISINFESTAZIONE OrElse
                       attivitaDemetra.tipo_operazione = CostantiPersonalizzate.LAVCOD_CONCIA_SEME OrElse
                       attivitaDemetra.tipo_operazione = CostantiPersonalizzate.LAVCOD_DISTRIBUZIONE_INSETTI OrElse
                       attivitaDemetra.tipo_operazione = CostantiPersonalizzate.LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE Then

                        Dim av_cod As Integer = 0
                        Dim av_gru As Integer = 0
                        If prodottoDemetra.avversita IsNot Nothing Then
                            Select Case prodottoDemetra.avversita.tipo
                                Case TipoAvversita.avversita
                                    av_cod = prodottoDemetra.avversita.codice
                                    av_gru = 0

                                Case TipoAvversita.gruppoAvversita
                                    av_cod = 0
                                    av_gru = prodottoDemetra.avversita.codice

                                Case Else
                                    errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.TipoAvversitaNonEsistente, prodottoDemetra.avversita.tipo)
                                    Exit Function
                            End Select
                        End If

                        dettaglioTrattamento.avversitaGruppo = GetAvversitaGruppo(av_cod, av_gru) 'i codici negativi per coadiuvanti etc vengono attribuiti in fase di ribaltamento
                    End If


                    attivita.risorse.Add(dettaglioTrattamento)

                ElseIf InfoOperazione.IsFertilizzazione Then
                    Dim dettaglioFertilizzazione As New DettaglioFertilizzazione

                    SetDatiProdotto(dettaglioFertilizzazione, attivitaDemetra, prodottoDemetra, InfoOperazione, errorMessage, objParametri_Server)
                    If errorMessage <> "" Then
                        Exit Function
                    End If

                    dettaglioFertilizzazione.Cu = prodottoDemetra.Cu

                    dettaglioFertilizzazione.N = 0

                    'Ricalcolo l'N se non mi arriva da NewAgri
                    If tipo = enum_Dati_App.AttivitaNewAgri AndAlso prodottoDemetra.N <= 0 Then

                        If IsNothing(DtFertilizzanti) Then
                            Dim objMetaschemaDAL As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R

                            DtFertilizzanti = objMetaschemaDAL.Leggi(0, "", CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAFINE,
                                                                     enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                        End If

                        If Not IsNothing(DtFertilizzanti) Then
                            Dim Dr = DtFertilizzanti.Select("Fer_Cod = " & prodottoDemetra.codice)

                            If Not IsNothing(Dr) AndAlso Dr.Length = 1 Then
                                dettaglioFertilizzazione.N = Dr(0).Item("N")
                            End If
                        End If

                    Else
                        dettaglioFertilizzazione.N = prodottoDemetra.N
                    End If


                    dettaglioFertilizzazione.P = prodottoDemetra.P
                    dettaglioFertilizzazione.K = prodottoDemetra.K
                    'dettaglioFertilizzazione.Mg 'TODO_DT: valutare se farselo passare
                    dettaglioFertilizzazione.efficienza = 1 'TODO_DT: valutare se farselo passare

                    attivita.risorse.Add(dettaglioFertilizzazione)

                ElseIf InfoOperazione.IsSemina Then
                    Dim dettaglioSemina As New DettaglioSemina

                    SetDatiProdotto(dettaglioSemina, attivitaDemetra, prodottoDemetra, InfoOperazione, errorMessage, objParametri_Server)
                    If errorMessage <> "" Then
                        Exit Function
                    End If

                    attivita.risorse.Add(dettaglioSemina)

                End If
            Next
        Else
            If InfoOperazione.IsTrattamento Then
                Dim dettaglioTrattamento As New DettaglioTrattamento
                dettaglioTrattamento.prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto(-1, InfoOperazione.Elem_Cod)
                dettaglioTrattamento.unitaDiMisuraIndicata = New UnitaDiMisura(enum_UnitaMisura.KG)
                attivita.risorse.Add(dettaglioTrattamento)
            ElseIf InfoOperazione.IsFertilizzazione Then
                Dim dettaglioFertilizzazione As New DettaglioFertilizzazione
                dettaglioFertilizzazione.prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto(-1, InfoOperazione.Elem_Cod)
                dettaglioFertilizzazione.unitaDiMisuraIndicata = New UnitaDiMisura(enum_UnitaMisura.KG)
                attivita.risorse.Add(dettaglioFertilizzazione)
            End If
        End If

        ''''''''''' RACCOLTI
        If attivitaDemetra.raccolti IsNot Nothing Then

            If InfoOperazione.IsRaccolta Then

                attivitaDemetra.RaggruppaRaccolti(creaPlot:=True, lottoDistinti:=True)

                'key: prodotto magazzino lotto
                Dim dictRaccolta As New Dictionary(Of (String, String, String), List(Of QuantitaSuImpianto))

                Dim lstQtaSuImpianto As New List(Of QuantitaSuImpianto)
                For Each raccoltoDemetra In attivitaDemetra.raccolti

                    Dim udmDemetra As UnitaDiMisura = Nothing

                    Dim udmFound As Boolean = False

                    Select Case raccoltoDemetra.udm
                        Case UdmProdotto.chilogrammi
                            udmDemetra = New UnitaDiMisura(enum_UnitaMisura.KG)
                            udmFound = True

                        Case UdmProdotto.numero
                            udmDemetra = New UnitaDiMisura(enum_UnitaMisura.Numero)
                            udmFound = True

                    End Select

                    If Not udmFound Then
                        errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.UdmNonValida, raccoltoDemetra.udm)
                        Exit Function
                    End If

                    Dim prodottoDemetra = New Prodotto(raccoltoDemetra.codice_esterno, InfoOperazione.Elem_Cod) 'DT: i trasformati vegetali nascono solo su GIAS, quindi sempre e solo codice_esterno valorizzato
                    prodottoDemetra.unitaDiMisura = udmDemetra

                    Dim foundProdottoPerSpecie = False
                    Dim specie = GetSpecieFromUtilizzoTerreno(attivita.utilizzoTerreno)

                    Dim objMaterie As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                    Dim dtMaterie As DataTable = objMaterie.Leggi_da_MatCod_senzaFiltroVisibilita(prodottoDemetra.elemCod, prodottoDemetra.codice, "", objParametri_Server)

                    If dtMaterie IsNot Nothing AndAlso dtMaterie.Rows.Count > 0 Then
                        For Each drMateria In dtMaterie.Rows
                            If drMateria.Item("Veg_cod") = specie.codice Then
                                foundProdottoPerSpecie = True
                                Exit For
                            End If
                        Next
                    End If

                    If foundProdottoPerSpecie Then

                        Dim magazzinoDemetra = DecodeFabbricato(raccoltoDemetra.magazzino, errorMessage, objParametri_Server)
                        If errorMessage <> "" Then
                            Exit Function
                        End If
                        Dim magazzinoDemetraKey = ""
                        If magazzinoDemetra IsNot Nothing Then
                            magazzinoDemetraKey = magazzinoDemetra.primaryKey.centroAziendalePK.partitaIva & "_" & magazzinoDemetra.primaryKey.centroAziendalePK.codice & "_" & magazzinoDemetra.primaryKey.codice
                        End If

                        Dim lottoDemetra As String = If(raccoltoDemetra.magazzino IsNot Nothing, raccoltoDemetra.magazzino.lotto, "")

                        If Not dictImpianti.ContainsKey(raccoltoDemetra.plot_id) Then
                            errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.ImpiantoNonPresenteInAttivita, raccoltoDemetra.plot_id, prodottoDemetra.codice)
                            Exit Function
                        End If

                        Dim newQtaSuImpianto = New QuantitaSuImpianto
                        newQtaSuImpianto.esercizioCDC = dictImpianti(raccoltoDemetra.plot_id).Item2
                        newQtaSuImpianto.Lotto = lottoDemetra
                        newQtaSuImpianto.Magazzino = magazzinoDemetra
                        newQtaSuImpianto.Prodotto = prodottoDemetra
                        newQtaSuImpianto.Qta = raccoltoDemetra.quantita

                        If Not dictRaccolta.ContainsKey((prodottoDemetra.codice, magazzinoDemetraKey, lottoDemetra)) Then

                            If Not String.IsNullOrEmpty(lottoDemetra) Then

                                For Each raccoltaKey In dictRaccolta.Keys
                                    If raccoltaKey.Item1 = prodottoDemetra.codice AndAlso raccoltaKey.Item3 = lottoDemetra Then
                                        If raccoltaKey.Item2 <> magazzinoDemetraKey Then
                                            errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.LottoDuplicato, lottoDemetra, prodottoDemetra.codice)
                                            Exit Function
                                        End If
                                    End If
                                Next

                            End If


                            dictRaccolta.Add((prodottoDemetra.codice, magazzinoDemetraKey, lottoDemetra), New List(Of QuantitaSuImpianto))
                        End If

                        dictRaccolta((prodottoDemetra.codice, magazzinoDemetraKey, lottoDemetra)).Add(newQtaSuImpianto)
                    End If

                Next

                For Each itemRaccolta In dictRaccolta
                    Dim dettaglioRaccolta As New DettaglioRaccolta

                    dettaglioRaccolta.Opzioni_Raccolta.Ripartizione = If(attivitaDemetra.flag_manuale, Opzioni_Raccolta.enum_Ripartizione_Raccolta.MANUALE, Opzioni_Raccolta.enum_Ripartizione_Raccolta.AUTO_SUPERFICIE)
                    dettaglioRaccolta.Opzioni_Raccolta.GenerazioneLotto = Opzioni_Raccolta.enum_Generazione_Lotto_Raccolta.MANUALE
                    dettaglioRaccolta.Opzioni_Raccolta.Modalita = Opzioni_Raccolta.enum_Modalita_Raccolta.MECCANICA

                    dettaglioRaccolta.QuantitaSuImpianti = New List(Of QuantitaSuImpianto)

                    Dim isFirst = True

                    For Each qtaSuImp In itemRaccolta.Value
                        If isFirst Then

                            isFirst = False

                            dettaglioRaccolta.prodotto = qtaSuImp.Prodotto
                            dettaglioRaccolta.unitaDiMisura = qtaSuImp.Prodotto.unitaDiMisura
                            dettaglioRaccolta.unitaDiMisuraIndicata = qtaSuImp.Prodotto.unitaDiMisura
                            dettaglioRaccolta.quantitaTotaleReale = 0
                            dettaglioRaccolta.flagDoseQuantitaTotale = 10
                            dettaglioRaccolta.MagazziniMovimentazioni = New List(Of RilevamentoDiMagazzino)
                            dettaglioRaccolta.dataIngresso = attivitaDemetra.data

                            If (qtaSuImp.Magazzino IsNot Nothing) Then
                                Dim rilevamentoMagazzino = New RilevamentoDiMagazzino
                                rilevamentoMagazzino.Magazzino = qtaSuImp.Magazzino
                                rilevamentoMagazzino.Lotto = qtaSuImp.Lotto
                                rilevamentoMagazzino.Prodotto = qtaSuImp.Prodotto
                                rilevamentoMagazzino.Qta = 0
                                rilevamentoMagazzino.udm = qtaSuImp.Prodotto.unitaDiMisura

                                dettaglioRaccolta.MagazziniMovimentazioni.Add(rilevamentoMagazzino)

                            End If

                        End If

                        dettaglioRaccolta.quantitaTotaleReale += qtaSuImp.Qta
                        If dettaglioRaccolta.MagazziniMovimentazioni IsNot Nothing AndAlso dettaglioRaccolta.MagazziniMovimentazioni.Count > 0 Then
                            dettaglioRaccolta.MagazziniMovimentazioni(0).Qta += qtaSuImp.Qta
                        End If


                        dettaglioRaccolta.QuantitaSuImpianti.Add(qtaSuImp)
                    Next

                    attivita.risorse.Add(dettaglioRaccolta)
                Next

            End If

        End If

        ''''''''''' IRRIGAZIONI
        If attivitaDemetra.irrigazioni IsNot Nothing Then

            If InfoOperazione.IsIrrigazione Then

                Dim hashImpiantiAttivita As HashSet(Of String) = New HashSet(Of String)

                For Each irrigazioneDemetra In attivitaDemetra.irrigazioni
                    Dim dettaglioIrrigazione As New DettaglioIrrigazione

                    Dim udmFound As Boolean = False
                    Select Case irrigazioneDemetra.udm
                        Case UdmProdotto.millimetri
                            dettaglioIrrigazione.unitaDiMisura = New UnitaDiMisura(enum_UnitaMisura.Millimetri)
                            dettaglioIrrigazione.QtaTotale = irrigazioneDemetra.quantita * irrigazioneDemetra.impianto.superficie_trattata * 10
                            udmFound = True
                        Case UdmProdotto.metriCubiEttaro
                            dettaglioIrrigazione.unitaDiMisura = New UnitaDiMisura(enum_UnitaMisura.METRI3__HA)
                            dettaglioIrrigazione.QtaTotale = irrigazioneDemetra.quantita * irrigazioneDemetra.impianto.superficie_trattata
                            udmFound = True
                    End Select

                    If Not udmFound Then
                        errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.UdmNonValida, irrigazioneDemetra.udm)
                        Exit Function
                    End If


                    dettaglioIrrigazione.tipoIrrigazione = New TipoIrrigazione(irrigazioneDemetra.tipo) 'DT: Demetra usa un sottoinsieme dei nostri codici, usiamo direttamente il valore che ci passano (potrebbe essere anche 0)

                    dettaglioIrrigazione.QtaRilevata = irrigazioneDemetra.quantita

                    If irrigazioneDemetra.frequenza > 0 Then
                        dettaglioIrrigazione.DataInizio = irrigazioneDemetra.inizio
                        dettaglioIrrigazione.DataFine = irrigazioneDemetra.fine
                        dettaglioIrrigazione.Frequenza = irrigazioneDemetra.frequenza
                    Else
                        dettaglioIrrigazione.DataInizio = CostantiPersonalizzate.AGRODATAINIZIO
                        dettaglioIrrigazione.DataFine = CostantiPersonalizzate.AGRODATAFINE
                        dettaglioIrrigazione.Frequenza = 0
                    End If

                    dettaglioIrrigazione.esercizioCDC = DecodeImpianto(piva, attivita.inizio, irrigazioneDemetra.impianto, errorMessage, objParametri_Server)
                    If errorMessage <> "" Then
                        Exit Function
                    End If

                    If hashImpiantiAttivita.Contains(irrigazioneDemetra.impianto.plot_id) Then
                        errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.ImpiantoDuplicato, irrigazioneDemetra.impianto.plot_id)
                        Exit Function
                    Else
                        Dim impiantoKey = dettaglioIrrigazione.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva & "_" & dettaglioIrrigazione.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice & "_" & dettaglioIrrigazione.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice & "_" & dettaglioIrrigazione.esercizioCDC.esercizio.impiantoPK.codice
                        hashImpiantiAttivita.Add(irrigazioneDemetra.impianto.plot_id)
                    End If

                    attivita.centriDiCosto.Add(dettaglioIrrigazione.esercizioCDC)

                    If attivita.centroAziendale Is Nothing Then
                        attivita.centroAziendale = New CentroAziendale(dettaglioIrrigazione.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK)
                    End If

                    If attivita.utilizzoTerreno Is Nothing Then
                        attivita.utilizzoTerreno = BuildUtilizzoTerreno(dettaglioIrrigazione.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                           dettaglioIrrigazione.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                           dettaglioIrrigazione.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice,
                                                                           dettaglioIrrigazione.esercizioCDC.esercizio.impiantoPK.codice,
                                                                           verbose:=False,
                                                                           objParametri_Server)

                        If attivita.utilizzoTerreno Is Nothing Then
                            errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.ImpiantoSenzaSpecie, irrigazioneDemetra.impianto.plot_id)
                            Exit Function
                        End If
                    End If
                    attivita.risorse.Add(dettaglioIrrigazione)
                Next

            End If

        End If

        ''''''''''' MACCHINE
        If attivitaDemetra.macchine IsNot Nothing Then

            For Each macchinaDemetra In attivitaDemetra.macchine
                Dim risorsaMacchina As RisorsaMacchina = DecodeMacchina(macchinaDemetra, attivita.inizio, errorMessage, objParametri_Server)
                If errorMessage <> "" Then
                    Exit Function
                End If

                If risorsaMacchina IsNot Nothing Then
                    attivita.risorse.Add(risorsaMacchina)
                End If

            Next

            If errorMessage <> "" Then
                Exit Function
            End If
        End If

        ''''''''''' OPERATORI
        If attivitaDemetra.operatori IsNot Nothing Then

            For Each operatoreDemetra In attivitaDemetra.operatori
                Dim risorsaPersona As RisorsaPersona = DecodeOperatore(piva, attivita.inizio, operatoreDemetra, errorMessage, objParametri_Server)
                If errorMessage <> "" Then
                    Exit Function
                End If

                If risorsaPersona IsNot Nothing Then
                    attivita.risorse.Add(risorsaPersona)
                End If

            Next

            If errorMessage <> "" Then
                Exit Function
            End If
        End If

        Return attivita

    End Function

    Private Function DecodeImpianto(pIva As String, dataOperazione As DateTime, impiantoDemetra As AgronicaCoreDTOStd.InData.Demetra.Impianto, ByRef errorMessage As String, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As EsercizioCDC

        Dim esercizioCdC As EsercizioCDC = Nothing

        If impiantoDemetra.superficie_trattata <= 0 Then
            errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.SuperficieTrattataMaggiore0, impiantoDemetra.plot_id)
            Return esercizioCdC
        End If

        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dim dtImpianti = objImpianti.Leggi_ConDatiImpianto(pIva, 0, 0, 0, 0, -1, enum_CodiceAnagrafe_Clienti.Demetra, impiantoDemetra.plot_id, "", "", objParametri_Server)

        If dtImpianti IsNot Nothing Then
            If dtImpianti.Rows.Count = 1 Then

                Dim piva_impianto As String = dtImpianti.Rows(0).Item("PIVA")
                Dim sa_cod_impianto As Integer = dtImpianti.Rows(0).Item("sa_cod")
                Dim appezza_impianto As Integer = dtImpianti.Rows(0).Item("appezza")
                Dim id_reg_impianto As Integer = dtImpianti.Rows(0).Item("Id_Reg")
                Dim sup_impianto As Decimal = dtImpianti.Rows(0).Item("Sup_Imp")

                'Check Superficie Trattata
                If impiantoDemetra.superficie_trattata > sup_impianto Then
                    errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.SuperficieTrattataMaggioreDellAnagrafica, impiantoDemetra.superficie_trattata, impiantoDemetra.plot_id, sup_impianto)
                    Return esercizioCdC
                End If

                Dim validita_inizio As DateTime = dtImpianti.Rows(0).Item("Validita_inizio")
                Dim validita_fine As DateTime = dtImpianti.Rows(0).Item("Validita_Fine")

                If validita_inizio > dataOperazione OrElse validita_fine < dataOperazione Then
                    errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.ImpiantoNonValido, impiantoDemetra.plot_id, String.Concat(piva_impianto, "_", sa_cod_impianto, "_", appezza_impianto, "_", id_reg_impianto), dataOperazione.ToString("d"))
                Else
                    Dim objEsercizi As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                    Dim esercizi As DataTable = objEsercizi.LeggiMinimal(piva_impianto, sa_cod_impianto, appezza_impianto, id_reg_impianto, 0, dataOperazione, dataOperazione, "", "", objParametri_Server)

                    Dim progetto_cod As Integer = 0
                    If esercizi IsNot Nothing AndAlso esercizi.Rows.Count > 0 Then
                        progetto_cod = esercizi.Rows(0).Field(Of Integer)("Progetto_Cod")
                    End If

                    Dim centroPK = New CentroAziendale.PK(sa_cod_impianto, piva_impianto)
                    Dim appezzamentoPK = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(appezza_impianto, centroPK)
                    Dim impiantoPK = New Impianto.PK(id_reg_impianto, appezzamentoPK)
                    Dim impianto = New Impianto(impiantoPK)

                    Dim esercizio = New Esercizio(progetto_cod, "") With {
                    .impiantoPK = impiantoPK
                    }

                    esercizioCdC = New EsercizioCDC With {
                        .superficieTrattata = impiantoDemetra.superficie_trattata,
                        .esercizio = esercizio
                    }
                End If

            Else
                If dtImpianti.Rows.Count = 0 Then
                    errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.ImpiantoNonEsistente, impiantoDemetra.plot_id)
                Else '>1
                    errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.ImpiantoNonUnivoco, impiantoDemetra.plot_id)
                End If
            End If
        Else
            errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.ImpiantoNonEsistente, impiantoDemetra.plot_id)
        End If

        Return esercizioCdC

    End Function

    Private Function DecodeFabbricato(magazzinoDemetra As AgronicaCoreDTOStd.InData.Demetra.Magazzino, ByRef errorMessage As String, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Fabbricato

        Dim fabbricato As Fabbricato = Nothing

        If magazzinoDemetra IsNot Nothing Then

            If magazzinoDemetra.codice_esterno <> "" AndAlso magazzinoDemetra.codice_esterno <> "0" Then

                Dim codice_fabbricato_gias = magazzinoDemetra.codice_esterno.Split("_"c)
                If codice_fabbricato_gias Is Nothing OrElse codice_fabbricato_gias.Length <> 3 Then
                    errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.ChiaveGIASFabbricatoNonValida, magazzinoDemetra.codice_esterno)
                    Exit Function
                End If

                Dim piva_fabbricato = codice_fabbricato_gias(0)
                Dim sacod_fabbricato = codice_fabbricato_gias(1)
                Dim cod_fabbricato = codice_fabbricato_gias(2)

                fabbricato = LoadFabbricato(piva_fabbricato, sacod_fabbricato, cod_fabbricato, errorMessage, objParametri_Server)
                If errorMessage <> "" Then
                    Exit Function
                End If

            ElseIf magazzinoDemetra.codice <> "" Then
                Dim objFabbricato As New AgronicaCoreInterscambioBIZ.Interscambio_Fabbricati_R
                Dim dtFabbricati = objFabbricato.Leggi_Tabella_Interscambio_ChiaveEsterna(enum_SistemiEsterni.demetra, magazzinoDemetra.codice, objParametri_Server)

                If dtFabbricati Is Nothing OrElse dtFabbricati.Rows.Count = 0 Then
                    errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.FabbricatoNonEsistentePerChiave, magazzinoDemetra.codice)
                    Exit Function
                ElseIf dtFabbricati.Rows.Count = 1 Then

                    Dim piva_fabbricato = dtFabbricati.Rows(0)("piva")
                    Dim sacod_fabbricato = dtFabbricati.Rows(0)("sa_cod")
                    Dim cod_fabbricato = dtFabbricati.Rows(0)("fabbricato_cod")

                    'esiste in interscambio, verifico che esista ancora su GIAS
                    fabbricato = LoadFabbricato(piva_fabbricato, sacod_fabbricato, cod_fabbricato, errorMessage, objParametri_Server)
                    If errorMessage <> "" Then
                        Exit Function
                    End If

                Else
                    errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.FabbricatoDuplicatoPerChiave, magazzinoDemetra.codice)
                    Exit Function
                End If

            Else
                errorMessage = My.Resources.AgronicaCoreDemetraBIZ.ChiaveNonValorizzata
                Exit Function
            End If

        End If

        Return fabbricato

    End Function

    Private Function DecodeMacchina(macchinaDemetra As AgronicaCoreDTOStd.InData.Demetra.Macchina, dataOperazione As DateTime, ByRef errorMessage As String, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As RisorsaMacchina

        Dim risorsaMacchina As RisorsaMacchina = Nothing

        If macchinaDemetra IsNot Nothing Then

            If macchinaDemetra.codice_esterno <> "" AndAlso macchinaDemetra.codice_esterno <> "0" Then

                Dim codice_macchina_gias = macchinaDemetra.codice_esterno.Split("_"c)
                If codice_macchina_gias Is Nothing OrElse codice_macchina_gias.Length <> 3 Then
                    errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.ChiaveGIASMacchinaNonValida, macchinaDemetra.codice_esterno)
                    Exit Function
                End If

                Dim mac_cod = CInt(codice_macchina_gias(2))

                risorsaMacchina = LoadMacchina(dataOperazione, mac_cod, errorMessage, objParametri_Server)
                If errorMessage <> "" Then
                    Exit Function
                End If

            ElseIf macchinaDemetra.codice <> "" Then

                Dim GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)

                Dim objMacchina As New AgronicaCoreInterscambioBIZ.Interscambio_Parco_Macchine_R
                Dim interscambio = objMacchina.GetInterscambioParcoMacchine(macchinaDemetra.codice, enum_SistemiEsterni.demetra, GiasContext)

                If interscambio Is Nothing Then
                    errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.MacchinaNonEsistentePerChiave, macchinaDemetra.codice)
                    Exit Function
                Else

                    'esiste in interscambio, verifico che esista ancora su GIAS
                    risorsaMacchina = LoadMacchina(dataOperazione, interscambio.Mac_Cod, errorMessage, objParametri_Server)

                    If errorMessage <> "" Then
                        Exit Function
                    End If

                End If

            Else
                errorMessage = My.Resources.AgronicaCoreDemetraBIZ.ChiaveNonValorizzata
                Exit Function
            End If

            risorsaMacchina.inizio = dataOperazione
            risorsaMacchina.fine = dataOperazione.AddHours(macchinaDemetra.ore_lavorate)
            risorsaMacchina.totaleOre = CDbl(macchinaDemetra.ore_lavorate)

        End If

        Return risorsaMacchina

    End Function

    Private Function DecodeOperatore(piva As String, dataOperazione As DateTime, operatoreDemetra As AgronicaCoreDTOStd.InData.Demetra.Operatore, ByRef errorMessage As String, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As RisorsaPersona

        Dim risorsaPersona As RisorsaPersona = Nothing

        If operatoreDemetra IsNot Nothing Then

            If operatoreDemetra.codice_esterno <> "" AndAlso operatoreDemetra.codice_esterno <> "0" Then

                Dim codice_contatto_gias = operatoreDemetra.codice_esterno.Split("_"c)
                If codice_contatto_gias Is Nothing OrElse codice_contatto_gias.Length <> 2 Then
                    errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.ChiaveGIASOperatoreNonValida, operatoreDemetra.codice_esterno)
                    Exit Function
                End If

                Dim piva_contatto = codice_contatto_gias(0)
                Dim cod_contatto = codice_contatto_gias(1)

                Dim risorsaUmana As RisorseUmane = LoadOperatore(piva, piva_contatto, cod_contatto, dataOperazione, errorMessage, objParametri_Server)
                If errorMessage <> "" Then
                    Exit Function
                End If

                risorsaPersona = New RisorsaPersona
                risorsaPersona.risorsaUmana = risorsaUmana

            ElseIf operatoreDemetra.codice <> "" Then

                Dim objOperatore As New AgronicaCoreInterscambioBIZ.Interscambio_Contatti_R
                Dim interscambio = objOperatore.Leggi_Tabella_Interscambio_ChiaveEsterna(enum_SistemiEsterni.demetra, operatoreDemetra.codice, objParametri_Server)

                If interscambio Is Nothing OrElse interscambio.Rows.Count = 0 Then
                    errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.OperatoreNonEsistentePerChiave, operatoreDemetra.codice)
                    Exit Function
                Else

                    'esiste in interscambio, verifico che esista ancora su GIAS

                    Dim risorsaUmana As RisorseUmane = LoadOperatore(piva, interscambio.Rows(0)("Piva"), interscambio.Rows(0)("Cod_Contatto"), dataOperazione, errorMessage, objParametri_Server)
                    If errorMessage <> "" Then
                        Exit Function
                    End If

                    risorsaPersona = New RisorsaPersona
                    risorsaPersona.risorsaUmana = risorsaUmana

                End If

            Else
                errorMessage = My.Resources.AgronicaCoreDemetraBIZ.ChiaveNonValorizzata
                Exit Function
            End If

            risorsaPersona.inizio = dataOperazione
            risorsaPersona.fine = dataOperazione.AddHours(operatoreDemetra.ore_lavorate)
            risorsaPersona.totaleOre = CDbl(operatoreDemetra.ore_lavorate)

        End If

        Return risorsaPersona

    End Function

    Private Function LoadFabbricato(piva As String, sa_cod As Integer, fabbricato_cod As Integer, ByRef errorMessage As String, objParametri_Server As AgronicaCoreParametri) As Fabbricato

        Dim fabbricato As Fabbricato = Nothing

        Dim objFabbricato As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim dtFabbricati = objFabbricato.Leggi(piva, sa_cod, fabbricato_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        If dtFabbricati Is Nothing OrElse dtFabbricati.Rows.Count = 0 Then
            errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.FabbricatoNonEsistentePerChiaveGIAS, piva & "_" & sa_cod & "_" & fabbricato_cod)
            Exit Function

        ElseIf dtFabbricati.Rows.Count = 1 Then
            Dim centroAziendale = New CentroAziendale(New CentroAziendale.PK(sa_cod, piva))
            fabbricato = New Fabbricato With {
                .primaryKey = New Fabbricato.PK With {
                    .centroAziendalePK = centroAziendale.primaryKey,
                    .codice = fabbricato_cod
                }
            }

        Else
            errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.FabbricatoDuplicatoPerChiaveGIAS, piva & "_" & sa_cod & "_" & fabbricato_cod)
            Exit Function
        End If

        Return fabbricato

    End Function

    Private Function LoadMacchina(dataOperazione As DateTime, mac_cod As Integer, ByRef errorMessage As String, objParametri_Server As AgronicaCoreParametri) As RisorsaMacchina

        Dim risorsaMacchina As RisorsaMacchina = Nothing

        Dim objMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R

        Dim dtMacchine = objMacchine.Leggi2("",
                        mac_cod,
                        CostantiPersonalizzate.SACOD_NOFILTRO,
                         0, "", True,
                        "", "",
                        objParametri_Server)


        If dtMacchine IsNot Nothing AndAlso dtMacchine.Rows.Count > 0 Then

            Dim validita_inizio As DateTime = dtMacchine.Rows(0).Item("Validita_inizio")
            Dim validita_fine As DateTime = dtMacchine.Rows(0).Item("Validita_Fine")

            If validita_inizio > dataOperazione OrElse validita_fine < dataOperazione Then
                errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.MacchinaNonValida, mac_cod, dataOperazione.ToString("d"))
                Exit Function
            Else
                risorsaMacchina = New RisorsaMacchina
                risorsaMacchina.macchina = New ParcoMacchine
                risorsaMacchina.macchina.codice = mac_cod
            End If
        Else
            errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.MacchinaNonEsistentePerChiaveGIAS, mac_cod)
            Exit Function
        End If

        Return risorsaMacchina

    End Function

    Private Function LoadOperatore(piva As String, pivaContatto As String, codContatto As String, dataOperazione As DateTime, ByRef errorMessage As String, objParametri_Server As AgronicaCoreParametri) As RisorseUmane

        If piva <> pivaContatto Then
            errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.OperatoreNonAppartenentePivaOperazione, pivaContatto, codContatto, piva)
            Exit Function
        End If

        Dim risorsaUmana As RisorseUmane = Nothing

        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
        If objContatti.Esiste_Contatto(pivaContatto, codContatto, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server) = True Then

            Dim strFiltroAggiuntivo As String = " (( Rapporti_Contabili.Cod_Rapporto in(-1,-4,-6, -5) ) or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Legale=1 or Rapporti_Contabili.Terzista=1) " & vbCrLf &
                                " AND Contatti.Cod_Contatto =  '" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(codContatto) & "' AND Risorse_Umane.Validita_Inizio <= " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDate(dataOperazione) & " AND Risorse_Umane.Validita_Fine >= " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDate(dataOperazione) & " "

            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
            Dim Dt_Manodopera As DataTable = objRapp_Contabili.RapportiContabilixCostiAccessori_Flag_Data_Scadenza_Patentino(pivaContatto,
                                                                                                                         False,
                                                                                                                        False,
                                                                                                                         dataOperazione,
                                                                                                                         False,
                                                                                                                          strFiltroAggiuntivo, " Contatti.Sa_Cod desc, Contatti.Rag_Soc, cognome, nome ASC  ", objParametri_Server)

            'se ci sono più risorse associate al contatto, si prende la prima
            If Dt_Manodopera IsNot Nothing AndAlso Dt_Manodopera.Rows.Count > 0 Then

                risorsaUmana = New RisorseUmane
                risorsaUmana.codice = Dt_Manodopera.Rows(0).Item("Cod_RisUm")

            Else

                errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.OperatoreNonEsistentePerChiaveGIAS, piva, codContatto, dataOperazione)
                Exit Function
            End If

        Else
            errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.OperatoreNonEsistentePerChiaveGIAS, piva, codContatto, dataOperazione)
            Exit Function
        End If

        Return risorsaUmana

    End Function

    Private Sub SetDatiProdotto(dettaglio As RisorsaProdotto, attivitaDemetra As AgronicaCoreDTOStd.InData.Demetra.Attivita, prodottoDemetra As AgronicaCoreDTOStd.InData.Demetra.Prodotto, InfoOperazione As InfoOperazione, ByRef errorMessage As String, objParametri_Server As AgronicaCoreParametri)
        dettaglio.prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto(prodottoDemetra.codice, InfoOperazione.Elem_Cod)
        dettaglio.quantitaTotaleReale = prodottoDemetra.quantita
        dettaglio.flagDoseQuantitaTotale = 10

        ''''''''''' UDM
        Dim udmFound = False
        If InfoOperazione.IsTrattamento OrElse InfoOperazione.IsFertilizzazione Then
            Select Case prodottoDemetra.udm.ToUpper
                Case UdmProdotto.chilogrammi
                    dettaglio.unitaDiMisuraIndicata = New UnitaDiMisura(enum_UnitaMisura.KG)
                    udmFound = True
                Case UdmProdotto.litri
                    dettaglio.unitaDiMisuraIndicata = New UnitaDiMisura(enum_UnitaMisura.Litri)
                    udmFound = True
                Case UdmProdotto.numero
                    If attivitaDemetra.tipo_operazione = CostantiPersonalizzate.LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE Then
                        dettaglio.unitaDiMisuraIndicata = New UnitaDiMisura(enum_UnitaMisura.Numero_Diffusori)
                        udmFound = True
                    ElseIf attivitaDemetra.tipo_operazione = CostantiPersonalizzate.LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA Then
                        dettaglio.unitaDiMisuraIndicata = New UnitaDiMisura(enum_UnitaMisura.Numero_Trappole)
                        udmFound = True
                    End If
                Case UdmProdotto.quintali
                    dettaglio.unitaDiMisuraIndicata = New UnitaDiMisura(enum_UnitaMisura.Quintali)
                    udmFound = True
                Case UdmProdotto.metriCubi
                    dettaglio.unitaDiMisuraIndicata = New UnitaDiMisura(enum_UnitaMisura.Metri_Cubi)
                    udmFound = True
            End Select


        ElseIf InfoOperazione.IsSemina Then
            Select Case prodottoDemetra.udm.ToUpper
                Case UdmProdotto.chilogrammi
                    dettaglio.unitaDiMisuraIndicata = New UnitaDiMisura(enum_UnitaMisura.KG)
                    udmFound = True
                Case UdmProdotto.numero
                    Dim objMateriePrime_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                    Dim DtMateriePrime = objMateriePrime_R.Leggi2("", InfoOperazione.Elem_Cod, prodottoDemetra.codice, "", 0, "", True, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                    dettaglio.unitaDiMisuraIndicata = New UnitaDiMisura(enum_UnitaMisura.Num_Piante)
                    If DtMateriePrime.Rows.Count > 0 Then
                        Dim semCod = DtMateriePrime.Rows(0).Item("Sem_Cod")
                        If semCod = 1 Then
                            dettaglio.unitaDiMisuraIndicata = New UnitaDiMisura(enum_UnitaMisura.Unita_Seme)
                        End If
                    End If
                    udmFound = True
            End Select

        End If

        If Not udmFound Then
            errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.UdmNonValida, prodottoDemetra.udm)
            Exit Sub
        Else
            dettaglio.unitaDiMisura = dettaglio.unitaDiMisuraIndicata
        End If


        ''''''''''' MAGAZZINO
        dettaglio.MagazziniMovimentazioni = New List(Of AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino)
        If prodottoDemetra.magazzino IsNot Nothing Then
            Dim fabbricato = DecodeFabbricato(prodottoDemetra.magazzino, errorMessage, objParametri_Server)
            If errorMessage <> "" Then
                Exit Sub
            End If
            If fabbricato IsNot Nothing Then
                Dim rilevamentoMagazzino = New RilevamentoDiMagazzino
                rilevamentoMagazzino.Magazzino = fabbricato
                rilevamentoMagazzino.Lotto = prodottoDemetra.magazzino.lotto
                rilevamentoMagazzino.Prodotto = New Prodotto(prodottoDemetra.codice, InfoOperazione.Elem_Cod)
                rilevamentoMagazzino.Qta = prodottoDemetra.quantita
                rilevamentoMagazzino.udm = dettaglio.unitaDiMisuraIndicata

                dettaglio.MagazziniMovimentazioni.Add(rilevamentoMagazzino)
            End If
        End If

    End Sub

    Private Function SetDisciplinare(ByVal tipo As enum_Dati_App, ByVal attivitaDemetra As AgronicaCoreDTOStd.InData.Demetra.Attivita) As Disciplinare

        Dim disciplinare As Disciplinare = Nothing

        Select Case tipo
            'Imposto il Disciplinare dal Regolamento_Cod ricevuto
            Case enum_Dati_App.AttivitaNewAgri

                If attivitaDemetra.tipo_operazione = CostantiPersonalizzate.LAVCOD_DISTRIBUZIONE_AMMENDANTI Then

                    disciplinare = New AgronicaCoreModelsSTD.metaschema.Disciplinare() With
                            {
                                .regolamentoConcimazione = New RegolamentoConcimazione(attivitaDemetra.regolamento_cod)
                            }
                End If

            Case Else
                disciplinare = New Disciplinare(0)
        End Select

        Return disciplinare

    End Function

End Class
