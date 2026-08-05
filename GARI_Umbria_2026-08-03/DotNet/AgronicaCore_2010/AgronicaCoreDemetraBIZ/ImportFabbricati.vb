Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreInterscambioBIZ
Imports AgronicaCoreModelsSTD.anagrafiche
Imports Newtonsoft.Json

Public Class ImportFabbricati

    Public Sub ImportFabbricato(cuaa As String, datiRequest As AnagraficaWrapper(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato), ByRef errorMessage As String, objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim objFabbricato As New AgronicaCoreModelsSTD.anagrafiche.Fabbricato()
        Dim onUpdate As Boolean = False

        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim centri_R As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            Dim interscambioWBIZ As New Interscambio_Fabbricati_W

            Dim interscambioRBIZ As New Interscambio_Fabbricati_R
            Dim chiaveSuInterscambio As DataTable

            Dim objCentri_W As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W

            Dim objFabbricati_R As New AgronicaCoreAnagrafeBIZ.Fabbricato_R

            Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim piva As String = xImpCodR.Piva_from_CUAA(cuaa, objParametri_Server)

            'da scommentare quando riusciamo a scrivere il fabbricato
            If piva = "" Then
                Throw New Exception(String.Format(My.Resources.AgronicaCoreDemetraBIZ.CuaaNonEsistente, cuaa))
            End If

            If datiRequest.codice = "" AndAlso datiRequest.codice_esterno = "" Then
                Throw New Exception(My.Resources.AgronicaCoreDemetraBIZ.ChiaviVuote)
            End If

            objFabbricato = datiRequest.elemento_anagrafico

            If objFabbricato.particella IsNot Nothing AndAlso objFabbricato.particella.primaryKey IsNot Nothing Then
                'controllo che nella particella siano valorizzati correttamente Sezione e Subalterno
                objFabbricato.particella.primaryKey.Sezione = If(objFabbricato.particella.primaryKey.Sezione = "", 0, objFabbricato.particella.primaryKey.Sezione)
                objFabbricato.particella.primaryKey.Subalterno = If(objFabbricato.particella.primaryKey.Subalterno = "", 0, objFabbricato.particella.primaryKey.Subalterno)
            End If

            If objFabbricato.indirizzo IsNot Nothing Then
                If IsNothing(objFabbricato.indirizzo.via) Then
                    objFabbricato.indirizzo.via = String.Empty
                End If
                If IsNothing(objFabbricato.indirizzo.frazione) Then
                    objFabbricato.indirizzo.frazione = String.Empty
                End If
                If IsNothing(objFabbricato.indirizzo.cap) Then
                    objFabbricato.indirizzo.cap = "00000"
                End If
                If IsNothing(objFabbricato.indirizzo.istatComune) Then
                    objFabbricato.indirizzo.istatComune = New AgronicaCoreModelsSTD.metaschema.Istat()
                End If
                If IsNothing(objFabbricato.indirizzo.istatComune.reg) Then
                    objFabbricato.indirizzo.istatComune.reg = "000"
                End If
                If IsNothing(objFabbricato.indirizzo.istatComune.prov) Then
                    objFabbricato.indirizzo.istatComune.prov = "000"
                End If
                If IsNothing(objFabbricato.indirizzo.istatComune.com) Then
                    objFabbricato.indirizzo.istatComune.com = "000"
                End If
                If IsNothing(objFabbricato.indirizzo.istatComune.cap) Then
                    objFabbricato.indirizzo.istatComune.cap = "00000"
                End If
            End If

                If datiRequest.codice_esterno = "" OrElse datiRequest.codice_esterno = "0" Then
                'significa che potrebbe essere un nuovo fabbricato, occorre verificare sulla tabella interscambi
                chiaveSuInterscambio = interscambioRBIZ.Leggi_Tabella_Interscambio_ChiaveEsterna(enum_SistemiEsterni.demetra,
                                                                                                 datiRequest.codice,
                                                                                                 objParametri_Server)

                'chiave esterna doppia
                If chiaveSuInterscambio.Rows.Count > 1 Then
                    Throw New Exception(String.Format(My.Resources.AgronicaCoreDemetraBIZ.FabbricatoDuplicatoPerChiave, datiRequest.codice))
                End If

                If chiaveSuInterscambio.Rows.Count > 0 Then
                    objFabbricato.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Fabbricato.PK(chiaveSuInterscambio.Rows(0)("Piva"), chiaveSuInterscambio.Rows(0)("Sa_Cod"), chiaveSuInterscambio.Rows(0)("Fabbricato_Cod"))
                    onUpdate = True

                Else

                    'leggo il primo centro aziendale che trovo
                    Dim centri_aziendaliDB = centri_R.Leggi(piva, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                    Dim objCentro As New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale(New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(centri_aziendaliDB(0)("sa_cod"), centri_aziendaliDB(0)("Piva")))
                    objFabbricato.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Fabbricato.PK(piva, objCentro.primaryKey.codice, 0)
                    onUpdate = False

                End If

            Else
                'spacchettare il campo codice esterno con piva-sa_cod-fabbricato_cod
                Dim chiaviFabbr As String() = datiRequest.codice_esterno.Split("_"c)

                If chiaviFabbr.Length <> 3 Then
                    Throw New Exception(String.Format(My.Resources.AgronicaCoreDemetraBIZ.ChiaveGiasSbagliata, datiRequest.codice_esterno))
                End If

                'verificare che esista effettivamente questa chiave
                Dim fabbricatoUpdate As Fabbricato = objFabbricati_R.Leggi_Fabbricato_Oggetto(CStr(piva),
                                                                                               chiaviFabbr(1),
                                                                                               chiaviFabbr(2),
                                                                                               objParametri_Server)

                'TODO: COSA FARE SE CHIAVE gias NON ESISTE PIù
                If fabbricatoUpdate Is Nothing Then
                    Throw New Exception(String.Format(My.Resources.AgronicaCoreDemetraBIZ.FabbricatoNonEsistentePerChiaveGIAS, datiRequest.codice_esterno))
                End If

                objFabbricato.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Fabbricato.PK(piva, chiaviFabbr(1), chiaviFabbr(2))
                onUpdate = True

            End If

            Dim objDecodificaUtenti = New decodificaUtenti
            Dim utentePrecedente = objParametri_Server.UsernameOperazione
            objParametri_Server.UsernameOperazione = objDecodificaUtenti.decoficaUtenteGiasDaUtenteDemetra(datiRequest.elemento_anagrafico.utente_ultima_modifica, cuaa, objParametri_Utenti)

            Dim objFabbricati_W As New AgronicaCoreAnagrafeBIZ.Fabbricato_W
            objFabbricato.primaryKey.codice = objFabbricati_W.Scrivi_Fabbricato_Anagrafica(objFabbricato,
                                                                                           objParametri_Server,
                                                                                           objParametri_Utenti, NoteLog:="Import Demetra",
                                                                                           SistemaOrigine:=enum_SistemiEsterni.demetra)

            objParametri_Server.UsernameOperazione = utentePrecedente

            'se non sono in update, la tabella interscambio è da popolare
            If onUpdate = False AndAlso objFabbricato.flag_cancellazione = False Then
                interscambioWBIZ.Scrivi_Tabella_Interscambio(enum_SistemiEsterni.demetra,
                                                            objFabbricato.primaryKey.centroAziendalePK.partitaIva,
                                                            objFabbricato.primaryKey.centroAziendalePK.codice,
                                                            objFabbricato.primaryKey.codice,
                                                            datiRequest.codice,
                                                            objParametri_Server)
            End If

            '-----------------------------------
            '   SCRITTURA LOG INVIO ANAGRAFE 'OK'
            '-----------------------------------
            Dim chiave_GIAS As String = objFabbricato.primaryKey.centroAziendalePK.partitaIva & "_" & CStr(objFabbricato.primaryKey.centroAziendalePK.codice) & "_" & CStr(objFabbricato.primaryKey.codice)
            Chiama_Scrivi_Log_Invio_Anagrafe(cuaa, objFabbricato,
                                             chiave_GIAS, datiRequest.codice,
                                             objFabbricato.primaryKey.centroAziendalePK.partitaIva, objFabbricato.primaryKey.centroAziendalePK.codice, objFabbricato.primaryKey.codice,
                                             If(objFabbricato.flag_cancellazione, enum_TipoOperazioneDB.Cancellazione, If(onUpdate, enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Scrittura)), "OK", "",
                                             objParametri_Server, UtilizzaTransazione:=False)


            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la transazione e la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            End If

            errorMessage = ex.Message

            '-----------------------------------
            '   SCRITTURA LOG INVIO ANAGRAFE 'KO'
            '-----------------------------------
            Dim chiave_GIAS As String = objFabbricato.primaryKey.centroAziendalePK.partitaIva & "_" & CStr(objFabbricato.primaryKey.centroAziendalePK.codice) & "_" & CStr(objFabbricato.primaryKey.codice)
            Chiama_Scrivi_Log_Invio_Anagrafe(cuaa, objFabbricato,
                                             chiave_GIAS, datiRequest.codice,
                                             objFabbricato.primaryKey.centroAziendalePK.partitaIva, objFabbricato.primaryKey.centroAziendalePK.codice, objFabbricato.primaryKey.codice,
                                             If(objFabbricato.flag_cancellazione, enum_TipoOperazioneDB.Cancellazione, If(onUpdate, enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Scrittura)), "KO", errorMessage,
                                             objParametri_Server, UtilizzaTransazione:=False)

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

    End Sub

    Private Shared Sub Chiama_Scrivi_Log_Invio_Anagrafe(CUAA As String, Fabbricato As Fabbricato,
                                                        chiave As String, chiave_esterna As String,
                                                        Piva As String, Sa_Cod As Integer, Fabbricato_Cod As Integer,
                                                        TipoOperazione As enum_TipoOperazioneDB, Esito As String, Dati_Ricevuti As String,
                                                        objParametri_Server As AgronicaCoreParametri, Optional UtilizzaTransazione As Boolean = True)

        Dim objLogInvioChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W

        Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim pacchettoDaLoggare As New AgronicaCoreDTOStd.InData.importazioni.ImportDemetra With {
            .CUAA = CUAA,
            .dati = JsonConvert.SerializeObject(Fabbricato, tzh)
        }

        Dim strPacchettoDaLoggare As String = JsonConvert.SerializeObject(pacchettoDaLoggare, tzh)

        objLogInvioChiamate.Scrivi_Log_Invio_Anagrafe(enum_Esportazioni_Sistema_Cod.Demetra_Import_Fabbricati, strPacchettoDaLoggare, enum_TipoEntita_Des.Fabbricati,
                                                      chiave, chiave_esterna,
                                                      Piva, Sa_Cod, 0, 0, 0, 0, Fabbricato_Cod, "",
                                                      TipoOperazione,
                                                      Esito, Dati_Ricevuti,
                                                      objParametri_Server,
                                                      UtilizzaTransazione:=UtilizzaTransazione)

    End Sub
End Class
