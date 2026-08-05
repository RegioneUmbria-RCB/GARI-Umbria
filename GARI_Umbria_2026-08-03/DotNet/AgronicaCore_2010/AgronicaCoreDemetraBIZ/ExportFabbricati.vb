
Imports System.Net
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreDTOStd.InData.importazioni
Imports AgronicaCoreInterscambioBIZ
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

Public Class ExportFabbricati

    Public Class ParametriExtra
        Public DemetraBaseUrl As String
        Public apikey As String
        Public CUAA As String()

        Public FiltroEsportazione As Integer = -1
        Public Ambiente As String
    End Class

    Public Function Esporta(parametriExtra As String,
                            configServizio As Configurazione_Servizio,
                            objParametri_Server As AgronicaCoreParametri,
                            ByRef Messaggio_di_Ritorno_Opzionale As String) As Boolean

        Dim result As Boolean

        Dim objDemetraBIZ_util As New Util

        Try
            Dim AgroDanagrafeLogDAL As New AgronicaLogAnagrafe_R
            Dim xImpCodR As New Imprese_Codici_Read

            Dim Parametri_Extra = JsonConvert.DeserializeObject(Of ParametriExtra)(parametriExtra)

            Dim StringaPivaValidi As String = ""

            If Parametri_Extra.CUAA IsNot Nothing AndAlso Parametri_Extra.CUAA.Length > 0 Then

                StringaPivaValidi = "logAnagrafe.Param1 IN ( "

                Dim indice As Integer = 0

                For Each cuaa As String In Parametri_Extra.CUAA

                    indice += 1

                    StringaPivaValidi = StringaPivaValidi & " '" & xImpCodR.Piva_from_CUAA(cuaa, objParametri_Server) & "'"

                    If indice < Parametri_Extra.CUAA.Length Then
                        StringaPivaValidi &= ","
                    End If

                Next

                StringaPivaValidi &= " )"

            End If

            Dim orderBy As String = ""

            Dim timeStamp As DateTime = DateTime.Now
            Dim dataTableResult As DataTable = AgroDanagrafeLogDAL.LeggiLogAnagrafeJoinInvio(0,
                                                                                             "",
                                                                                             "",
                                                                                             enum_Esportazioni_Sistema_Cod.Demetra_Export_Fabbricati,
                                                                                             enum_TipoOperazioneDB.Lettura,
                                                                                             enum_SistemiEsterni.demetra,
                                                                                             enum_TipoEntita_Des.Fabbricati,
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
                                                                                             objParametri_Server)
            Dim objFabbricato As New AgronicaCoreModelsSTD.anagrafiche.Fabbricato
            Dim objFabbricati_R As New AgronicaCoreAnagrafeBIZ.Fabbricato_R
            Dim retrial As Boolean = False
            Dim IDChiamata As Integer = -1

            For Each row As DataRow In dataTableResult.Rows

                Dim FlagTransazioneLocale As Boolean = False
                Dim FlagConnessioneLocale As Boolean = False

                Dim datiDaEsportare As New AnagraficaWrapper(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato)

                Dim pacchettoDaInviare As ImportDemetra = Nothing

                Dim TipoOperazione As enum_TipoOperazioneDB

                Try
                    Dim esitoCorrente = Util_Costanti.ESITO_OK
                    Dim errorMessageCorrente = ""

                    'Apro la connessione al DB
                    ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server)

                    If row("Esito") = Util_Costanti.ESITO_KO OrElse row("Esito") = Util_Costanti.ESITO_BLK Then
                        retrial = True
                        IDChiamata = row("ID_Chiamata")
                    Else
                        retrial = False
                        IDChiamata = -1
                    End If

                    Dim chiaveFabbricato As String() = row("Chiave").ToString().Split("_"c)

                    Dim cuaa = xImpCodR.Leggi_CUAA(chiaveFabbricato(0), objParametri_Server)

                    If Not String.IsNullOrEmpty(cuaa) Then
                        objFabbricato = objFabbricati_R.Leggi_Fabbricato_Oggetto(chiaveFabbricato(0), chiaveFabbricato(1), chiaveFabbricato(2), objParametri_Server)

                        TipoOperazione = row("Tipo_Operazione")
                        If row("Tipo_Operazione") = enum_TipoOperazioneDB.Cancellazione Then
                            objFabbricato.flag_cancellazione = True
                        End If

                        objFabbricato.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Fabbricato.PK(chiaveFabbricato(0), chiaveFabbricato(1), chiaveFabbricato(2))

                        If objFabbricato.particella IsNot Nothing AndAlso (objFabbricato.particella.primaryKey.Com = "0" OrElse objFabbricato.particella.primaryKey.Com = "") AndAlso (objFabbricato.particella.primaryKey.Prov = "0" OrElse objFabbricato.particella.primaryKey.Prov = "") Then
                            objFabbricato.particella.primaryKey = Nothing
                        End If

                        datiDaEsportare.elemento_anagrafico = objFabbricato

                        datiDaEsportare.codice = chiaveFabbricato(0) + "_" + chiaveFabbricato(1) + "_" + chiaveFabbricato(2)

                        'leggo tabella interscambio
                        Dim interscambioRBIZ As New Interscambio_Fabbricati_R
                        Dim chiaveEsterna As DataTable = interscambioRBIZ.Leggi_Tabella_Interscambio_ChiaveGIAS(enum_SistemiEsterni.demetra, chiaveFabbricato(0), chiaveFabbricato(1), chiaveFabbricato(2), objParametri_Server)

                        If chiaveEsterna.Rows.Count = 0 Then
                            datiDaEsportare.codice_esterno = ""
                        Else
                            datiDaEsportare.codice_esterno = chiaveEsterna.Rows(0)("Codice_Esterno")
                        End If

                        Dim tzh As New JsonSerializerSettings With {.DateFormatString = "yyyy-MM-ddT00:00:00Z"}
                        Dim datoCompresso As String = AgroZip.CompressioneBase64(1, JsonConvert.SerializeObject(datiDaEsportare, tzh))

                        pacchettoDaInviare = New ImportDemetra
                        pacchettoDaInviare.dati = datoCompresso
                        pacchettoDaInviare.CUAA = cuaa

                        Dim errorMessage As String = ""
                        If Not objDemetraBIZ_util.CallEndpoint(Parametri_Extra.apikey, Parametri_Extra.DemetraBaseUrl, pacchettoDaInviare, errorMessage) Then
                            Throw New Exception(errorMessage)
                        End If

                    Else

                        esitoCorrente = Util_Costanti.ESITO_BLK
                        errorMessageCorrente = Util_Costanti.BLK_MESSAGE_PREFIX & " CUAA non esistente"

                        datiDaEsportare.codice = chiaveFabbricato(0) + "_" + chiaveFabbricato(1) + "_" + chiaveFabbricato(2)

                        Dim tzh As New JsonSerializerSettings With {.DateFormatString = "yyyy-MM-ddT00:00:00Z"}
                        Dim datoCompresso As String = AgroZip.CompressioneBase64(1, JsonConvert.SerializeObject(datiDaEsportare, tzh))

                        pacchettoDaInviare = New ImportDemetra
                        pacchettoDaInviare.dati = datoCompresso
                        pacchettoDaInviare.CUAA = cuaa

                        'Giulia 17/01/2025: Necessario, perché se entra in questo ramo poi nel codice sotto prende i dati della chiave da questo oggetto
                        objFabbricato.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Fabbricato.PK(chiaveFabbricato(0), chiaveFabbricato(1), chiaveFabbricato(2))

                    End If

                    '-----------------------------------
                    '   SCRITTURA LOG INVIO ANAGRAFE 'OK'
                    '-----------------------------------
                    If retrial Then
                        Update_Log_Invio_Chiamate(IDChiamata, pacchettoDaInviare, TipoOperazione, esitoCorrente, errorMessageCorrente, timeStamp, objParametri_Server)
                    Else
                        Chiama_Scrivi_Log_Invio_Anagrafe(pacchettoDaInviare, row("Chiave").ToString(), TipoOperazione, esitoCorrente, objFabbricato.primaryKey.centroAziendalePK.partitaIva, objFabbricato.primaryKey.centroAziendalePK.codice, objFabbricato.primaryKey.codice, errorMessageCorrente, timeStamp, objParametri_Server)
                    End If

                Catch ex As Exception

                    Dim errorMessage = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

                    '-----------------------------------
                    '   SCRITTURA LOG INVIO ANAGRAFE 'KO'
                    '-----------------------------------
                    If retrial Then
                        Update_Log_Invio_Chiamate(IDChiamata, pacchettoDaInviare, TipoOperazione, Util_Costanti.ESITO_KO, errorMessage, timeStamp, objParametri_Server)
                    Else

                        Chiama_Scrivi_Log_Invio_Anagrafe(pacchettoDaInviare, row("Chiave").ToString(), TipoOperazione, Util_Costanti.ESITO_KO, objFabbricato.primaryKey.centroAziendalePK.partitaIva, objFabbricato.primaryKey.centroAziendalePK.codice, objFabbricato.primaryKey.codice, errorMessage, timeStamp, objParametri_Server)
                    End If

                Finally

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Chiudo la transazione e la connessione al DB
                    ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

                    ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

                End Try

            Next

            result = True

            If Parametri_Extra.FiltroEsportazione <> Enum_FiltroEsportazione_to_ElasticSearch.Nessuno Then
                Dim objLogger As New ExtractLog
                Dim Tipo_Esportazione As New List(Of String) From {
                        enum_Esportazioni_Sistema_Cod.Demetra_Export_Fabbricati
                    }
                objLogger.ExtractLog(configServizio, Tipo_Esportazione, Parametri_Extra.FiltroEsportazione, Parametri_Extra.Ambiente, objParametri_Server, Messaggio_di_Ritorno_Opzionale)
            End If

        Catch ex As Exception

            result = False

            Messaggio_di_Ritorno_Opzionale &= " Errore durante l'esportazione dei fabbricati: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)
            objDemetraBIZ_util.Chiama_ScriviLOG(configServizio.Tipo_Sincro.ToString, Messaggio_di_Ritorno_Opzionale, configServizio.DirectoryLOG, configServizio.Tipo_Sincro.ToString & "_log.txt", objParametri_Server)

        End Try

        Return result

    End Function


    Private Shared Sub Chiama_Scrivi_Log_Invio_Anagrafe(pacchettoDaInviare As ImportDemetra,
                                                        chiave As String,
                                                        TipoOperazione As enum_TipoOperazioneDB,
                                                        Esito As String,
                                                        piva As String,
                                                        saCod As Integer,
                                                        fabbricatoCod As Integer,
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

        objlog_invioChiamate.Scrivi_Log_Invio_Anagrafe(enum_Esportazioni_Sistema_Cod.Demetra_Export_Fabbricati, strPacchettoDaLoggare, enum_TipoEntita_Des.Fabbricati, chiave, "", piva, saCod, 0, 0, 0, 0, fabbricatoCod, "", TipoOperazione, Esito, datiRicevuti, objParametri_Server, Data_Invio:=dataInvio)

    End Sub


    Private Shared Sub Update_Log_Invio_Chiamate(ID As Integer, pacchettoDaInviare As ImportDemetra, TipoOperazione As enum_TipoOperazioneDB, Esito As String, Dati_Ricevuti As String, Data_Invio As DateTime, objParametri_Server As AgronicaCoreParametri)

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

        objlog_invioChiamate.Update_Log_Invio_Chiamate(ID, enum_Esportazioni_Sistema_Cod.Demetra_Export_Fabbricati, strPacchettoDaLoggare, TipoOperazione, Esito, Dati_Ricevuti, objParametri_Server, Data_Invio)

    End Sub

End Class
