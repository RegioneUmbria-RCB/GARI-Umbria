Imports AgronicaCoreDataProvider
Imports System.Net
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreDTOStd.InData.importazioni
Imports AgronicaCoreInterscambioBIZ
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.InData.Demetra
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreContabDAL

Public Class ExportMacchine
    Public Class ParametriExtra
        Public DemetraBaseUrl As String
        Public apikey As String
        Public CUAA As String()
        Public defaultCodiceSIAN As String

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
            Dim interscambioRBIZ As New Interscambio_Parco_Macchine_R
            Dim demetrabiz As New Util

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

            Dim dataTableResult As DataTable = AgroDanagrafeLogDAL.LeggiLogAnagrafeJoinInvio(0,
                                                                                             "",
                                                                                             "",
                                                                                             enum_Esportazioni_Sistema_Cod.Demetra_Export_Macchine,
                                                                                             enum_TipoOperazioneDB.Lettura,
                                                                                             enum_SistemiEsterni.demetra,
                                                                                             enum_TipoEntita_Des.ParcoMacchine,
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

            Dim retrial As Boolean = False
            Dim IDChiamata As Integer = -1
            Dim chiaveMacchina As String()

            For Each row As DataRow In dataTableResult.Rows

                Dim FlagTransazioneLocale As Boolean = False
                Dim FlagConnessioneLocale As Boolean = False

                Dim pacchettoDaInviare As New ImportDemetra()

                Dim esitoCorrente = Util_Costanti.ESITO_OK
                Dim errorMessageCorrente = ""

                Dim TipoOperazione As enum_TipoOperazioneDB

                Dim timeStamp As Date = Date.Now

                Try

                    'Apro la connessione al DB
                    ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server)

                    If row("Esito") = Util_Costanti.ESITO_KO OrElse row("Esito") = Util_Costanti.ESITO_BLK Then
                        retrial = True
                        IDChiamata = row("ID_Chiamata")
                    Else
                        retrial = False
                        IDChiamata = -1
                    End If

                    chiaveMacchina = row("Chiave").ToString().Split("_"c)

                    TipoOperazione = row("Tipo_Operazione")

                    'lavez - 12/02/2024 - nuova istanza ad ogni iterazion o rimangono dati sporchi....
                    Dim datiDaEsportare As New AnagraficaWrapper(Of Equipaggiamento)
                    datiDaEsportare.elemento_anagrafico = New Equipaggiamento()

                    Dim cuaa As String = xImpCodR.Leggi_CUAA(chiaveMacchina(0), objParametri_Server)

                    'leggo tabella interscambio
                    Dim chiaveEsterna As DataTable = interscambioRBIZ.GetChiaveEsterna(enum_SistemiEsterni.demetra,
                                                                                       chiaveMacchina(2),
                                                                                       objParametri_Server)
                    datiDaEsportare.codice = row("Chiave").ToString()

                    If chiaveEsterna.Rows.Count = 0 Then
                        datiDaEsportare.codice_esterno = ""
                    Else
                        datiDaEsportare.codice_esterno = chiaveEsterna.Rows(0)("Codice_Esterno")
                    End If

                    pacchettoDaInviare.CUAA = cuaa

                    If Not String.IsNullOrEmpty(cuaa) Then

                        datiDaEsportare.elemento_anagrafico = MapMacchinaToEquipaggiamento(chiaveMacchina(0),
                                                                                           chiaveMacchina(2),
                                                                                           (TipoOperazione = enum_TipoOperazioneDB.Cancellazione),
                                                                                           Parametri_Extra.defaultCodiceSIAN,
                                                                                           objParametri_Server)

                        pacchettoDaInviare.dati = BuildPackageToSend(datiDaEsportare)

                        If Not demetrabiz.CallEndpoint(Parametri_Extra.apikey, Parametri_Extra.DemetraBaseUrl, pacchettoDaInviare, errorMessageCorrente) Then
                            Throw New Exception(errorMessageCorrente)
                        End If

                    Else

                        esitoCorrente = Util_Costanti.ESITO_BLK
                        errorMessageCorrente = Util_Costanti.BLK_MESSAGE_PREFIX & " CUAA non esistente"

                        pacchettoDaInviare.dati = BuildPackageToSend(datiDaEsportare)

                    End If

                    If retrial Then
                        Update_Log_Invio_Chiamate(IDChiamata,
                                                  pacchettoDaInviare,
                                                  TipoOperazione,
                                                  esitoCorrente,
                                                  errorMessageCorrente,
                                                  timeStamp,
                                                  objParametri_Server)
                    Else
                        Chiama_Scrivi_Log_Invio_Macchine(pacchettoDaInviare,
                                                         row("Chiave").ToString(),
                                                         chiaveMacchina(0),
                                                         chiaveMacchina(1),
                                                         chiaveMacchina(2),
                                                         TipoOperazione,
                                                         esitoCorrente,
                                                         errorMessageCorrente,
                                                         timeStamp,
                                                         objParametri_Server)
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
                                                  objParametri_Server)
                    Else

                        Chiama_Scrivi_Log_Invio_Macchine(pacchettoDaInviare,
                                                         row("Chiave").ToString(),
                                                         chiaveMacchina(0),
                                                         chiaveMacchina(1),
                                                         chiaveMacchina(2),
                                                         TipoOperazione,
                                                         Util_Costanti.ESITO_KO,
                                                         errorMessage,
                                                         timeStamp,
                                                         objParametri_Server)
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
                        enum_Esportazioni_Sistema_Cod.Demetra_Export_Macchine
                    }
                objLogger.ExtractLog(configServizio, Tipo_Esportazione, Parametri_Extra.FiltroEsportazione, Parametri_Extra.Ambiente, objParametri_Server, Messaggio_di_Ritorno_Opzionale)
            End If

        Catch ex As Exception

            result = False

            Messaggio_di_Ritorno_Opzionale &= " Errore durante l'esportazione delle macchine: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)
            objDemetraBIZ_util.Chiama_ScriviLOG(configServizio.Tipo_Sincro.ToString, Messaggio_di_Ritorno_Opzionale, configServizio.DirectoryLOG, configServizio.Tipo_Sincro.ToString & "_log.txt", objParametri_Server)

        End Try

        Return result

    End Function

    Private Function BuildPackageToSend(ByVal datiDaEsportare As AnagraficaWrapper(Of Equipaggiamento)) As String
        Dim tzh As New JsonSerializerSettings With {.DateFormatString = "yyyy-MM-dd"}
        Return AgroZip.CompressioneBase64(1, JsonConvert.SerializeObject(datiDaEsportare, tzh))
    End Function

    Private Function MapMacchinaToEquipaggiamento(ByVal piva As String,
                                                  ByVal mac_cod As Integer,
                                                  ByVal FlagCancellazione As Boolean,
                                                  ByVal DefaultSIANCode As String,
                                                  ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreDTOStd.InData.Demetra.Equipaggiamento
        Dim objMacchina As New AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine
        Dim equip As New AgronicaCoreDTOStd.InData.Demetra.Equipaggiamento
        Dim objMacchineBIZ_R As New AgronicaCoreContabBIZ.Parco_Macchine_R


        equip.flag_cancellazione = FlagCancellazione

        If Not equip.flag_cancellazione Then
            objMacchina = objMacchineBIZ_R.Leggi_Macchina(piva,
                                                          mac_cod,
                                                          objParametri_Server)

            'lavez - 20/02/2024 - Codice Agea della macchina, se l'ho valorizzato riporto quello altrimenti recupero la decodifica tramite classcode
            If objMacchina.ageaCod IsNot Nothing AndAlso objMacchina.ageaCod.codice <> "" Then
                equip.tipo = objMacchina.ageaCod.codice
            Else
                equip.tipo = GetCodeAGEA(objMacchina.tipo.codice,
                                     objMacchina.dettaglio_1.codice,
                                     objMacchina.dettaglio_2.codice,
                                     DefaultSIANCode,
                                     objParametri_Server)
            End If


            equip.telaio = objMacchina.telaio
            equip.targa = objMacchina.targa
            equip.descrizione = objMacchina.descrizione
            equip.modello = objMacchina.modello
            equip.data_ultima_taratura = objMacchina.data_Ultima_Taratura
            equip.scadenza_taratura = objMacchina.scadenza_Taratura
            equip.validita = New Validita() With {.inizio = objMacchina.validita.inizio, .fine = objMacchina.validita.fine}
            equip.alimentazione = objMacchina.alimentazione.codice
            'Lavez - 30/07/2024 - numero certificato taratura
            equip.nr_certificato = objMacchina.numero_certificato

        End If

        Return equip
    End Function

    Private Function GetCodeAGEA(ByVal Tipo As String,
                                 ByVal Dettaglio1 As String,
                                 ByVal Dettaglio2 As String,
                                 ByVal DefaultSIANCode As String,
                                 ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim objCodificaSian_DAL As New Codifica_Macchine_Agea

        Dim ClassCode As String = DefaultSIANCode
        Dim sCCodestr As String = IIf(Tipo <> "", Tipo, "") + IIf(Dettaglio1 <> "", "." + Dettaglio1, "") + IIf(Dettaglio2 <> "", "." + Dettaglio2, "")

        'se non ho specificato nulla ritorno default
        If sCCodestr = "" Then
            Return ClassCode
        End If

        'ricerca a 3 livelli
        Dim dtCodAGEA = objCodificaSian_DAL.leggi(objParametri_Server, "", "", sCCodestr)
        If dtCodAGEA.Rows.Count > 0 Then
            ClassCode = dtCodAGEA.Rows(0)("AGEA_Cod")
            Return ClassCode
        End If

        'ricerca a 2 livelli
        sCCodestr = IIf(Tipo <> "", Tipo, "") + IIf(Dettaglio1 <> "", "." + Dettaglio1, "")
        dtCodAGEA = objCodificaSian_DAL.leggi(objParametri_Server, "", "", sCCodestr)
        If dtCodAGEA.Rows.Count > 0 Then
            ClassCode = dtCodAGEA.Rows(0)("AGEA_Cod")
            Return ClassCode
        End If

        'ricerca a 1 livello
        sCCodestr = IIf(Tipo <> "", Tipo, "")
        dtCodAGEA = objCodificaSian_DAL.leggi(objParametri_Server, "", "", sCCodestr)
        If dtCodAGEA.Rows.Count > 0 Then
            ClassCode = dtCodAGEA.Rows(0)("AGEA_Cod")
            Return ClassCode
        End If

        Return ClassCode
    End Function


    Private Shared Sub Chiama_Scrivi_Log_Invio_Macchine(pacchettoDaInviare As ImportDemetra, Chiave As String, piva As String, sa_cod As String, mac_cod As Integer, TipoOperazione As enum_TipoOperazioneDB, Esito As String, Dati_Ricevuti As String, Data_Invio As DateTime, objParametri_Server As AgronicaCoreParametri)

        Dim objlog_invioChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W
        Dim DatiMacchina As String = ""

        If pacchettoDaInviare.dati IsNot Nothing Then
            Dim pacchettoDaLoggare As New ImportDemetra

            pacchettoDaLoggare.CUAA = pacchettoDaInviare.CUAA
            pacchettoDaLoggare.dati = AgroZip.DeCompressioneBase64(1, pacchettoDaInviare.dati)

            Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            DatiMacchina = JsonConvert.SerializeObject(pacchettoDaLoggare, tzh)
        End If

        objlog_invioChiamate.Scrivi_Log_Invio_Anagrafe(enum_Esportazioni_Sistema_Cod.Demetra_Export_Macchine, DatiMacchina, enum_TipoEntita_Des.ParcoMacchine, Chiave, "", piva, sa_cod, 0, 0, 0, 0, 0, "", TipoOperazione, Esito, Dati_Ricevuti, objParametri_Server, Mac_Cod:=mac_cod, Data_Invio:=Data_Invio)

    End Sub

    Private Shared Sub Update_Log_Invio_Chiamate(ID As Integer, pacchettoDaInviare As ImportDemetra, TipoOperazione As enum_TipoOperazioneDB, Esito As String, Dati_Ricevuti As String, Data_Invio As DateTime, objParametri_Server As AgronicaCoreParametri)

        Dim objlog_invioChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W
        Dim DatiMacchina As String = ""


        If pacchettoDaInviare.dati IsNot Nothing Then
            Dim pacchettoDaLoggare As New ImportDemetra
            pacchettoDaLoggare.CUAA = pacchettoDaInviare.CUAA
            pacchettoDaLoggare.dati = AgroZip.DeCompressioneBase64(1, pacchettoDaInviare.dati)

            Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            DatiMacchina = JsonConvert.SerializeObject(pacchettoDaLoggare, tzh)
        End If

        objlog_invioChiamate.Update_Log_Invio_Chiamate(ID, enum_Esportazioni_Sistema_Cod.Demetra_Export_Macchine, DatiMacchina, TipoOperazione, Esito, Dati_Ricevuti, objParametri_Server, Data_Invio)

    End Sub

End Class
