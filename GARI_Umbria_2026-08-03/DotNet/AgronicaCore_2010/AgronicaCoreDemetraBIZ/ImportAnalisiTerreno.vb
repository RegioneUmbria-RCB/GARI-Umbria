Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD.analisi
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports AgronicaCoreUtility

Public Enum Tipo_Interazione
    Check = 1
    Scrivi = 2
End Enum

Public Class ImportAnalisiTerreno

    Public Sub ImportAnalisiTerreno(cuaa As String,
                                    lista_analisi As List(Of AgronicaCoreDTOStd.InData.Demetra.AnalisiTerreno),
                                    ByRef errorMessage As String,
                                    objParametri_Super_Server As AgronicaCoreParametri,
                                    objParametri_Server As AgronicaCoreParametri,
                                    objParametri_Utenti As AgronicaCoreParametri,
                                    user_Agent As String)


        Dim erroriList As New List(Of String)
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean

        Try

            '--------------------
            '   CHECK CUAA
            '--------------------
            Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim piva = xImpCodR.Piva_from_CUAA(cuaa, objParametri_Server)

            If piva = "" Then
                errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.CuaaNonEsistente, cuaa)
                Exit Sub
            End If

            Dim _analisiTerrenoDemetra As New AgronicaCoreDTOStd.InData.Demetra.AnalisiTerreno
            Dim _Analisi_Testata_Cod As String = ""
            Dim _codice_Demetra As String = ""

            Dim objAnalisiTerreno_W As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_W
            Dim objInterscambioAnalisi_W As New AgronicaCoreInterscambioBIZ.Interscambio_Analisi_Testata_W
            Dim objInterscambioAnalisi_R As New AgronicaCoreInterscambioBIZ.Interscambio_Analisi_Testata_R

            Dim chiaveSuInterscambio As DataTable
            Dim onUpdate As Boolean = False

            Dim objDecodificaUtenti As New decodificaUtenti

            Dim utente_ultima_modifica As String = ""
            Dim utentePrecedente = objParametri_Server.UsernameOperazione

            Dim analisiNonInviata As String = ""

            Dim datiDaLoggare As New objAnalisiDemetra

            Try
                ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server)

                For check1_scrivi2 = Tipo_Interazione.Check To Tipo_Interazione.Scrivi

                    If check1_scrivi2 = Tipo_Interazione.Scrivi AndAlso erroriList.Count > 0 Then
                        Exit For
                    End If

                    For Each analisiTerrenoDemetra In lista_analisi

                        Dim index = lista_analisi.IndexOf(analisiTerrenoDemetra) + 1
                        analisiNonInviata = String.Format(My.Resources.AgronicaCoreDemetraBIZ.AnalisiIndexErrore_, index)

                        Dim analisiTerreno As New AnalisiTerreno

                        Dim codice_Demetra As String = analisiTerrenoDemetra.codice 'codice Demetra per le chiamate Demetra2Gias
                        Dim Analisi_Testata_Cod As String = analisiTerrenoDemetra.codice_esterno 'codice Gias per le chiamate Demetra2Gias

                        utente_ultima_modifica = analisiTerrenoDemetra.utente_ultima_modifica

                        '-------------------------------
                        '   RIEMPIO LE VARIABILI UN CASO DI EXCEPTION
                        '-------------------------------
                        _analisiTerrenoDemetra = analisiTerrenoDemetra
                        _Analisi_Testata_Cod = Analisi_Testata_Cod
                        _codice_Demetra = codice_Demetra

                        '-------------------------------
                        '   CONTROLLO VALIDITA' CHIAVI
                        '-------------------------------
                        If (codice_Demetra = "" OrElse codice_Demetra = "0") AndAlso
                        (Analisi_Testata_Cod = "" OrElse Analisi_Testata_Cod = "0") Then
                            Dim messaggio As String = analisiNonInviata & My.Resources.AgronicaCoreDemetraBIZ.ChiaviVuote
                            erroriList.Add(messaggio)
                            Continue For
                        End If

                        Dim msgErroreCoerenzaDettagli As String = ""
                        If Not (Check_Coerenza_DettagliAnalisi(analisiTerrenoDemetra, msgErroreCoerenzaDettagli, objParametri_Server)) Then
                            Dim messaggio As String = analisiNonInviata & String.Format(My.Resources.AgronicaCoreDemetraBIZ.ParametriAnalisiInesistentiGIAS, msgErroreCoerenzaDettagli)
                            erroriList.Add(messaggio)
                            Continue For
                        End If

                        '---------------
                        '   CHECK DATI
                        '---------------
                        If analisiTerrenoDemetra.codice_esterno = "" OrElse analisiTerrenoDemetra.codice_esterno = "0" Then
                            'significa che potrebbe essere una nuova Analisi, occorre verificare sulla tabella interscambio
                            chiaveSuInterscambio = objInterscambioAnalisi_R.Leggi_Tabella_Interscambio_ChiaveEsterna(enum_SistemiEsterni.demetra,
                                                                                                                     analisiTerrenoDemetra.codice,
                                                                                                                     objParametri_Server)
                            'chiave esterna doppia
                            If chiaveSuInterscambio.Rows.Count > 1 Then
                                Throw New Exception(String.Format(My.Resources.AgronicaCoreDemetraBIZ.ChiaveEsternaDoppia, analisiTerrenoDemetra.codice))
                            End If

                            If chiaveSuInterscambio.Rows.Count > 0 Then
                                'trovata corrispondenza
                                Analisi_Testata_Cod = chiaveSuInterscambio.Rows(0)("Analisi_Testata_Cod")
                                analisiTerrenoDemetra.codice_esterno = Analisi_Testata_Cod

                                'provo a recuperare l'analisi, il BIZ chiamato risponde con un eccezione se questa non esiste su Gias
                                Try
                                    analisiTerreno = AgronicaCoreAnagrafeBIZ.Analisi_Modello_R.Leggi_AnalisiTerreno_Modello(objParametri_Server.PivaSuperUser, Analisi_Testata_Cod, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                                Catch ex As Exception
                                    Dim messaggio As String = analisiNonInviata & String.Format(My.Resources.AgronicaCoreDemetraBIZ.ChiaveGiasSbagliata, Analisi_Testata_Cod)
                                    erroriList.Add(messaggio)
                                    Continue For
                                End Try

                                onUpdate = True

                            Else
                                'non esistono corrispondenze, devo creare una nuova analisi
                                Analisi_Testata_Cod = 0
                                onUpdate = False

                            End If

                        Else

                            'il codice_esterno (Gias) è stato valorizzato, provo a recuperare l'analisi associata
                            'il BIZ chiamato risponde con un eccezione se questa non esiste su Gias

                            Analisi_Testata_Cod = analisiTerrenoDemetra.codice_esterno

                            Try
                                analisiTerreno = AgronicaCoreAnagrafeBIZ.Analisi_Modello_R.Leggi_AnalisiTerreno_Modello(objParametri_Server.PivaSuperUser, Analisi_Testata_Cod, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                            Catch ex As Exception
                                Dim messaggio As String = analisiNonInviata & String.Format(My.Resources.AgronicaCoreDemetraBIZ.ChiaveGiasSbagliata, Analisi_Testata_Cod)
                                erroriList.Add(messaggio)
                                Continue For
                            End Try

                            onUpdate = True

                        End If

                        If check1_scrivi2 = Tipo_Interazione.Scrivi Then
                            'Ogni analisi che logghiamo su Agronica_Log_Invio_Analisi deve avere il suo pacchetto con il CUAA
                            datiDaLoggare.ListaAnalisiTerreno = New List(Of AgronicaCoreDTOStd.InData.Demetra.AnalisiTerreno)
                            datiDaLoggare.ListaAnalisiTerreno.Add(analisiTerrenoDemetra)

                            '---------------
                            '   PREP DATI
                            '---------------
                            'copio tutte le proprietà dell'analisi inviata da Demetra sul modello Gias
                            'inserisco la chiave dedotta negli step sopra (valorizzata in UPD e 0 in scrittura)
                            analisiTerreno = analisiTerrenoDemetra.PropertyCopier(analisiTerreno)
                            analisiTerreno.codice = Analisi_Testata_Cod

                            'In cancellazione ho solo bisogno di un ID
                            If analisiTerrenoDemetra.flag_cancellazione = False Then
                                'imposto i dati default per l'import Demetra
                                SetData_AnalisiTerreno_Demetra(analisiTerreno, piva, If(analisiTerrenoDemetra.numero_certificato Is Nothing, "", analisiTerrenoDemetra.numero_certificato))
                            End If

                            '---------------------------
                            '  RECUPERO UTENTE MODIFICA
                            '---------------------------
                            objParametri_Server.UsernameOperazione = objDecodificaUtenti.decoficaUtenteGiasDaUtenteDemetra(utente_ultima_modifica, cuaa, objParametri_Utenti)


                            '----------------------
                            '   SCRITTURA ANALISI
                            '----------------------
                            objAnalisiTerreno_W.Scrivi_AnalisiTerreno_Modello(piva, Analisi_Testata_Cod, analisiTerreno, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, FlagConnessioneLocale:=FlagConnessioneLocale, FlagTransazioneLocale:=FlagTransazioneLocale, NoteLog:="Import Demetra", Origine:=enum_SistemiEsterni.demetra)
                            analisiTerrenoDemetra.codice_esterno = Analisi_Testata_Cod


                            '-----------------------------------
                            '   SCRITTURA TABELLA INTERSCAMBIO
                            '-----------------------------------
                            'se NON sono in update, la tabella interscambio è da popolare
                            If onUpdate = False AndAlso analisiTerrenoDemetra.flag_cancellazione = False Then
                                objInterscambioAnalisi_W.Scrivi_Tabella_Interscambio(enum_SistemiEsterni.demetra, Analisi_Testata_Cod, codice_Demetra, objParametri_Server)
                            End If

                            '-----------------------------------
                            '   SCRITTURA LOG INVIO ANALISI 'OK'
                            '-----------------------------------
                            Chiama_Scrivi_Log_Invio_Analisi(cuaa, piva, datiDaLoggare, Analisi_Testata_Cod, codice_Demetra, If(analisiTerrenoDemetra.flag_cancellazione, enum_TipoOperazioneDB.Cancellazione, If(onUpdate, enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Scrittura)), "OK", "", objParametri_Server, UtilizzaTransazione:=False)

                        End If
                    Next
                Next

                ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            Catch ex As Exception

                If objParametri_Server.objTransazione IsNot Nothing Then
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                End If

                Dim messaggio As String = analisiNonInviata & ex.Message
                erroriList.Add(messaggio)


                '-----------------------------------
                '   SCRITTURA LOG INVIO ANALISI 'KO'
                '-----------------------------------
                Chiama_Scrivi_Log_Invio_Analisi(cuaa, piva, datiDaLoggare, _Analisi_Testata_Cod, _codice_Demetra, If(_analisiTerrenoDemetra.flag_cancellazione, enum_TipoOperazioneDB.Cancellazione, If(onUpdate, enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Scrittura)), "KO", ex.Message, objParametri_Server)

            Finally

                ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

            End Try

            If erroriList.Count > 0 Then
                errorMessage = My.Resources.AgronicaCoreDemetraBIZ.ImportazioneAnalisiFallita_ & String.Join(" | ", erroriList)
            End If

            objParametri_Server.UsernameOperazione = utentePrecedente

        Catch ex As Exception
            errorMessage = ex.Message
        End Try

    End Sub

    Private Shared Sub SetData_AnalisiTerreno_Demetra(ByRef analisiTerreno As AnalisiTerreno, piva As String, numero_certificato As String)
        analisiTerreno.AnalisiTipo = New AnalisiTipo(enum_AnalisiTipo.Analisi_Terreno)
        analisiTerreno.analisiTipologia = New AnalisiTipologia(enum_AnalisiTipologia_Schema.Piano_Concimazione)
        analisiTerreno.entitaImprese = New List(Of AnalisiEntita(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))

        analisiTerreno.entitaImprese =
            New List(Of AnalisiEntita(Of AgronicaCoreModelsSTD.anagrafiche.Impresa)) From {
                New AnalisiEntita(Of AgronicaCoreModelsSTD.anagrafiche.Impresa) With {
                    .elementoAnagrafico = New AgronicaCoreModelsSTD.anagrafiche.Impresa With {
                        .partitaIva = piva
                    }
                }
            }

        If analisiTerreno.campioni IsNot Nothing AndAlso analisiTerreno.campioni.Count > 0 Then
            'In caso di modica impostiamo sul primo campione le coordinate che ci hanno passato
            analisiTerreno.campioni.OrderBy(Function(c) c.codice)(0).latitude = analisiTerreno.latitude
            analisiTerreno.campioni.OrderBy(Function(c) c.codice)(0).longitude = analisiTerreno.longitude
        End If

        If analisiTerreno.certificatoAnalisi Is Nothing Then
            analisiTerreno.certificatoAnalisi = New CertificatoAnalisi()
        End If
        analisiTerreno.certificatoAnalisi.numero_certificato = numero_certificato

    End Sub

    Private Shared Function Check_Coerenza_DettagliAnalisi(analisiTerreno As AgronicaCoreDTOStd.InData.Demetra.AnalisiTerreno, ByRef msgErrore As String, objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim ret As Boolean

        Dim objAnalisiParametri As New AgronicaCoreAnagrafeDAL.Analisi_Parametri_R
        Dim listaMessaggi As New List(Of String)

        If analisiTerreno.dettagli IsNot Nothing Then
            For Each dettaglio In analisiTerreno.dettagli
                If dettaglio.valore1 IsNot Nothing Then
                    If Not objAnalisiParametri.Check_ParametroxSchema(dettaglio.parametro.codice, enum_AnalisiTipologia_Schema.Piano_Concimazione, objParametri_Server) Then
                        listaMessaggi.Add(dettaglio.parametro.codice & If(dettaglio.parametro.descrizione <> "", " (" & dettaglio.parametro.descrizione & ")", "") & ";")
                    End If
                End If
            Next
        End If

        If listaMessaggi.Count > 0 Then
            msgErrore = String.Join(" ", listaMessaggi)
            ret = False
        Else
            ret = True
        End If

        Return ret
    End Function

    Private Shared Sub Chiama_Scrivi_Log_Invio_Analisi(CUAA As String, Piva As String, AnalisiTerreno As objAnalisiDemetra, analisi_testata_cod As Integer, chiave_esterna As String, TipoOperazione As enum_TipoOperazioneDB, Esito As String, Dati_Ricevuti As String, objParametri_Server As AgronicaCoreParametri, Optional UtilizzaTransazione As Boolean = True)

        Dim objLogInvioChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W
        Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim pacchettoDaLoggare As New AgronicaCoreDTOStd.InData.importazioni.ImportDemetra With {
            .CUAA = CUAA,
            .dati = JsonConvert.SerializeObject(AnalisiTerreno, tzh)
        }

        Dim strPacchettoDaLoggare = JsonConvert.SerializeObject(pacchettoDaLoggare, tzh)

        objLogInvioChiamate.Scrivi_Log_Invio_Analisi(enum_Esportazioni_Sistema_Cod.Demetra_Import_Analisi, strPacchettoDaLoggare, analisi_testata_cod, chiave_esterna, TipoOperazione, Esito, Dati_Ricevuti, objParametri_Server, UtilizzaTransazione, Piva:=Piva)

    End Sub
End Class


