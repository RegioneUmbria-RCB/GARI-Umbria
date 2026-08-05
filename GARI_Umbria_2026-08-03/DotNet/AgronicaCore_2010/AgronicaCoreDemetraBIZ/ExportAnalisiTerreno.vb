
Imports System.Net
Imports System.Xml
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreDTOStd.InData.importazioni
Imports AgronicaCoreInterscambioBIZ
Imports AgronicaCoreModelsSTD.analisi
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

Public Class objAnalisiDemetra
    Public ListaAnalisiTerreno As List(Of AgronicaCoreDTOStd.InData.Demetra.AnalisiTerreno)
End Class

Public Class ExportAnalisiTerreno

    Public Class ParametriExtra
        Public DemetraBaseUrl As String
        Public apikey As String
        Public CUAA As String()

        Public FiltroEsportazione As Integer = -1
        Public Ambiente As String
    End Class

    Public Function Esporta(parametriExtra As String,
                            configServizio As Configurazione_Servizio,
                            objParametri_Super_Server As AgronicaCoreParametri,
                            objParametri_Server As AgronicaCoreParametri,
                            objParametri_Utenti As AgronicaCoreParametri,
                            ByRef Messaggio_di_Ritorno_Opzionale As String) As Boolean

        Dim result As Boolean = True

        Dim objDemetraBIZ_util As New Util

        Try
            Dim FlagTransazioneLocale As Boolean = False
            Dim FlagConnessioneLocale As Boolean = False

            Dim objLogAnalisi_R As New AgronicaLogAnalisi_R
            Dim objAnalisi_R As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_R
            Dim objInterscambioAnalisi_R As New Interscambio_Analisi_Testata_R
            Dim xImpCodR As New Imprese_Codici_Read

            Dim Parametri_Extra = JsonConvert.DeserializeObject(Of ParametriExtra)(parametriExtra)

            Dim piva_ammesse As New List(Of String)

            If Parametri_Extra.CUAA IsNot Nothing AndAlso Parametri_Extra.CUAA.Length > 0 Then
                For Each cuaa As String In Parametri_Extra.CUAA
                    piva_ammesse.Add(xImpCodR.Piva_from_CUAA(cuaa, objParametri_Server))
                Next
            End If

            Dim timeStamp As DateTime = DateTime.Now
            Dim dataTableResult As DataTable = objLogAnalisi_R.Analisi_NonInviate(enum_Esportazioni_Sistema_Cod.Demetra_Export_Analisi, piva_ammesse,
                                                                                  "", "",
                                                                                  objParametri_Server, enum_SistemiEsterni.demetra)

            Dim pacchettoDaInviare As ImportDemetra = Nothing

            Dim Analisi_Testata_Cod As String = ""
            Dim analisi_terreno As New AgronicaCoreModelsSTD.analisi.AnalisiTerreno
            Dim analisi_terreno_demetra As New AgronicaCoreDTOStd.InData.Demetra.AnalisiTerreno
            Dim datiDaEsportare As New objAnalisiDemetra

            Dim onUpdate As Boolean = False
            Dim retrial As Boolean = False
            Dim IDChiamata As Integer = -1

            Dim Piva As String = ""
            Dim TipoOperazione As enum_TipoOperazioneDB

            For Each row As DataRow In dataTableResult.Rows

                Try
                    ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server)

                    '------------------
                    '   PREP VARIAIBLI
                    '------------------
                    Dim esitoCorrente = Util_Costanti.ESITO_OK
                    Dim errorMessageCorrente = ""

                    Analisi_Testata_Cod = ""
                    Piva = ""
                    analisi_terreno = New AgronicaCoreModelsSTD.analisi.AnalisiTerreno
                    analisi_terreno_demetra = New AgronicaCoreDTOStd.InData.Demetra.AnalisiTerreno
                    datiDaEsportare.ListaAnalisiTerreno = New List(Of AgronicaCoreDTOStd.InData.Demetra.AnalisiTerreno)

                    Analisi_Testata_Cod = row("Analisi_Testata_Cod")
                    Piva = row("Piva")

                    If row("Esito") = Util_Costanti.ESITO_KO OrElse row("Esito") = Util_Costanti.ESITO_BLK Then
                        retrial = True
                        IDChiamata = row("ID_Chiamata")
                    Else
                        retrial = False
                        IDChiamata = -1
                    End If

                    TipoOperazione = row("Tipo_Operazione")
                    If TipoOperazione = enum_TipoOperazioneDB.Cancellazione Then
                        analisi_terreno.flag_cancellazione = True
                    ElseIf TipoOperazione = enum_TipoOperazioneDB.Modifica Then
                        onUpdate = True
                    Else
                        onUpdate = False
                    End If

                    'In extremis, se non trovo la piva dal log, vado a ricercarla tramite il modello o l'XML in cancellazione
                    If Piva = "" Then
                        'Se siamo in ins/upd e abbiamo quindi l'analisi modello in mano siamo in grado di estrarre la piva da una delle entità associate
                        If TipoOperazione <> enum_TipoOperazioneDB.Cancellazione Then
                            '---------------------
                            '   RECUPERO ANALISI
                            '---------------------
                            If TipoOperazione <> enum_TipoOperazioneDB.Cancellazione Then
                                analisi_terreno = AgronicaCoreAnagrafeBIZ.Analisi_Modello_R.Leggi_AnalisiTerreno_Modello(objParametri_Server.PivaSuperUser, Analisi_Testata_Cod, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                            End If

                            Piva = estrai_piva_analisi(analisi_terreno)

                        Else

                            Piva = estraiPiva_da_XML(row("object_data"), objParametri_Server)

                        End If

                    End If

                    Dim cuaa = xImpCodR.Leggi_CUAA(Piva, objParametri_Server)
                    If Not String.IsNullOrEmpty(cuaa) Then
                        'PROCEDO CON INVIO SE:
                        '- CUAA VALORIZZATO
                        '- LAT/LONG VALORIZZATI IN INS/UPD

                        '---------------------
                        '   RECUPERO ANALISI
                        '---------------------
                        If TipoOperazione <> enum_TipoOperazioneDB.Cancellazione Then
                            analisi_terreno = AgronicaCoreAnagrafeBIZ.Analisi_Modello_R.Leggi_AnalisiTerreno_Modello(objParametri_Server.PivaSuperUser, Analisi_Testata_Cod, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                        End If

                        '---------------------
                        '   PREP ANALISI
                        '---------------------
                        'copio tutte le proprietà dell'analisi GIAS sul modello Demetra (il modello Demetra è un sotto insieme del modello GIAS, hanno le stesse proprietà)
                        analisi_terreno_demetra = analisi_terreno.PropertyCopier(analisi_terreno_demetra)
                        analisi_terreno_demetra.codice = Analisi_Testata_Cod
                        analisi_terreno_demetra.utente_ultima_modifica = row("utente")

                        If analisi_terreno.certificatoAnalisi IsNot Nothing AndAlso analisi_terreno.certificatoAnalisi.numero_certificato <> "" Then
                            analisi_terreno_demetra.numero_certificato = analisi_terreno.certificatoAnalisi.numero_certificato
                        End If

                        '---------------------------
                        '   RECUPERO CHIAVE ESTERNA
                        '---------------------------
                        Dim chiave_esterna As DataTable = objInterscambioAnalisi_R.Leggi_Tabella_Interscambio_ChiaveGIAS(enum_SistemiEsterni.demetra, Analisi_Testata_Cod, objParametri_Server)
                        If chiave_esterna.Rows.Count = 0 Then
                            analisi_terreno_demetra.codice_esterno = ""
                        Else
                            analisi_terreno_demetra.codice_esterno = chiave_esterna.Rows(0)("Codice_Esterno")
                        End If

                        '---------------------------
                        '   ESTRAZIONE COORDINATE
                        '---------------------------
                        If TipoOperazione <> enum_TipoOperazioneDB.Cancellazione AndAlso (analisi_terreno_demetra.latitude Is Nothing OrElse analisi_terreno_demetra.latitude = 0 OrElse analisi_terreno_demetra.longitude Is Nothing OrElse analisi_terreno_demetra.longitude = 0) Then
                            'In esportazione è sempre necessaria la presenza di lat-long.
                            estraiCoordinate(analisi_terreno, analisi_terreno_demetra, objParametri_Server)
                        End If

                        'Se non siamo in cancellazione e manca anche solo una coordinata, blocco esportazione elemento
                        If (TipoOperazione <> enum_TipoOperazioneDB.Cancellazione AndAlso (analisi_terreno_demetra.latitude Is Nothing OrElse analisi_terreno_demetra.longitude Is Nothing)) Then
                            errorMessageCorrente = Util_Costanti.BLK_MESSAGE_PREFIX & " Coordinate analisi non corrette"
                            GoTo _BLK
                        End If

                        datiDaEsportare.ListaAnalisiTerreno.Add(analisi_terreno_demetra)

                        Dim tzh As New JsonSerializerSettings With {.DateFormatString = "yyyy-MM-ddT00:00:00Z"}
                        Dim datoCompresso As String = AgroZip.CompressioneBase64(1, JsonConvert.SerializeObject(datiDaEsportare, tzh))
                        pacchettoDaInviare = New ImportDemetra With {
                            .dati = datoCompresso,
                            .CUAA = cuaa
                        }

                        Dim errorMessage As String = ""
                        If Not objDemetraBIZ_util.CallEndpoint(Parametri_Extra.apikey, Parametri_Extra.DemetraBaseUrl, pacchettoDaInviare, errorMessage) Then
                            Throw New Exception(errorMessage)
                        End If

                    Else

                        errorMessageCorrente = Util_Costanti.BLK_MESSAGE_PREFIX & " CUAA non esistente"
_BLK:
                        esitoCorrente = Util_Costanti.ESITO_BLK

                        analisi_terreno_demetra.codice = row("Analisi_Testata_Cod")
                        datiDaEsportare.ListaAnalisiTerreno.Add(analisi_terreno_demetra)

                        Dim tzh As New JsonSerializerSettings With {.DateFormatString = "yyyy-MM-ddT00:00:00Z"}
                        Dim datoCompresso As String = AgroZip.CompressioneBase64(1, JsonConvert.SerializeObject(datiDaEsportare, tzh))

                        pacchettoDaInviare = New ImportDemetra With {
                            .dati = datoCompresso,
                            .CUAA = cuaa
                        }

                    End If

                    '--------------------------------------------
                    '   SCRITTURA LOG INVIO ANALISI 'OK' / 'BLK'
                    '--------------------------------------------
                    If retrial Then
                        Chiama_Update_Log_Invio_Chiamate(IDChiamata, pacchettoDaInviare, TipoOperazione, esitoCorrente, errorMessageCorrente, timeStamp, objParametri_Server)
                    Else
                        Chiama_Scrivi_Log_Invio_Analisi(Piva, pacchettoDaInviare, Analisi_Testata_Cod, analisi_terreno_demetra.codice_esterno, TipoOperazione, esitoCorrente, errorMessageCorrente, timeStamp, objParametri_Server)
                    End If

                Catch ex As Exception

                    Dim errorMessage = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

                    '------------------------------------
                    '   SCRITTURA LOG INVIO ANALISI 'KO'
                    '------------------------------------
                    If retrial Then
                        Chiama_Update_Log_Invio_Chiamate(IDChiamata, pacchettoDaInviare, TipoOperazione, Util_Costanti.ESITO_KO, ex.Message, timeStamp, objParametri_Server)
                    Else
                        Chiama_Scrivi_Log_Invio_Analisi(Piva, pacchettoDaInviare, Analisi_Testata_Cod, analisi_terreno_demetra.codice_esterno, TipoOperazione, Util_Costanti.ESITO_KO, ex.Message, timeStamp, objParametri_Server)
                    End If

                Finally

                    ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
                    ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

                End Try

            Next

            result = True

            If Parametri_Extra.FiltroEsportazione <> Enum_FiltroEsportazione_to_ElasticSearch.Nessuno Then
                Dim objLogger As New ExtractLog
                Dim Tipo_Esportazione As New List(Of String) From {
                    enum_Esportazioni_Sistema_Cod.Demetra_Export_Analisi
                }
                objLogger.ExtractLog(configServizio, Tipo_Esportazione, Parametri_Extra.FiltroEsportazione, Parametri_Extra.Ambiente, objParametri_Server, Messaggio_di_Ritorno_Opzionale)
            End If

        Catch ex As Exception
            result = False

            Messaggio_di_Ritorno_Opzionale &= " Errore durante l'esportazione delle analisi: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)
            objDemetraBIZ_util.Chiama_ScriviLOG(configServizio.Tipo_Sincro.ToString, Messaggio_di_Ritorno_Opzionale, configServizio.DirectoryLOG, configServizio.Tipo_Sincro.ToString & "_log.txt", objParametri_Server)

        End Try

        Return result

    End Function

    Private Shared Sub Chiama_Scrivi_Log_Invio_Analisi(Piva As String, pacchettoDaInviare As ImportDemetra, analisi_testata_cod As Integer, chiave_esterna As String, TipoOperazione As enum_TipoOperazioneDB, Esito As String, Dati_Ricevuti As String, Data_Invio As DateTime, objParametri_Server As AgronicaCoreParametri)

        Dim strPacchettoDaLoggare As String = ""

        Dim objLogInvioChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W

        If Not IsNothing(pacchettoDaInviare) Then
            Dim pacchettoDaLoggare As New ImportDemetra With {
                .CUAA = pacchettoDaInviare.CUAA,
                .dati = AgroZip.DeCompressioneBase64(1, pacchettoDaInviare.dati)
            }
            Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            strPacchettoDaLoggare = JsonConvert.SerializeObject(pacchettoDaLoggare, tzh)
        End If

        objLogInvioChiamate.Scrivi_Log_Invio_Analisi(enum_Esportazioni_Sistema_Cod.Demetra_Export_Analisi, strPacchettoDaLoggare, analisi_testata_cod, chiave_esterna, TipoOperazione, Esito, Dati_Ricevuti, objParametri_Server, UtilizzaTransazione:=False, Data_Invio, Piva:=Piva)

    End Sub

    Private Shared Sub Chiama_Update_Log_Invio_Chiamate(ID As Integer, pacchettoDaInviare As ImportDemetra, TipoOperazione As enum_TipoOperazioneDB, Esito As String, Dati_Ricevuti As String, Data_Invio As DateTime, objParametri_Server As AgronicaCoreParametri)

        Dim strPacchettoDaLoggare As String = ""

        Dim objLogInvioChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W

        If Not IsNothing(pacchettoDaInviare) Then
            Dim pacchettoDaLoggare As New ImportDemetra With {
                .CUAA = pacchettoDaInviare.CUAA,
                .dati = AgroZip.DeCompressioneBase64(1, pacchettoDaInviare.dati)
            }

            Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            strPacchettoDaLoggare = JsonConvert.SerializeObject(pacchettoDaLoggare, tzh)
        End If

        objLogInvioChiamate.Update_Log_Invio_Chiamate(ID, enum_Esportazioni_Sistema_Cod.Demetra_Export_Analisi, strPacchettoDaLoggare, TipoOperazione, Esito, Dati_Ricevuti, objParametri_Server, Data_Invio)

    End Sub

    ''''
    ''' <summary>
    ''' SOLO IN CASO DI CANCELLAZIONE ANDIAMO A RECUPERARE IL CUAA DA object_data
    ''' </summary>
    ''' <param name="DatiAnalisi"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    Private Shared Function estraiPiva_da_XML(DatiAnalisi As String, objParametri_Server As AgronicaCoreParametri) As String
        Dim piva As String = ""

        If DatiAnalisi <> "" Then

            '-------------------------------
            ' XML
            '-------------------------------
            Try
                Dim XmlDoc As XmlDocument

                Dim XmlDatiTestata As XmlElement
                Dim xTestata As XmlElement
                Dim xDatiAnalisi_EntitaxTestata As XmlElement
                Dim xAnalisi_EntitaxTestata As XmlElement

                XmlDoc = New Xml.XmlDocument
                XmlDoc.LoadXml(DatiAnalisi)

                '-------------------------------
                ' PRELEVO DATI 
                '-------------------------------
                XmlDatiTestata = XmlDoc.GetElementsByTagName("DatiTestate").Item(0)

                '-------------------------------
                '   PRELEVO TESTATA ANALISI
                '-------------------------------
                xTestata = XmlDatiTestata.GetElementsByTagName("Testata").Item(0)

                '-------------------------------
                ' PRELEVO ANALISI_ENTITAXTESTATA
                '-------------------------------
                xDatiAnalisi_EntitaxTestata = xTestata.GetElementsByTagName("DatiAnalisi_EntitaxTestata").Item(0)
                'Se c'è un solo elemento, estraggo la piva
                If xDatiAnalisi_EntitaxTestata.GetElementsByTagName("Analisi_EntitaxTestata").Count = 1 Then
                    xAnalisi_EntitaxTestata = xDatiAnalisi_EntitaxTestata.GetElementsByTagName("Analisi_EntitaxTestata").Item(0)

                    '-------------------------------
                    ' ESTRAGGO LA PIVA
                    '-------------------------------
                    piva = CStr(xAnalisi_EntitaxTestata.GetAttribute("piva"))
                End If

            Catch ex As Exception
                piva = ""
            End Try

            '-------------------------------
            ' MODELLO
            '-------------------------------
            If piva = "" Then
                Try
                    Dim objAnalisiModello As AgronicaCoreModelsSTD.analisi.AnalisiTerreno = JsonConvert.DeserializeObject(DatiAnalisi)
                    If objAnalisiModello IsNot Nothing Then
                        If objAnalisiModello.entitaImprese IsNot Nothing AndAlso objAnalisiModello.entitaImprese.Count = 1 Then
                            'Se c'è un solo elemento, estraggo la piva
                            piva = objAnalisiModello.entitaImprese(0).elementoAnagrafico.partitaIva
                        End If
                    End If

                Catch ex As Exception
                    piva = ""
                End Try

            End If
        End If

        Return piva

    End Function

    Private Shared Function estrai_piva_analisi(analisi_terreno As AnalisiTerreno) As String

        If analisi_terreno.entitaImprese IsNot Nothing AndAlso analisi_terreno.entitaImprese.Count > 0 Then
            Return analisi_terreno.entitaImprese(0).elementoAnagrafico.partitaIva
        End If
        If analisi_terreno.entitaCentri IsNot Nothing AndAlso analisi_terreno.entitaCentri.Count > 0 Then
            Return analisi_terreno.entitaCentri(0).elementoAnagrafico.primaryKey.partitaIva
        End If
        If analisi_terreno.entitaCampi IsNot Nothing AndAlso analisi_terreno.entitaCampi.Count > 0 Then
            Return analisi_terreno.entitaCampi(0).elementoAnagrafico.primaryKey.centroAziendalePK.partitaIva

        End If
        If analisi_terreno.entitaAppezzamenti IsNot Nothing AndAlso analisi_terreno.entitaAppezzamenti.Count > 0 Then
            Return analisi_terreno.entitaAppezzamenti(0).elementoAnagrafico.primaryKey.centroAziendalePK.partitaIva

        End If
        If analisi_terreno.entitaImpianti IsNot Nothing AndAlso analisi_terreno.entitaImpianti.Count > 0 Then
            Return analisi_terreno.entitaImpianti(0).elementoAnagrafico.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva

        End If
        If analisi_terreno.entitaFabbricati IsNot Nothing AndAlso analisi_terreno.entitaFabbricati.Count > 0 Then
            Return analisi_terreno.entitaFabbricati(0).elementoAnagrafico.primaryKey.centroAziendalePK.partitaIva
        End If

        Return ""

    End Function

    Private Shared Sub estraiCoordinate(analisi_terreno As AnalisiTerreno, ByRef analisi_terreno_demetra As AgronicaCoreDTOStd.InData.Demetra.AnalisiTerreno, objParametri_Server As AgronicaCoreParametri)

        'Provo a pescarlo dal primo campione già presente nelle analisi
        If analisi_terreno.campioni IsNot Nothing AndAlso analisi_terreno.campioni.Count > 0 Then
            'In caso di modica impostiamo sul primo campione le coordinate che ci hanno passato
            analisi_terreno_demetra.latitude = analisi_terreno.campioni.OrderBy(Function(c) c.codice)(0).latitude
            analisi_terreno_demetra.longitude = analisi_terreno.campioni.OrderBy(Function(c) c.codice)(0).longitude
        End If

        'Se nei campioni non ho recuperato anche solo una delle due coordinate, leggo dal GIS
        If (analisi_terreno_demetra.latitude Is Nothing OrElse analisi_terreno_demetra.latitude = 0) OrElse (analisi_terreno_demetra.longitude Is Nothing OrElse analisi_terreno_demetra.longitude = 0) Then
            'Leggo a scalare dai Impianto, Appezza, Particella
            Dim found As Boolean = False
            Dim objGIS As New AgronicaCoreGisDAL.GIS_Entita_R

            Dim Piva As String = ""
            Dim Sa_Cod As Integer = 0
            Dim Appezza As Integer = 0
            Dim Id_Imp As Integer = 0

            Dim PROV As String = ""
            Dim COM As String = ""
            Dim SEZIONE As String = ""
            Dim FOGLIO As Integer = 0
            Dim NUMERO As Integer = 0
            Dim SUBALTERNO As String = ""

            'recupero il punto associato al primo impianto dell'impresa
            If analisi_terreno.entitaImprese IsNot Nothing AndAlso analisi_terreno.entitaImprese.Count > 0 Then
                Piva = analisi_terreno.entitaImprese(0).elementoAnagrafico.partitaIva

                found = objGIS.LeggiWKT_xAnalisiDemetra(Piva, analisi_terreno_demetra.latitude, analisi_terreno_demetra.longitude, enum_Gis_LayerElementiGrafici_std.IMPIANTI, objParametri_Server)
            End If

            'recupero il punto associato al primo impianto del centro
            If analisi_terreno.entitaCentri IsNot Nothing AndAlso analisi_terreno.entitaCentri.Count > 0 Then
                Piva = analisi_terreno.entitaCentri(0).elementoAnagrafico.primaryKey.partitaIva
                Sa_Cod = analisi_terreno.entitaCentri(0).elementoAnagrafico.primaryKey.codice

                found = objGIS.LeggiWKT_xAnalisiDemetra(Piva, analisi_terreno_demetra.latitude, analisi_terreno_demetra.longitude, enum_Gis_LayerElementiGrafici_std.IMPIANTI, objParametri_Server)
            End If

            'provo a recuperare un punto dal primo impianto associato
            If analisi_terreno.entitaImpianti IsNot Nothing AndAlso analisi_terreno.entitaImpianti.Count > 0 Then
                Piva = analisi_terreno.entitaImpianti(0).elementoAnagrafico.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
                Sa_Cod = analisi_terreno.entitaImpianti(0).elementoAnagrafico.primaryKey.appezzamentoPK.centroAziendalePK.codice
                Appezza = analisi_terreno.entitaImpianti(0).elementoAnagrafico.primaryKey.appezzamentoPK.codice
                Id_Imp = analisi_terreno.entitaImpianti(0).elementoAnagrafico.primaryKey.codice

                found = objGIS.LeggiWKT_xAnalisiDemetra(Piva, analisi_terreno_demetra.latitude, analisi_terreno_demetra.longitude, enum_Gis_LayerElementiGrafici_std.IMPIANTI, objParametri_Server, Sa_Cod, Appezza, Id_Imp)
            End If

            'provo a recuperare un punto dal primo appezzamento associato
            If Not found AndAlso analisi_terreno.entitaAppezzamenti IsNot Nothing AndAlso analisi_terreno.entitaAppezzamenti.Count > 0 Then
                Piva = analisi_terreno.entitaAppezzamenti(0).elementoAnagrafico.primaryKey.centroAziendalePK.partitaIva
                Sa_Cod = analisi_terreno.entitaAppezzamenti(0).elementoAnagrafico.primaryKey.centroAziendalePK.codice
                Appezza = analisi_terreno.entitaAppezzamenti(0).elementoAnagrafico.primaryKey.codice

                found = objGIS.LeggiWKT_xAnalisiDemetra(Piva, analisi_terreno_demetra.latitude, analisi_terreno_demetra.longitude, enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI, objParametri_Server, Sa_Cod, Appezza)
            End If

            'provo a recuperare un punto dalla prima particella associata
            If Not found AndAlso analisi_terreno.entitaParticelleCatastali IsNot Nothing AndAlso analisi_terreno.entitaParticelleCatastali.Count > 0 Then
                Dim centro = analisi_terreno.entitaParticelleCatastali(0).elementoAnagrafico.centro
                Dim particella = analisi_terreno.entitaParticelleCatastali(0).elementoAnagrafico.particella
                PROV = particella.primaryKey.Prov
                COM = particella.primaryKey.Com
                SEZIONE = particella.primaryKey.Sezione
                FOGLIO = particella.primaryKey.Foglio
                NUMERO = particella.primaryKey.Numero
                SUBALTERNO = particella.primaryKey.Subalterno

                found = objGIS.LeggiWKT_xAnalisiDemetra(centro.partitaIva, analisi_terreno_demetra.latitude, analisi_terreno_demetra.longitude, enum_Gis_LayerElementiGrafici_std.CATASTO, objParametri_Server, centro.codice, 0, 0, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO)
            End If
        End If

    End Sub

End Class
