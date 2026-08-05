Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreDTOStd.InData.importazioni
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDTOStd.InData.Demetra
Imports AgronicaCoreInterscambioBIZ
Imports AgronicaCoreContabDAL
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports System.Linq
Imports AgronicaCoreModelsSTD.attivita.risorse

Public Class ParametriExtra_Squadre_Export_Demetra
    Public DemetraBaseUrl As String
    Public apikey As String
    Public CUAA As String()

End Class

Public Class ExportSquadre

    Private _objParametri_Super_Server As AgronicaCoreParametri = Nothing
    Private _objParametri_Server As AgronicaCoreParametri = Nothing
    Private _objParametri_Utenti As AgronicaCoreParametri = Nothing
    Private _parametriExtra As ParametriExtra_Squadre_Export_Demetra = Nothing

    Private _configServizio As AgronicaCoreVarieDAL.Configurazione_Servizio = Nothing

    Public Sub New()

    End Sub

    Public Sub New(ByVal parametriExtra As String,
                   ByVal configServizio As AgronicaCoreVarieDAL.Configurazione_Servizio,
                   ByRef objParametri_Super_Server As AgronicaCoreParametri,
                   ByRef objParametri_Server As AgronicaCoreParametri,
                   ByRef objParametri_Utenti As AgronicaCoreParametri)

        _objParametri_Super_Server = objParametri_Super_Server
        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti

        _parametriExtra = JsonConvert.DeserializeObject(Of ParametriExtra_Squadre_Export_Demetra)(parametriExtra)

        _configServizio = configServizio
    End Sub


    Public Function Esporta(ByRef Messaggio_di_Ritorno_Opzionale As String) As Boolean

        Dim result As Boolean

        Dim objDemetraBIZ_util As New Util

        Try

            Dim AgroDanagrafeLogDAL As New AgronicaLogAnagrafe_R
            Dim xImpCodR As New Imprese_Codici_Read
            Dim interscambioRBIZ As New Interscambio_SquadreXAttivita_R

            'leggiamo i log per capire quali squadre inviare
            Dim StringaPivaValidi As String = ""

            If _parametriExtra.CUAA IsNot Nothing AndAlso _parametriExtra.CUAA.Length > 0 Then

                StringaPivaValidi = "logAnagrafe.Param1 IN ( "

                Dim indice As Integer = 0

                For Each cuaa As String In _parametriExtra.CUAA

                    indice += 1

                    StringaPivaValidi = StringaPivaValidi & " '" & xImpCodR.Piva_from_CUAA(cuaa, _objParametri_Server) & "'"

                    If indice < _parametriExtra.CUAA.Length Then
                        StringaPivaValidi &= ","
                    End If

                Next

                StringaPivaValidi &= " )"

            End If

            Dim orderBy As String = ""

            Dim dataTableResult As DataTable = AgroDanagrafeLogDAL.LeggiLogAnagrafeJoinInvio(0,
                                                                                             "",
                                                                                             "",
                                                                                             enum_Esportazioni_Sistema_Cod.Demetra_Export_Squadre,
                                                                                             enum_TipoOperazioneDB.Lettura,
                                                                                             enum_SistemiEsterni.demetra,
                                                                                             enum_TipoEntita_Des.Squadre,
                                                                                             "",
                                                                                             "",
                                                                                             "",
                                                                                             "",
                                                                                             "",
                                                                                             "",
                                                                                             "",
                                                                                             0,
                                                                                             StringaPivaValidi,
                                                                                             orderBy,
                                                                                             _objParametri_Server)

            Dim retrial As Boolean = False
            Dim IDChiamata As Integer = -1
            Dim chiaveSquadra As String()

            For Each row As DataRow In dataTableResult.Rows

                Dim FlagTransazioneLocale As Boolean = False
                Dim FlagConnessioneLocale As Boolean = False

                Dim CDG_DAL As New CDG_DAL_R

                Dim datiDaEsportare As New AnagraficaWrapper(Of Squadra)
                Dim pacchettoDaInviare As New ImportDemetra()

                Dim esitoCorrente = Util_Costanti.ESITO_OK
                Dim errorMessageCorrente = ""

                Dim TipoOperazione As enum_TipoOperazioneDB

                Dim timeStamp As Date = Date.Now

                Try

                    'Apro la connessione al DB
                    ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, _objParametri_Server)

                    If row("Esito") = Util_Costanti.ESITO_KO OrElse row("Esito") = Util_Costanti.ESITO_BLK Then
                        retrial = True
                        IDChiamata = row("ID_Chiamata")
                    Else
                        retrial = False
                        IDChiamata = -1
                    End If

                    chiaveSquadra = row("Chiave").ToString().Split("_"c)
                    TipoOperazione = row("Tipo_Operazione")

                    'occorre leggere la SquadraXAttivita 
                    Dim squadraRow As DataTable = CDG_DAL.Leggi_SquadrexAttvita("", chiaveSquadra(1), 0, 0, 0, "", Date.UtcNow, _objParametri_Server)

                    'se non esiste, sollevo eccezione
                    If TipoOperazione <> enum_TipoOperazioneDB.Cancellazione AndAlso squadraRow.Rows.Count = 0 Then
                        Messaggio_di_Ritorno_Opzionale &= " Squadra " & chiaveSquadra(1) & " non presente in Anagrafica "
                        Throw New Exception(Messaggio_di_Ritorno_Opzionale)
                    End If

                    datiDaEsportare.codice = chiaveSquadra(1)   'String.Format("{0}_{1}", chiaveSquadra(0), chiaveSquadra(1))

                    Dim pivaFromSquadra As String = chiaveSquadra(0)
                    'se esiste, comincio a valorizzare l'oggetto da inviare
                    Dim cuaa As String = xImpCodR.Leggi_CUAA(pivaFromSquadra, _objParametri_Server)

                    If Not String.IsNullOrEmpty(cuaa) Then

                        pacchettoDaInviare.CUAA = cuaa

                        datiDaEsportare.elemento_anagrafico = New Squadra()

                        Dim chiaveEsterna As DataTable = interscambioRBIZ.Leggi_Tabella_Interscambio_ChiaveGIAS(enum_SistemiEsterni.demetra,
                                                                                                               datiDaEsportare.codice,
                                                                                                               _objParametri_Server)

                        If chiaveEsterna.Rows.Count = 0 Then
                            datiDaEsportare.codice_esterno = ""
                        Else
                            datiDaEsportare.codice_esterno = chiaveEsterna.Rows(0)("Codice_Esterno")
                        End If

                        If TipoOperazione = enum_TipoOperazioneDB.Cancellazione Then
                            datiDaEsportare.elemento_anagrafico.flag_cancellazione = True
                        Else
                            'valorizzo tutti i dati richiesti da esportare
                            Dim risUmBIZ As New AgronicaCoreAnagrafeBIZ.RisorseUmane_R
                            Dim interscContattiBIZ As New AgronicaCoreInterscambioBIZ.Interscambio_Contatti_R

                            datiDaEsportare.elemento_anagrafico.descrizione = squadraRow.Rows(0)("des_Squadra")

                            datiDaEsportare.elemento_anagrafico.capisquadra = New List(Of ElementiSquadra)()
                            datiDaEsportare.elemento_anagrafico.membri = New List(Of ElementiSquadra)()

                            Dim valoreCodRisumList = squadraRow.Rows(0)("cod_risum_list")
                            Dim valoreCodRisumCapoSquadraList = squadraRow.Rows(0)("cod_risum_caposquadra_list")

                            'dobbiamo distinguere tra capi squadra e membri normali
                            Dim listaCodRisumMembri As List(Of String) = If(
                                                                            IsDBNull(valoreCodRisumList) OrElse valoreCodRisumList Is Nothing,
                                                                            New List(Of String),
                                                                            New List(Of String)(valoreCodRisumList.ToString().Split("|"c))
                                                                        )

                            Dim listaCodRisumCapiSquadra As List(Of String) = If(
                                                                                IsDBNull(valoreCodRisumCapoSquadraList) OrElse valoreCodRisumCapoSquadraList Is Nothing,
                                                                                New List(Of String),
                                                                                New List(Of String)(valoreCodRisumCapoSquadraList.ToString().Split("|"c))
                                                                            )

                            BuildListContatti(pivaFromSquadra, listaCodRisumCapiSquadra, datiDaEsportare.elemento_anagrafico.capisquadra)
                            BuildListContatti(pivaFromSquadra, listaCodRisumMembri, datiDaEsportare.elemento_anagrafico.membri)

                            datiDaEsportare.elemento_anagrafico.validita = New Validita() With {.inizio = squadraRow.Rows(0)("Validita_Inizio"), .fine = squadraRow.Rows(0)("Validita_Fine")}

                        End If

                        pacchettoDaInviare.dati = BuildDatoCompresso(datiDaEsportare)

                        If Not objDemetraBIZ_util.CallEndpoint(_parametriExtra.apikey, _parametriExtra.DemetraBaseUrl, pacchettoDaInviare, errorMessageCorrente) Then
                            Throw New Exception(errorMessageCorrente)
                        End If

                    Else

                        esitoCorrente = Util_Costanti.ESITO_BLK
                        errorMessageCorrente = Util_Costanti.BLK_MESSAGE_PREFIX & " CUAA non esistente"

                        pacchettoDaInviare.dati = BuildDatoCompresso(datiDaEsportare)
                        pacchettoDaInviare.CUAA = cuaa

                    End If

                    If retrial Then
                        Update_Log_Invio_Chiamate(IDChiamata,
                                                  pacchettoDaInviare,
                                                  TipoOperazione,
                                                  esitoCorrente,
                                                  errorMessageCorrente,
                                                  timeStamp,
                                                  _objParametri_Server)
                    Else
                        'commentato in attesa che vengano realizzati i metodi per loggare la creazione di una squadra
                        Chiama_Scrivi_Log_Invio_Anagrafe(pacchettoDaInviare,
                                                         row("Chiave").ToString(),
                                                         TipoOperazione,
                                                         esitoCorrente,
                                                         pivaFromSquadra,
                                                         errorMessageCorrente,
                                                         timeStamp,
                                                         _objParametri_Server)
                    End If

                Catch ex As Exception

                    Dim errorMessage = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

                    If retrial Then
                        Update_Log_Invio_Chiamate(IDChiamata,
                                                  pacchettoDaInviare,
                                                  TipoOperazione,
                                                  Util_Costanti.ESITO_KO,
                                                  errorMessage,
                                                  timeStamp,
                                                  _objParametri_Server)
                    Else
                        'commentato in attesa che vengano realizzati i metodi per loggare la creazione di una squadra
                        Chiama_Scrivi_Log_Invio_Anagrafe(pacchettoDaInviare,
                                                         row("Chiave").ToString(),
                                                         TipoOperazione,
                                                         Util_Costanti.ESITO_KO,
                                                         chiaveSquadra(0),
                                                         errorMessageCorrente,
                                                         timeStamp,
                                                         _objParametri_Server)
                    End If

                Finally

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Chiudo la transazione e la connessione al DB
                    ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, _objParametri_Server)
                    ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, _objParametri_Server)

                End Try
            Next

            result = True

        Catch ex As Exception

            result = False

            Messaggio_di_Ritorno_Opzionale &= " Errore durante l'esportazione delle squadre: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)
            objDemetraBIZ_util.Chiama_ScriviLOG(_configServizio.Tipo_Sincro.ToString, Messaggio_di_Ritorno_Opzionale, _configServizio.DirectoryLOG, _configServizio.Tipo_Sincro.ToString & "_log.txt", _objParametri_Server)

        End Try

        Return result

    End Function

    Private Sub BuildListContatti(piva As String, listaCodRisUm As List(Of String), listaContatti As List(Of ElementiSquadra))

        Dim risUmBIZ As New AgronicaCoreAnagrafeBIZ.RisorseUmane_R
        Dim interscContattiBIZ As New AgronicaCoreInterscambioBIZ.Interscambio_Contatti_R

        For Each codRis In listaCodRisUm

            Dim elemSquadra As New ElementiSquadra

            'recupero la risorsa
            Dim risorsa As RisorseUmane = risUmBIZ.Decodifica(piva, codRis, _objParametri_Server, False)

            Dim rowInterSc As DataTable = interscContattiBIZ.Leggi_Tabella_Interscambio_ChiaveGIAS(enum_SistemiEsterni.demetra, piva, risorsa.contatto.primaryKey.codice, codRis, _objParametri_Server)

            elemSquadra.codice = String.Format("{0}_{1}", piva, risorsa.contatto.primaryKey.codice)
            elemSquadra.codice_esterno = If(rowInterSc.Rows.Count = 0, "", rowInterSc.Rows(0)("Codice_Esterno"))

            listaContatti.Add(elemSquadra)

        Next

    End Sub

    Private Function BuildDatoCompresso(ByVal datiDaEsportare As AnagraficaWrapper(Of Squadra)) As String
        Dim tzh As New JsonSerializerSettings With {.DateFormatString = "yyyy-MM-ddT00:00:00Z"}
        Return AgroZip.CompressioneBase64(1, JsonConvert.SerializeObject(datiDaEsportare, tzh))

    End Function


    Private Shared Sub Update_Log_Invio_Chiamate(ID As Integer, pacchettoDaInviare As ImportDemetra, TipoOperazione As enum_TipoOperazioneDB, Esito As String, Dati_Ricevuti As String, Data_Invio As DateTime, objParametri_Server As AgronicaCoreParametri)

        Dim objlog_invioChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W
        Dim DatiSquadra As String = ""


        If pacchettoDaInviare.dati IsNot Nothing Then
            Dim pacchettoDaLoggare As New ImportDemetra
            pacchettoDaLoggare.CUAA = pacchettoDaInviare.CUAA
            pacchettoDaLoggare.dati = AgroZip.DeCompressioneBase64(1, pacchettoDaInviare.dati)

            Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            DatiSquadra = JsonConvert.SerializeObject(pacchettoDaLoggare, tzh)
        End If

        objlog_invioChiamate.Update_Log_Invio_Chiamate(ID, enum_Esportazioni_Sistema_Cod.Demetra_Export_Squadre, DatiSquadra, TipoOperazione, Esito, Dati_Ricevuti, objParametri_Server, Data_Invio)

    End Sub

    Private Shared Sub Chiama_Scrivi_Log_Invio_Anagrafe(pacchettoDaInviare As ImportDemetra,
                                                        chiave As String,
                                                        TipoOperazione As enum_TipoOperazioneDB,
                                                        Esito As String,
                                                        piva As String,
                                                        datiRicevuti As String,
                                                        dataInvio As DateTime,
                                                        objParametri_Server As AgronicaCoreParametri,
                                                            Optional UtilizzaTransazione As Boolean = True)

        Dim objlog_invioChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W

        Dim strPacchettoDaLoggare As String = ""

        If Not IsNothing(pacchettoDaInviare) Then
            Dim pacchettoDaLoggare As New ImportDemetra With {
                .CUAA = pacchettoDaInviare.CUAA,
                .dati = AgroZip.DeCompressioneBase64(1, pacchettoDaInviare.dati)
            }

            Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            strPacchettoDaLoggare = JsonConvert.SerializeObject(pacchettoDaLoggare, tzh)
        End If

        objlog_invioChiamate.Scrivi_Log_Invio_Anagrafe(
                                                        enum_Esportazioni_Sistema_Cod.Demetra_Export_Squadre,
                                                        strPacchettoDaLoggare,
                                                        enum_TipoEntita_Des.Squadre,
                                                        chiave,
                                                        "",
                                                        piva,
                                                        0,
                                                        0,
                                                        0,
                                                        0,
                                                        0,
                                                        0,
                                                        "",
                                                        TipoOperazione,
                                                        Esito,
                                                        datiRicevuti,
                                                        objParametri_Server,
                                                        Data_Invio:=dataInvio)

    End Sub

End Class