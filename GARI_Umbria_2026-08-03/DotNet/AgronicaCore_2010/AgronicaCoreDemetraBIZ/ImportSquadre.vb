Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreDTOStd.InData.Demetra
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreInterscambioBIZ
Imports AgronicaCoreModelsSTD.anagrafiche
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeBIZ.AnagrafeNG

Public Class ImportSquadre

    Public Sub ImportSquadre(cuaa As String, datiRequest As AnagraficaWrapper(Of Squadra), ByRef errorMessage As String, objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        'Dim objFabbricato As New AgronicaCoreModelsSTD.anagrafiche.Fabbricato()
        Dim onUpdate As Boolean = False

        Dim piva As String = ""
        Dim idSquadra As Integer = 0
        Dim objSquadraXAttivita As New SquadreXAttivita With {
            .ID_Squadra = 0
        }

        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim interscambioWBIZ As New Interscambio_SquadreXAttivita_W
            Dim interscambioRBIZ As New Interscambio_SquadreXAttivita_R

            Dim CDG_DAL_Read As New CDG_DAL_R
            Dim CDG_DAL_Write As New CDG_DAL_W

            Dim chiaveSuInterscambio As DataTable

            Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            piva = xImpCodR.Piva_from_CUAA(cuaa, objParametri_Server)

            'da scommentare quando riusciamo a scrivere il fabbricato
            If piva = "" Then
                Throw New Exception(String.Format(My.Resources.AgronicaCoreDemetraBIZ.CuaaNonEsistente, cuaa))
            End If

            If String.IsNullOrEmpty(datiRequest.codice) AndAlso String.IsNullOrEmpty(datiRequest.codice_esterno) Then
                Throw New Exception(My.Resources.AgronicaCoreDemetraBIZ.ChiaviVuote)
            End If

            If String.IsNullOrEmpty(datiRequest.codice_esterno) OrElse datiRequest.codice_esterno = "0" Then
                'significa che potrebbe essere una nuova squadra, occorre verificare sulla tabella interscambi
                chiaveSuInterscambio = interscambioRBIZ.Leggi_Tabella_Interscambio_ChiaveEsterna(enum_SistemiEsterni.demetra,
                                                                                                 datiRequest.codice,
                                                                                                 objParametri_Server)

                'chiave esterna doppia
                If chiaveSuInterscambio.Rows.Count > 1 Then
                    Throw New Exception(String.Format(My.Resources.AgronicaCoreDemetraBIZ.SquadraDuplicataPerChiave, datiRequest.codice))
                End If

                If chiaveSuInterscambio.Rows.Count > 0 Then

                    objSquadraXAttivita.Piva = chiaveSuInterscambio.Rows(0)("Piva")
                    objSquadraXAttivita.ID_Squadra = chiaveSuInterscambio.Rows(0)("ID_Squadra")

                    onUpdate = True

                Else

                    objSquadraXAttivita.Piva = piva
                    objSquadraXAttivita.ID_Squadra = 0
                    onUpdate = False

                End If

            Else
                'spacchettare il campo codice esterno con piva-ID_SQUADRA
                Dim chiaveSquadra As String() = datiRequest.codice_esterno.Split("_"c)

                If chiaveSquadra.Length <> 2 Then
                    Throw New Exception(String.Format(My.Resources.AgronicaCoreDemetraBIZ.ChiaveGiasSbagliata, datiRequest.codice_esterno))
                End If

                objSquadraXAttivita.Piva = chiaveSquadra(0)
                objSquadraXAttivita.ID_Squadra = chiaveSquadra(1)

                onUpdate = True

            End If

            'verifichiamo che non si tratti di cancellazione, altrimenti è sicuramente ins/edit
            If datiRequest.elemento_anagrafico.flag_cancellazione AndAlso objSquadraXAttivita.ID_Squadra <> 0 Then

                idSquadra = objSquadraXAttivita.ID_Squadra

                interscambioWBIZ.Cancella_Tabella_Interscambio(enum_SistemiEsterni.demetra, datiRequest.codice, objParametri_Server)
                CDG_DAL_Write.Elimina_Squadra_Attivita(objSquadraXAttivita.Piva, objSquadraXAttivita.ID_Squadra, objParametri_Server, enum_SistemiEsterni.demetra)

            Else

                If objSquadraXAttivita.ID_Squadra <> 0 Then
                    Dim squadraRow As DataTable = CDG_DAL_Read.Leggi_SquadrexAttvita("", objSquadraXAttivita.ID_Squadra, 0, 0, 0, "", Date.UtcNow, objParametri_Server)

                    'TODO: COSA FARE SE CHIAVE gias NON ESISTE PIù
                    If squadraRow.Rows.Count = 0 Then
                        Throw New Exception(String.Format(My.Resources.AgronicaCoreDemetraBIZ.SquadraNonEsistentePerChiaveGIAS, datiRequest.codice_esterno))
                    End If

                    objSquadraXAttivita.Username_Creazione = squadraRow.Rows(0)("Username_Creazione")
                    objSquadraXAttivita.Data_Creazione = squadraRow.Rows(0)("Data_Creazione")

                End If


                objSquadraXAttivita.cod_risum_caposquadra_list = BuildListCodRisum(piva, datiRequest.elemento_anagrafico.capisquadra, objParametri_Server)

                objSquadraXAttivita.cod_risum_list = BuildListCodRisum(piva, datiRequest.elemento_anagrafico.membri, objParametri_Server)

                objSquadraXAttivita.des_Squadra = If(String.IsNullOrEmpty(datiRequest.elemento_anagrafico.descrizione), String.Format("desc_{0}", objSquadraXAttivita.ID_Squadra), datiRequest.elemento_anagrafico.descrizione)

                objSquadraXAttivita.Validita_Inizio = datiRequest.elemento_anagrafico.validita.inizio
                objSquadraXAttivita.Validita_Fine = datiRequest.elemento_anagrafico.validita.fine

                Dim squadraString As String = JsonConvert.SerializeObject(objSquadraXAttivita)

                CDG_DAL_Write.Scrivi_Squadra_Attivita(piva,
                                                    squadraString,
                                                    objParametri_Server,
                                                    NOTELOG_ANAGRAFE_NG,
                                                    enum_SistemiEsterni.demetra,
                                                    idSquadra)

                If onUpdate = False Then
                    'TODO: cambiare il codice passato alla funzione
                    interscambioWBIZ.Scrivi_Tabella_Interscambio(enum_SistemiEsterni.demetra, piva, idSquadra, datiRequest.codice, objParametri_Server)
                End If

            End If

            '-----------------------------------
            '   SCRITTURA LOG INVIO ANAGRAFE 'OK'
            '-----------------------------------
            Dim chiave_GIAS As String = piva & "_" & CStr(idSquadra)
            Chiama_Scrivi_Log_Invio_Anagrafe(cuaa, datiRequest, chiave_GIAS, datiRequest.codice, piva,
                                             If(datiRequest.elemento_anagrafico.flag_cancellazione, enum_TipoOperazioneDB.Cancellazione, If(onUpdate, enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Scrittura)),
                                             Util_Costanti.ESITO_OK, "",
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
            Dim chiave_GIAS As String = piva & "_" & CStr(idSquadra)
            Chiama_Scrivi_Log_Invio_Anagrafe(cuaa, datiRequest, chiave_GIAS, datiRequest.codice, piva,
                                             If(datiRequest.elemento_anagrafico.flag_cancellazione, enum_TipoOperazioneDB.Cancellazione, If(onUpdate, enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Scrittura)),
                                             Util_Costanti.ESITO_KO, errorMessage,
                                             objParametri_Server, UtilizzaTransazione:=False)

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

    End Sub

    Private Function BuildListCodRisum(ByVal Piva As String, ByVal listContatti As List(Of ElementiSquadra), ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim sbCodRisum As New System.Text.StringBuilder()

        For Each contatto In listContatti

            Dim codRisum As String = FindCodRisumByContatto(Piva, contatto.codice, contatto.codice_esterno, objParametri_Server)

            If codRisum = "" Then

                Throw New Exception(String.Format(My.Resources.AgronicaCoreDemetraBIZ.CodRisUmNonTrovata, contatto.codice))

            End If

            If sbCodRisum.Length > 0 Then
                sbCodRisum.Append("|")
            End If

            sbCodRisum.Append(codRisum)

        Next

        Return sbCodRisum.ToString()

    End Function

    Private Function FindCodRisumByContatto(ByVal Piva As String, ByVal codiceEsterno As String, ByVal codice As String, ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim interscContattiBIZ As New AgronicaCoreInterscambioBIZ.Interscambio_Contatti_R
        Dim risorseUmaneDAL As New Risorse_Umane_R

        Dim codiceContatto As String = ""

        If String.IsNullOrEmpty(codice) Then

            Dim rowInterSc As DataTable = interscContattiBIZ.Leggi_Tabella_Interscambio_ChiaveEsterna(enum_SistemiEsterni.demetra, codiceEsterno, objParametri_Server)

            If rowInterSc.Rows.Count = 0 Then
                Throw New Exception(String.Format(My.Resources.AgronicaCoreDemetraBIZ.ContattoNonPresenteInInterscambio, codiceEsterno))
            End If

            If rowInterSc.Rows.Count > 1 Then
                Throw New Exception(String.Format(My.Resources.AgronicaCoreDemetraBIZ.ContattoDuplicato, codiceEsterno))
            End If

            Piva = rowInterSc(0)("Piva")
            codiceContatto = rowInterSc(0)("Cod_Contatto")

        Else
            'ho già il codice contatto
            Dim chiaveGiasContatto As String() = codice.Split("_"c)

            If chiaveGiasContatto.Length <> 2 Then
                Throw New Exception(String.Format(My.Resources.AgronicaCoreDemetraBIZ.ChiaveGiasSbagliata, chiaveGiasContatto))
            End If

            Piva = chiaveGiasContatto(0)
            codiceContatto = chiaveGiasContatto(1)

        End If

        'se invece esiste, andiamo a recuperare la risorsaUmana da inserire nell'oggetto Squadra
        'leggiamo il codice e andiamo a leggere le risorse_umane

        Dim risUmRow = risorseUmaneDAL.Leggi(Piva, codiceContatto, 0, 0, 0, "", True, True, "", "", objParametri_Server)

        Return If(risUmRow.Rows.Count > 0, risUmRow(0)("Cod_RisUm"), "")

    End Function

    Private Shared Sub Chiama_Scrivi_Log_Invio_Anagrafe(CUAA As String, Squadra As AnagraficaWrapper(Of Squadra),
                                                        chiave As String, chiave_esterna As String,
                                                        Piva As String, TipoOperazione As enum_TipoOperazioneDB,
                                                        Esito As String, Dati_Ricevuti As String,
                                                        objParametri_Server As AgronicaCoreParametri, Optional UtilizzaTransazione As Boolean = True)

        Dim objLogInvioChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W

        Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim pacchettoDaLoggare As New AgronicaCoreDTOStd.InData.importazioni.ImportDemetra With {
            .CUAA = CUAA,
            .dati = JsonConvert.SerializeObject(Squadra, tzh)
        }

        Dim strPacchettoDaLoggare As String = JsonConvert.SerializeObject(pacchettoDaLoggare, tzh)

        objLogInvioChiamate.Scrivi_Log_Invio_Anagrafe(enum_Esportazioni_Sistema_Cod.Demetra_Import_Squadre, strPacchettoDaLoggare, enum_TipoEntita_Des.Squadre,
                                                      chiave, chiave_esterna,
                                                      Piva, 0, 0, 0, 0, 0, 0, "",
                                                      TipoOperazione,
                                                      Esito, Dati_Ricevuti,
                                                      objParametri_Server,
                                                      UtilizzaTransazione:=UtilizzaTransazione)

    End Sub
End Class
