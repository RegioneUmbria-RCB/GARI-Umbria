Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreDTOStd.InData.Demetra
Imports AgronicaCoreDTOStd.InData.importazioni
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class ExtractLog

    Public Function ExtractLog(ByVal configServizio As AgronicaCoreVarieDAL.Configurazione_Servizio,
                               ByVal Tipo_Esportazione As List(Of String),
                               ByVal FiltroImportati As Enum_FiltroEsportazione_to_ElasticSearch,
                               ByVal Enviroment As String,
                               ByVal objParametri_Server As AgronicaCoreParametri,
                               ByRef Messaggio_di_Ritorno_Opzionale As String) As Boolean

        Dim objChiamateR As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_R
        Dim objChiamateW As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
        Dim objAppDatiW As New AgronicaCoreContabDAL.APP_Dati_W

        Dim dtIsEmpty As Boolean = False

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim urlColdirettiLogger As String = objConfigSiti.Leggi_Valore(16, "Coldiretti_ElasticSearchUrl", "", "", objParametri_Server)

        Dim currentTipoEsportazione As String = 0
        Dim currentIDLogInvioChiamata As String = 0

        If urlColdirettiLogger <> "" Then
            '08/07/24 Puntiamo ad un nuovo endpoint gias.IdEsportazione. Su configurazione siti rimane quello originale gias.generico, sostituisco il generico con il nuovo ID
            'in questo caso, L'id viene aggiunto poco prima di chiamare l'endpoint 
            Dim objLogger As New ElasticSearchLogger(Enviroment, urlColdirettiLogger.Replace("generico", String.Empty))

            Dim xFiltroAggiuntivo As String = ""
            If Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Export_LavoratoriQDC) OrElse Tipo_Esportazione.Count = 0 Then
                'Per questo tipo esportazione escludiamo i KO StatusCode=PreconditionFailed --> Il lavoratore attende il processing del fornitore
                xFiltroAggiuntivo &= "Report.Dati_Ricevuti NOT LIKE '%StatusCode=PreconditionFailed%'"
            End If

            Dim PivaList As New List(Of String)
            Dim dicPivaxCUAA As New Dictionary(Of String, String) 'Piva, CUAA

            Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read


            Do Until dtIsEmpty 'Estraiamo 500 record alla volta, finché non ci sono più righe
                Try
                    Dim dataTableResult = objChiamateR.Consulta_Log_Interscambio(Tipo_Esportazione,
                                                                                 FiltroImportati,
                                                                                 xFiltroAggiuntivo,
                                                                                 "",
                                                                                 objParametri_Server,
                                                                                 isElasticSearch:=True)

                    If dataTableResult.Rows.Count > 0 Then

                        Dim listaInviati_Chiamate As New List(Of Integer)
                        Dim listaInviati_AppDati As New List(Of String)

                        Dim timestamp = Now

                        'Se devo inviare log di import Attività, mi estraggo tutti i cuaa
                        If Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Import_Attivita) OrElse Tipo_Esportazione.Count = 0 Then
                            If dataTableResult.Select("Tipo_Esportazione = " & (enum_Esportazioni_Sistema_Cod.Demetra_Import_Attivita)).Length > 0 Then
                                Dim dtImportAttivita = dataTableResult.Select("Tipo_Esportazione = " & (enum_Esportazioni_Sistema_Cod.Demetra_Import_Attivita)).CopyToDataTable()
                                If dtImportAttivita IsNot Nothing AndAlso dtImportAttivita.Rows.Count > 0 Then
                                    For Each r As DataRow In dtImportAttivita.Rows
                                        Try
                                            If r("Piva") <> "" Then
                                                PivaList.Add(r("Piva"))
                                            End If
                                        Catch ex As Exception
                                        End Try
                                    Next

                                    '---------------------
                                    ' ESTRAZIONE CUAA
                                    '---------------------
                                    If PivaList.Count > 0 Then
                                        Dim AlmenoUnaNuovaPiva As Boolean = False
                                        PivaList = PivaList.Distinct().ToList()
                                        For Each piva In PivaList
                                            'Potrei essere alla nEsima lettura dei log, verifico se ci sono nuove pive prima di effettuare la lettura massiva dei CUAA
                                            If Not dicPivaxCUAA.ContainsKey(piva) Then
                                                AlmenoUnaNuovaPiva = True
                                                Exit For
                                            End If
                                        Next

                                        If AlmenoUnaNuovaPiva Then
                                            Dim dt = xImpCodR.CUAA_from_Piva_Massivo(PivaList.Distinct().ToList(), objParametri_Server)
                                            If dt.Rows.Count > 0 Then
                                                For Each row_Impresa In dt.Rows
                                                    If Not dicPivaxCUAA.ContainsKey(row_Impresa.Item("Piva")) Then
                                                        dicPivaxCUAA.Add(row_Impresa.Item("Piva"), row_Impresa.Item("CUAA"))
                                                    End If
                                                Next
                                            End If
                                        End If

                                    End If
                                End If
                            End If
                        End If

                        For Each row As DataRow In dataTableResult.Rows

                            Dim Dati_Inviati As String = If(IsDBNull(row("Dati_Inviati")), "", row("Dati_Inviati"))
                            Dim Dati_Ricevuti As String = If(IsDBNull(row("Dati_Ricevuti")), "", row("Dati_Ricevuti"))

                            Dim Esito As String = If(IsDBNull(row("Esito")), "", row("Esito"))

                            Dim Data_Ora_Invio As DateTime = If(IsDBNull(row("Data_Ora_Invio")), AGRODATAINIZIO, row("Data_Ora_Invio"))

                            Dim TipoEsportazione = row("Tipo_Esportazione")
                            currentTipoEsportazione = TipoEsportazione

                            Dim Componente As String = If(IsDBNull(row("Tipo")), "", row("Tipo"))

                            Dim Operazione As String = If(IsDBNull(row("Tipo_Operazione")), "", row("Tipo_Operazione"))

                            Dim ID_Log_Invio As String = row("ID_Log_Invio")
                            currentIDLogInvioChiamata = ID_Log_Invio

                            Dim CUAA As String = ""
                            Dim payloadToES As Object = Nothing

                            If Not IsNothing(Dati_Inviati) AndAlso Not Dati_Inviati.Equals("") Then
                                Try
                                    Select Case TipoEsportazione
                                        Case enum_Esportazioni_Sistema_Cod.Demetra_Export_MovimentiMag, enum_Esportazioni_Sistema_Cod.Demetra_Export_Fornitori,
                                             enum_Esportazioni_Sistema_Cod.Demetra_Import_Fornitori, enum_Esportazioni_Sistema_Cod.Demetra_Import_MovimentiMag

                                            'Utilizziamo l'oggetto che viene loggato in export, PIVA contiene il realtà il CUAA
                                            Dim obj = JsonConvert.DeserializeObject(Of JObject)(Dati_Inviati)
                                            If obj("piva").ToString() IsNot Nothing AndAlso obj("piva").ToString() <> "" Then
                                                CUAA = obj("piva").ToString()
                                            End If

                                            Dim payloadToES_XML As New payloadToES_XML With {.piva = obj("piva").ToString(), .dati = obj("dati").ToString()}
                                            payloadToES = payloadToES_XML
                                        Case enum_Esportazioni_Sistema_Cod.Demetra_Import_Attivita
                                            If (row("Piva") <> "") Then
                                                If dicPivaxCUAA.ContainsKey(row("Piva")) Then
                                                    CUAA = dicPivaxCUAA(row("Piva"))
                                                End If
                                            End If

                                            'Trasformo gli array vuoti in null, così che possano essere rimossi in serializzazione
                                            Dati_Inviati = Dati_Inviati.Replace("[]", "null")
                                            Dim attivita = JsonConvert.DeserializeObject(Of List(Of AgronicaCoreModelsSTD.attivita.Attivita))(Dati_Inviati)

                                            'Serializzo in un oggetto generico che non contiene proprietà null
                                            Dim noNUllObj = JsonConvert.SerializeObject(attivita, Formatting.None, New JsonSerializerSettings With {.NullValueHandling = NullValueHandling.Ignore})

                                            Dim payloadToES_JSON As New payloadToES_JSON With {.CUAA = CUAA, .dati = JsonConvert.DeserializeObject(Of Object)(noNUllObj)}
                                            payloadToES = payloadToES_JSON

                                        Case Else
                                            Dim objDatiInviati = JsonConvert.DeserializeObject(Of ImportDemetra)(Dati_Inviati)
                                            CUAA = objDatiInviati.CUAA

                                            'Trasformo gli array vuoti in null, così che possano essere rimossi in serializzazione
                                            objDatiInviati.dati = objDatiInviati.dati.Replace("[]", "null")

                                            'Devo trasformarlo in un oggetto tipizzato del tipo corretto, per poter rimuovere i backslash
                                            Dim obj = Nothing
                                            Select Case TipoEsportazione
                                                Case enum_Esportazioni_Sistema_Cod.Demetra_Import_Analisi, enum_Esportazioni_Sistema_Cod.Demetra_Export_Analisi
                                                    obj = JsonConvert.DeserializeObject(Of objAnalisiDemetra)(objDatiInviati.dati)

                                                Case enum_Esportazioni_Sistema_Cod.Demetra_Export_Attivita
                                                    obj = JsonConvert.DeserializeObject(Of List(Of AgronicaCoreDTOStd.InData.Demetra.Attivita))(objDatiInviati.dati)

                                                Case enum_Esportazioni_Sistema_Cod.Demetra_Import_Fabbricati
                                                    obj = JsonConvert.DeserializeObject(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato)(objDatiInviati.dati)
                                                Case enum_Esportazioni_Sistema_Cod.Demetra_Export_Fabbricati
                                                    obj = JsonConvert.DeserializeObject(Of AnagraficaWrapper(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato))(objDatiInviati.dati)

                                                Case enum_Esportazioni_Sistema_Cod.Demetra_Import_Macchine
                                                    obj = JsonConvert.DeserializeObject(Of Equipaggiamento)(objDatiInviati.dati)
                                                Case enum_Esportazioni_Sistema_Cod.Demetra_Export_Macchine
                                                    obj = JsonConvert.DeserializeObject(Of AnagraficaWrapper(Of Equipaggiamento))(objDatiInviati.dati)

                                                Case enum_Esportazioni_Sistema_Cod.Demetra_Import_LavoratoriQDC
                                                    obj = JsonConvert.DeserializeObject(Of Contatto)(objDatiInviati.dati)
                                                Case enum_Esportazioni_Sistema_Cod.Demetra_Export_LavoratoriQDC
                                                    obj = JsonConvert.DeserializeObject(Of AnagraficaWrapper(Of Contatto))(objDatiInviati.dati)
                                            End Select

                                            'Serializzo in un oggetto generico che non contiene proprietà null
                                            Dim noNUllObj = JsonConvert.SerializeObject(obj, Formatting.None, New JsonSerializerSettings With {.NullValueHandling = NullValueHandling.Ignore})

                                            Dim payloadToES_JSON As New payloadToES_JSON With {.CUAA = objDatiInviati.CUAA, .dati = JsonConvert.DeserializeObject(Of Object)(noNUllObj)}
                                            payloadToES = payloadToES_JSON

                                    End Select
                                Catch ex As Exception
                                    CUAA = ""
                                    payloadToES = Nothing
                                End Try
                            End If

                            'Invio ad ElasticSearch --> se OK, mi salvo l'ID del log per fare l'update di inviato e datainvio 
                            Dim res = objLogger.WriteLog(CUAA, Operazione, Componente, TipoEsportazione, Esito, Dati_Ricevuti, payloadToES, Data_Ora_Invio, timestamp)
                            If res Then
                                If TipoEsportazione = enum_Esportazioni_Sistema_Cod.Demetra_Import_Attivita Then
                                    listaInviati_AppDati.Add(ID_Log_Invio)
                                Else
                                    listaInviati_Chiamate.Add(CInt(ID_Log_Invio))
                                End If
                            End If
                        Next

                        If listaInviati_Chiamate.Count > 0 Then
                            objChiamateW.UpdateMassivo_Inviato_ElasticSearch(listaInviati_Chiamate, timestamp, objParametri_Server)
                        End If

                        If listaInviati_AppDati.Count > 0 Then
                            objAppDatiW.UpdateMassivo_Inviato_ElasticSearch(listaInviati_AppDati, timestamp, objParametri_Server)
                        End If

                    Else
                        dtIsEmpty = True
                    End If

                Catch ex As Exception

                    Messaggio_di_Ritorno_Opzionale &= "[Current Tipo_Esportazione: " & currentTipoEsportazione.ToString() & " - ID: " & currentIDLogInvioChiamata.ToString() & "] Errore durante l'esportazione dei log ad ElasticSearch: " & vbCrLf & "Errore -->" & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)

                    'Concateno i tipi di esportazione da inviare ad ElasticSearch
                    Dim strTipoEsportazione As String = "TipoExportID: "
                    If Tipo_Esportazione IsNot Nothing AndAlso Tipo_Esportazione.Count > 0 Then
                        strTipoEsportazione &= String.Join("-", Tipo_Esportazione)
                    Else
                        strTipoEsportazione &= "Tutti"
                    End If

                    'Concateno il tipo di esito da inviare ad ElasticSearch
                    strTipoEsportazione &= "; ExportFilter: " & FiltroImportati.ToString()

                    Dim objDemetraBIZ_util As New Util
                    objDemetraBIZ_util.Chiama_ScriviLOG("ElasticSearch: " & configServizio.Tipo_Sincro.ToString & " [" & strTipoEsportazione & "]", Messaggio_di_Ritorno_Opzionale, configServizio.DirectoryLOG, configServizio.Tipo_Sincro.ToString & "_log.txt", objParametri_Server, verificaInviaElasticSearch:=False)

                    Return False
                End Try
            Loop

        End If

        Return True

    End Function
End Class

Public Class payloadToES_JSON
    Public Property CUAA As String
    Public Property dati As Object
End Class
Public Class payloadToES_XML
    Public Property piva As String
    Public Property dati As Object
End Class