Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.GiasAPP
Imports Newtonsoft.Json.Linq

Public Class Agronica_Log_Invio_Chiamate_R
    Private Enum enum_Esito
        OK = 0
        KO = 1
        BLK = 2
    End Enum
    Private Enum enum_Tipo_Operazione
        Aggiornamento = 0
        Cancellazione = 1
        Inserimento = 2
    End Enum

    Public Function Consulta_Log_Interscambio(ByVal Tipi As List(Of String),
                                              ByVal Data_Inizio As Date,
                                              ByVal Data_Fine As Date,
                                              ByVal FiltroImportati As Integer,
                                              ByVal Piva_CUAA As String,
                                              objParametri_Server As AgronicaCoreParametri
                                              ) As List(Of ConsultaSincroLog)

        Dim objChiamate As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_R
        'NOI: Ci basiamo su un interfaccia già esistente (consultazione invii app), i nomi delle proprietà non sono adattissimi al contesto
        Dim result As New List(Of ConsultaSincroLog)
        Dim objUtentiVisibilitaR As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim DtImpreseVisibili As DataTable

        Dim dt = objChiamate.Consulta_Log_Interscambio(Tipi,
                                                       FiltroImportati,
                                                       "",
                                                       "",
                                                       objParametri_Server,
                                                       Data_Inizio,
                                                       Data_Fine,
                                                       Piva_CUAA)

        'mi tiro fuori le aziende visibili all'utente
        DtImpreseVisibili = objUtentiVisibilitaR.Leggi(enum_TipoEntita.Impresa, "", "", objParametri_Server)

        dt.Columns.Add(New DataColumn("CUAA", GetType(String)))

        Dim righe = dt.ToExpandoObject()

        If righe.Count > 0 Then

            Dim dicCUAAxImpresa As New Dictionary(Of String, (String, String)) 'CUAA, (Piva, Rag_Soc)
            Dim dicPivaxRagSoc As New Dictionary(Of String, String) 'Piva, Rag_Soc

            Dim CUAAList As New List(Of String)
            Dim PivaList As New List(Of String)

            Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim xImpR As New AgronicaCoreAnagrafeDAL.Imprese_Read

            For Each row In righe

                Dim riga As New ConsultaSincroLog
                riga.utente = If(IsDBNull(row("utente")), "", row("utente"))
                riga.data_sincro = If(IsDBNull(row("Data_Ora_Invio")), AGRODATAINIZIO, row("Data_Ora_Invio"))
                riga.tipo_dato = row("Tipo_Esportazione")

                Select Case riga.tipo_dato
                    Case enum_Esportazioni_Sistema_Cod.Demetra_Export_MovimentiMag, enum_Esportazioni_Sistema_Cod.Demetra_Import_MovimentiMag,
                         enum_Esportazioni_Sistema_Cod.Demetra_Import_Fornitori, enum_Esportazioni_Sistema_Cod.Demetra_Export_Fornitori
                        'SONO XML
                        riga.Dati = If(IsDBNull(row("Dati_Inviati")), "", row("Dati_Inviati").ToString().Replace("\r", "").Replace("\n", "").Replace("\t", ""))
                    Case Else
                        'SONO OGGETTI
                        riga.Dati = If(IsDBNull(row("Dati_Inviati")), "", row("Dati_Inviati").ToString().Replace("\", ""))
                End Select

                Dim descrizione As String = ""
                Dim Chiave_GIAS As String = If(Not IsDBNull(row("Chiave_GIAS")) AndAlso row("Chiave_GIAS") <> "", "Chiave interna: " & row("Chiave_GIAS"), "")
                Dim Chiave_Demetra As String = If(Not IsDBNull(row("Chiave_Esterna")) AndAlso row("Chiave_Esterna") <> "", "Chiave esterna: " & row("Chiave_Esterna"), "")

                riga.descrizione = If(Chiave_GIAS <> "" AndAlso Chiave_Demetra <> "", Chiave_GIAS & " | " & Chiave_Demetra, Chiave_GIAS & Chiave_Demetra)

                riga.Riferimento = If(IsDBNull(row("Chiave_GIAS")), "", row("Chiave_GIAS"))

                Select Case row("Tipo_Operazione")
                    Case "INS"
                        riga.tipo_aggiornamento = enum_Tipo_Operazione.Inserimento
                    Case "DEL"
                        riga.tipo_aggiornamento = enum_Tipo_Operazione.Cancellazione
                    Case Else
                        riga.tipo_aggiornamento = enum_Tipo_Operazione.Aggiornamento
                End Select

                Select Case row("Esito")
                    Case "OK"
                        riga.stato_applicazione = enum_Esito.OK
                    Case "KO"
                        riga.stato_applicazione = enum_Esito.KO
                    Case "BLK"
                        riga.stato_applicazione = enum_Esito.BLK
                End Select

                riga.errori = row("Dati_Ricevuti")

                'Lo faccio solo alla prima iterazione
                If righe.IndexOf(row) = 0 Then
                    'DEVO ESTRARRE I CUAA E LE PIVA DA TUTTE LE RIGHE
                    For Each r In righe
                        Dim tipo_dato = r("Tipo_Esportazione")
                        r("CUAA") = ""
                        Try
                            Select Case tipo_dato
                                Case enum_Esportazioni_Sistema_Cod.Demetra_Export_MovimentiMag, enum_Esportazioni_Sistema_Cod.Demetra_Import_MovimentiMag,
                                     enum_Esportazioni_Sistema_Cod.Demetra_Import_Fornitori
                                    'Utilizziamo l'oggetto che viene loggato in export, PIVA contiene il realtà il CUAA
                                    Dim dati = If(IsDBNull(r("Dati_Inviati")), "", r("Dati_Inviati"))
                                    If Not IsNothing(dati) AndAlso Not dati.Equals("") Then
                                        Dim objToApi = JsonConvert.DeserializeObject(Of JObject)(dati)
                                        If objToApi("piva").ToString() IsNot Nothing AndAlso objToApi("piva").ToString() <> "" Then
                                            CUAAList.Add(objToApi("piva").ToString())
                                            r("CUAA") = objToApi("piva").ToString()
                                        End If
                                    End If

                                Case enum_Esportazioni_Sistema_Cod.Demetra_Import_DataPublish
                                    'ESTRIAMO LA PIVA DAL CUAA DELLA CHIAVE
                                    Dim dummy As String() = r("Chiave_GIAS").split("_")
                                    If dummy.Length = 2 Then
                                        r("CUAA") = dummy(1)
                                        CUAAList.Add(r("CUAA"))
                                    End If

                                Case Else
                                    If r("Piva") <> "" Then
                                        PivaList.Add(r("Piva"))
                                    End If
                            End Select
                        Catch ex As Exception
                        End Try
                    Next

                    '---------------------
                    ' ESTRAZIONE AZIENDE
                    '---------------------
                    If CUAAList.Count > 0 Then
                        CUAAList = CUAAList.Distinct().ToList()
                        Dim dtImprese = xImpCodR.Leggi_Imprese_From_CUAA(CUAAList, objParametri_Server)

                        If dtImprese.Rows.Count > 0 Then
                            For Each row_Impresa In dtImprese.Rows
                                dicCUAAxImpresa.Add(row_Impresa.ITEM("CUAA"), (row_Impresa.Item("Piva"), row_Impresa.Item("Rag_Soc")))
                            Next
                        End If
                    End If

                    If PivaList.Count > 0 Then
                        PivaList = PivaList.Distinct().ToList()
                        Dim dtImprese = xImpR.RagSoc_from_Piva_Massivo(PivaList.Distinct().ToList(), objParametri_Server)

                        If dtImprese.Rows.Count > 0 Then
                            For Each row_Impresa In dtImprese.Rows
                                dicPivaxRagSoc.Add(row_Impresa.Item("Piva"), row_Impresa.Item("Rag_Soc"))
                            Next
                        End If
                    End If
                End If

                Dim CUAA As String = If(IsDBNull(row("CUAA")), "", row("CUAA"))
                If CUAA <> "" AndAlso dicCUAAxImpresa.ContainsKey(CUAA) Then
                    riga.azienda_cod = dicCUAAxImpresa(CUAA).Item1
                    riga.azienda_des = dicCUAAxImpresa(CUAA).Item2
                End If

                Dim Piva As String = If(IsDBNull(row("Piva")), "", row("Piva"))
                If Piva <> "" AndAlso dicPivaxRagSoc.ContainsKey(Piva) Then
                    riga.azienda_cod = Piva
                    riga.azienda_des = dicPivaxRagSoc(Piva)
                End If

                'possiamo inserire qui il controllo sulla visibilità aziendale, visto che vengono tirate fuori qui le Pive e le CUAA
                If DtImpreseVisibili IsNot Nothing AndAlso DtImpreseVisibili.Rows.Count > 0 Then

                    Dim listaPiva As New List(Of String)
                    For Each r As DataRow In DtImpreseVisibili.Rows
                        listaPiva.Add(r("PIVA").ToString())
                    Next

                    If listaPiva.Contains(riga.azienda_cod) Then
                        result.Add(riga)
                    End If

                Else
                    result.Add(riga)
                End If

            Next

        End If

        Return result

    End Function
End Class
Public Class Agronica_Log_Invio_Chiamate_W

    Public Sub Scrivi_Log_Invio_Analisi(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                                        Dati_Inviati As String,
                                        Analisi_Testata_Cod As Integer,
                                        Chiave_Esterna As String,
                                        TipoOperazione As enum_TipoOperazioneDB,
                                        Esito As String,
                                        Dati_Ricevuti As String,
                                        objParametri_Server As AgronicaCoreParametri,
                                            Optional UtilizzaTransazione As Boolean = True,
                                            Optional Data_Invio As DateTime? = Nothing,
                                            Optional Piva As String = "")

        Dim objLogInvioChiamate As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
        Dim objLogInvioAnalisi As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Analisi_W

        Dim Tipo_Operazione As String = ""
        Select Case TipoOperazione
            Case enum_TipoOperazioneDB.Scrittura
                Tipo_Operazione = "INS"
            Case enum_TipoOperazioneDB.Modifica
                Tipo_Operazione = "UPD"
            Case enum_TipoOperazioneDB.Cancellazione
                Tipo_Operazione = "DEL"
        End Select

        Dim ID_LogInvioChiamata = objLogInvioChiamate.Scrivi(Tipo_Esportazione,
                                                             Dati_Inviati, Data_Invio,
                                                             Esito, Dati_Ricevuti,
                                                             "0", Tipo_Operazione,
                                                             objParametri_Server,
                                                             UtilizzaTransazione:=UtilizzaTransazione)

        objLogInvioAnalisi.Scrivi(Tipo_Esportazione, ID_LogInvioChiamata, 0,
                                  Analisi_Testata_Cod, Chiave_Esterna,
                                  objParametri_Server, Piva:=Piva)
    End Sub

    Public Sub Scrivi_Log_Invio_Anagrafe(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                                         Dati_Inviati As String,
                                         Tipo As String,
                                         Chiave As String,
                                         Chiave_Esterna As String,
                                         Piva As String,
                                         Sa_Cod As Integer,
                                         Campo_Cod As Integer,
                                         Appezza As Integer,
                                         Id_Reg As Integer,
                                         Progetto_Cod As Integer,
                                         Fabbricato_Cod As Integer,
                                         Note As String,
                                         TipoOperazione As enum_TipoOperazioneDB,
                                         Esito As String,
                                         Dati_Ricevuti As String,
                                         objParametri_Server As AgronicaCoreParametri,
                                         Optional Causale_Cod As Integer = 0,
                                         Optional UtilizzaTransazione As Boolean = True,
                                         Optional Mac_Cod As Integer = 0,
                                         Optional Data_Invio As DateTime? = Nothing,
                                         Optional Dettaglio1 As String = "",
                                         Optional Dettaglio2 As String = "",
                                         Optional Dettaglio3 As String = "")

        Dim objLogInvioChiamate As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
        Dim objLogInvioAnagrafe As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_W

        Dim Tipo_Operazione As String = ""
        Select Case TipoOperazione
            Case enum_TipoOperazioneDB.Scrittura
                Tipo_Operazione = "INS"
            Case enum_TipoOperazioneDB.Modifica
                Tipo_Operazione = "UPD"
            Case enum_TipoOperazioneDB.Cancellazione
                Tipo_Operazione = "DEL"
        End Select

        Dim ID_LogInvioChiamata = objLogInvioChiamate.Scrivi(Tipo_Esportazione,
                                                             Dati_Inviati,
                                                             Data_Invio,
                                                             Esito,
                                                             Dati_Ricevuti,
                                                             "0",
                                                             Tipo_Operazione,
                                                             objParametri_Server,
                                                             Dettaglio1,
                                                             Dettaglio2,
                                                             Dettaglio3,
                                                             UtilizzaTransazione:=UtilizzaTransazione)

        objLogInvioAnagrafe.Scrivi(Tipo_Esportazione, ID_LogInvioChiamata, 0,
                                       Tipo,
                                       Chiave, Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod,
                                       Note,
                                       DateTime.Now,
                                       objParametri_Server,
                                       Chiave_Esterna, Campo_Cod, Fabbricato_Cod,
                                       Causale_Cod, Mac_Cod)
    End Sub

    Public Sub Create_Log_Invio_Anagrafe(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                                         Dati_Inviati As String,
                                         Tipo As String,
                                         Chiave As String,
                                         Chiave_Esterna As String,
                                         Piva As String,
                                         Sa_Cod As Integer,
                                         Campo_Cod As Integer,
                                         Appezza As Integer,
                                         Id_Reg As Integer,
                                         Progetto_Cod As Integer,
                                         Fabbricato_Cod As Integer,
                                         Note As String,
                                         TipoOperazione As enum_TipoOperazioneDB,
                                         Esito As String,
                                         Dati_Ricevuti As String,
                                         objParametri_Server As AgronicaCoreParametri,
                                         GiasContext As Gias_DeveloperServer_Entities,
                                         Optional Causale_Cod As Integer = 0,
                                         Optional Mac_Cod As Integer = 0)

        Dim objLogInvioChiamate As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
        Dim objLogInvioAnagrafe As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_W

        Dim Tipo_Operazione As String = ""
        Select Case TipoOperazione
            Case enum_TipoOperazioneDB.Scrittura
                Tipo_Operazione = "INS"
            Case enum_TipoOperazioneDB.Modifica
                Tipo_Operazione = "UPD"
            Case enum_TipoOperazioneDB.Cancellazione
                Tipo_Operazione = "DEL"
        End Select

        Dim ID_Log_Invio = objLogInvioChiamate.Create_Agronica_Log_Invio_Chiamate(
                                                                Tipo_Esportazione,
                                                                Dati_Inviati,
                                                                Date.Now(), Esito,
                                                                Dati_Ricevuti,
                                                                0, Tipo_Operazione,
                                                                objParametri_Server,
                                                                GiasContext)

        objLogInvioAnagrafe.Create_Agronica_Log_Invio_Anagrafe(Tipo_Esportazione, ID_Log_Invio.ID,
                                                               0, Tipo,
                                                               Chiave, Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod,
                                                               Note,
                                                               objParametri_Server, GiasContext,
                                                               Chiave_Esterna, Campo_Cod, Fabbricato_Cod,
                                                               Causale_Cod, Mac_Cod)
    End Sub

    Public Sub Scrivi_Log_Invio_Ricette(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                                            Dati_Inviati As String,
                                            lstRicette As List(Of Tuple(Of Integer, Integer)),
                                            Chiave_Esterna As String,
                                            TipoOperazione As enum_TipoOperazioneDB,
                                            Esito As String,
                                            Dati_Ricevuti As String,
                                            objParametri_Server As AgronicaCoreParametri,
                                            giasContext As Gias_DeveloperServer_Entities,
                                            Optional UtilizzaTransazione As Boolean = True,
                                            Optional Data_Invio As DateTime? = Nothing,
                                            Optional dettaglio1 As String = "",
                                            Optional dettaglio2 As String = "",
                                            Optional dettaglio3 As String = "")

        Dim objLogInvioChiamate As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
        Dim objLogInvioRicette As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Ricette_W

        Dim Tipo_Operazione As String = ""
        Select Case TipoOperazione
            Case enum_TipoOperazioneDB.Scrittura
                Tipo_Operazione = "INS"
            Case enum_TipoOperazioneDB.Modifica
                Tipo_Operazione = "UPD"
            Case enum_TipoOperazioneDB.Cancellazione
                Tipo_Operazione = "DEL"
        End Select

        Dim ID_LogInvioChiamata = objLogInvioChiamate.Scrivi(Tipo_Esportazione,
                                                             Dati_Inviati, Data_Invio,
                                                             Esito, Dati_Ricevuti,
                                                             "0", Tipo_Operazione,
                                                             objParametri_Server,
                                                             UtilizzaTransazione:=UtilizzaTransazione,
                                                             Dettaglio1:=dettaglio1,
                                                             Dettaglio2:=dettaglio2,
                                                             Dettaglio3:=dettaglio3)

        For Each item In lstRicette
            objLogInvioRicette.Create_Agronica_Log_Invio_Ricette(Tipo_Esportazione, ID_Ricetta:=item.Item1, ID_Ricetta_Operazione:=item.Item2, Chiave_Esterna, ID_LogInvioChiamata, objParametri_Server, giasContext)
        Next

    End Sub

    Public Sub Scrivi_Log_Invio_Agenda(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                                       Dati_Inviati As String,
                                       lstAgende As List(Of Tuple(Of String, Integer, String)),
                                       Id_Operazione_Esterna As Integer,
                                       TipoOperazione As enum_TipoOperazioneDB,
                                       Esito As String,
                                       Dati_Ricevuti As String,
                                       objParametri_Server As AgronicaCoreParametri,
                                       giasContext As Gias_DeveloperServer_Entities,
                                       Optional UtilizzaTransazione As Boolean = True,
                                       Optional Data_Invio As DateTime? = Nothing,
                                       Optional dettaglio1 As String = "",
                                       Optional dettaglio2 As String = "",
                                       Optional dettaglio3 As String = "")

        Dim objLogInvioChiamate As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
        Dim objLogInvioAgenda As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Agenda_W

        Dim Tipo_Operazione As String = ""
        Select Case TipoOperazione
            Case enum_TipoOperazioneDB.Scrittura
                Tipo_Operazione = "INS"
            Case enum_TipoOperazioneDB.Modifica
                Tipo_Operazione = "UPD"
            Case enum_TipoOperazioneDB.Cancellazione
                Tipo_Operazione = "DEL"
        End Select

        Dim ID_LogInvioChiamata = objLogInvioChiamate.Scrivi(Tipo_Esportazione,
                                                             Dati_Inviati, Data_Invio,
                                                             Esito, Dati_Ricevuti,
                                                             "0", Tipo_Operazione,
                                                             objParametri_Server,
                                                             UtilizzaTransazione:=UtilizzaTransazione,
                                                             Dettaglio1:=dettaglio1,
                                                             Dettaglio2:=dettaglio2,
                                                             Dettaglio3:=dettaglio3)

        For Each tuplaAgenda In lstAgende
            objLogInvioAgenda.Create_Agronica_Log_Invio_Agenda(Tipo_Esportazione,
                                                               tuplaAgenda.Item2,
                                                               Id_Operazione_Esterna,
                                                               ID_LogInvioChiamata,
                                                               objParametri_Server,
                                                               giasContext,
                                                               Piva:=tuplaAgenda.Item1,
                                                               Chiave_Esterna:=tuplaAgenda.Item3,
                                                               Chiave_Gias:=tuplaAgenda.Item1 & "_" & tuplaAgenda.Item2)
        Next

    End Sub

    Public Sub Scrivi_Log_Invio_Contatti(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                                        Dati_Inviati As String,
                                        Piva As String,
                                        Cod_Contatto As String,
                                        Chiave_Esterna As String,
                                        TipoOperazione As enum_TipoOperazioneDB,
                                        Esito As String,
                                        Dati_Ricevuti As String,
                                        objParametri_Server As AgronicaCoreParametri,
                                            Optional UtilizzaTransazione As Boolean = True,
                                            Optional Data_Invio As DateTime? = Nothing)

        Dim objLogInvioChiamate As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
        Dim objLogInvioContatti As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Contatti_W

        Dim Tipo_Operazione As String = ""
        Select Case TipoOperazione
            Case enum_TipoOperazioneDB.Scrittura
                Tipo_Operazione = "INS"
            Case enum_TipoOperazioneDB.Modifica
                Tipo_Operazione = "UPD"
            Case enum_TipoOperazioneDB.Cancellazione
                Tipo_Operazione = "DEL"
        End Select

        Dim ID_LogInvioChiamata = objLogInvioChiamate.Scrivi(Tipo_Esportazione,
                                                             Dati_Inviati, Data_Invio,
                                                             Esito, Dati_Ricevuti,
                                                             "0", Tipo_Operazione,
                                                             objParametri_Server,
                                                             UtilizzaTransazione:=UtilizzaTransazione)

        Dim chiave As String = String.Format("{0}_{1}", Piva, Cod_Contatto)
        Dim note As String = ""
        objLogInvioContatti.Scrivi(Tipo_Esportazione,
                                   ID_LogInvioChiamata, 0,
                                   chiave, Chiave_Esterna,
                                   Piva, 0, Cod_Contatto,
                                   note, objParametri_Server)

    End Sub

    Public Sub Update_Log_Invio_Chiamate(ID As Integer,
                                         Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                                         Dati_Inviati As String,
                                         TipoOperazione As enum_TipoOperazioneDB,
                                         Esito As String,
                                         Dati_Ricevuti As String,
                                         objParametri_Server As AgronicaCoreParametri,
                                            Optional Data_Invio As DateTime? = Nothing)

        Dim objLogInvioChiamate As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W

        Dim Tipo_Operazione As String = ""
        Select Case TipoOperazione
            Case enum_TipoOperazioneDB.Scrittura
                Tipo_Operazione = "INS"
            Case enum_TipoOperazioneDB.Modifica
                Tipo_Operazione = "UPD"
            Case enum_TipoOperazioneDB.Cancellazione
                Tipo_Operazione = "DEL"
        End Select

        objLogInvioChiamate.Modifica(ID, Tipo_Esportazione, Dati_Inviati, Esito, Dati_Ricevuti, "0", Tipo_Operazione, objParametri_Server, Data_Invio:=Data_Invio)

    End Sub

    Public Function Scrivi_Log_Invio_Chiamate(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                                             Dati_Inviati As String,
                                             Data_Invio As DateTime,
                                             Controllata As Integer,
                                             TipoOperazione As enum_TipoOperazioneDB,
                                             Esito As String,
                                             Dati_Ricevuti As String,
                                             objParametri_Server As AgronicaCoreParametri,
                                                Optional Dettaglio1 As String = "",
                                                Optional Dettaglio2 As String = "",
                                                Optional Dettaglio3 As String = "",
                                                Optional UtilizzaTransazione As Boolean = True) As Integer

        Dim objLogInvioChiamate As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W

        Dim Tipo_Operazione As String = ""
        Select Case TipoOperazione
            Case enum_TipoOperazioneDB.Scrittura
                Tipo_Operazione = "INS"
            Case enum_TipoOperazioneDB.Modifica
                Tipo_Operazione = "UPD"
            Case enum_TipoOperazioneDB.Cancellazione
                Tipo_Operazione = "DEL"
        End Select

        Return objLogInvioChiamate.Scrivi(Tipo_Esportazione,
                                          Dati_Inviati,
                                          Data_Invio,
                                          Esito,
                                          Dati_Ricevuti,
                                          Controllata,
                                          Tipo_Operazione,
                                          objParametri_Server,
                                          Dettaglio1:=Dettaglio1,
                                          Dettaglio2:=Dettaglio2,
                                          Dettaglio3:=Dettaglio3,
                                          UtilizzaTransazione:=UtilizzaTransazione)

    End Function
End Class
