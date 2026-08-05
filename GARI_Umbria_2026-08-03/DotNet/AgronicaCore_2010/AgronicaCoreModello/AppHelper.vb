Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports System.Globalization
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreModelsSTD.GiasAPP
Imports AgronicaCoreModelsSTD.anagrafiche

Public Class AppHelper


    ''' <summary>
    ''' Dizionario per mappare gli import che utilizzano come tracciato AgronicaCoreStd\AgronicaCoreDTOStd\InData\Demetra\Attivita.cs
    ''' </summary>
    Public Shared DictOrigineAttivitaToImport As New Dictionary(Of enum_Dati_App, String) From {
        {enum_Dati_App.AttivitaDemetra, enum_OrigineApp.Demetra},
        {enum_Dati_App.AttivitaNewAgri, enum_OrigineApp.PUA},
        {enum_Dati_App.RicetteDemetra, enum_OrigineApp.Demetra}
    }

    Public Shared Function FindValueInDictOrigineAttivitaToImport(ByVal val As String) As Boolean
        Dim find As Boolean = False

        If Not String.IsNullOrEmpty(val) Then
            Dim index = DictOrigineAttivitaToImport.Values.ToList().FindIndex(Function(x) x.ToUpper() = val.ToUpper())

            If index > -1 Then
                find = True
            End If
        End If

        Return find
    End Function

    ' impostazioni default per importazioni in base al tipo dati app
    Public IMPORTAZIONI_DEFAULT = New Dictionary(Of enum_Dati_App, enum_Import_App) From {
        {enum_Dati_App.Attivita, enum_Import_App.Completo},
        {enum_Dati_App.AttivitaCdG, enum_Import_App.Nessuno},
        {enum_Dati_App.Ricette, enum_Import_App.Completo},
        {enum_Dati_App.Rilievi, enum_Import_App.Completo},
        {enum_Dati_App.Visite, enum_Import_App.Completo},
        {enum_Dati_App.Documenti, enum_Import_App.Completo},
        {enum_Dati_App.Movimenti, enum_Import_App.Completo},
        {enum_Dati_App.Acquisti, enum_Import_App.Completo},
        {enum_Dati_App.Manutenzioni, enum_Import_App.Completo},
        {enum_Dati_App.MovimentiGruppo, enum_Import_App.Completo},
        {enum_Dati_App.AttivitaDemetra, enum_Import_App.Completo},
        {enum_Dati_App.AttivitaNewAgri, enum_Import_App.Completo},
        {enum_Dati_App.RicetteDemetra, enum_Import_App.Completo}
    }

    'Public Const IMPORTAZIONI_DEFAULT As String = "10_0|11_0|12_0|20_0|30_0|40_0|50_2|51_2|60_2"
    'Public Const IMPORTAZIONI_MASTER As String = "10_2|11_2|12_2|20_2|30_2|40_2|50_2|51_2|60_2"

    Public Enum enum_Dati_App
        Imprese = 1
        Appezzamenti = 4
        Attivita = 10
        AttivitaCdG = 11
        Ricette = 12
        Rilievi = 20
        Visite = 30
        Documenti = 40
        Movimenti = 50
        Acquisti = 51
        MovimentiGruppo = 52
        Manutenzioni = 60
        AttivitaZoo = 70
        CapoAnimale = 71
        PianoCampionamento = 80
        PianoCampionamentoConSpostamento = 81
        AttivitaDemetra = 90
        AttivitaNewAgri = 100
        MovimentiDemetra = 110
        AcquistiDemetra = 111
        RicetteDemetra = 112
    End Enum

    Public Enum enum_Import_App
        Nessuno = 0 ' Solo frontiera
        Parziale = 1 ' x AgroGSB
        Completo = 2 ' Master APP
    End Enum

    Public Enum enum_Stato_Applicazione
        NonImportato = 0
        Importato = 1
        ImportatoBrogliaccio = 2
        ImportatoQuaderno = 3
    End Enum

    Public listaOperazioniRilievi As New List(Of Integer) From {
        LAVCOD_RILIEVO_AVVERSITA_CAMPO,
        LAVCOD_RILIEVO_INDICI_MATURITA,
        LAVCOD_FASI_FENOLOGICHE
    }

    Public Function Leggi_TipoDati_APP(ByVal Lav_Cod As Integer) As enum_Dati_App
        Dim tipoDatiApp = enum_Dati_App.Attivita
        If listaOperazioniRilievi.Contains(Lav_Cod) Then
            tipoDatiApp = enum_Dati_App.Rilievi
        End If
        Return tipoDatiApp
    End Function

    Public Function Leggi_ImportDati_APP(ByVal tipo As enum_Dati_App, ByRef objParametri_Utenti As AgronicaCoreParametri, Optional ByRef importAgenda As String = "") As enum_Import_App

        Dim importazioni = IMPORTAZIONI_DEFAULT
        Dim impostazioni = Leggi_Impostazioni_APP(objParametri_Utenti)

        If impostazioni IsNot Nothing Then
            If Not String.IsNullOrEmpty(impostazioni.ImportAgenda) Then
                importAgenda = impostazioni.ImportAgenda
            End If
            If Not String.IsNullOrEmpty(impostazioni.Importazioni) Then
                If impostazioni.Importazioni = "2" Then
                    Return enum_Import_App.Completo
                ElseIf impostazioni.Importazioni = "1" Then
                    Return enum_Import_App.Parziale
                ElseIf impostazioni.Importazioni = "0" Then
                    Return enum_Import_App.Nessuno
                Else
                    'importazioni = impostazioni.Importazioni
                    For Each importazione In impostazioni.Importazioni.Split("|")
                        Dim import = importazione.Split("_")
                        importazioni(import(0)) = import(1)
                    Next
                End If
            ElseIf impostazioni.Master Then
                'importazioni = IMPORTAZIONI_MASTER
                Return enum_Import_App.Completo
            End If
        End If


        'For Each importazione In importazioni.Split("|")
        '    Dim import = importazione.Split("_")
        '    If CStr(tipo) = import(0) Then
        '        If import(1) = "2" Then
        '            Return enum_Import_App.Completo
        '        ElseIf import(1) = "1" Then
        '            Return enum_Import_App.Parziale
        '        Else
        '            Return enum_Import_App.Nessuno
        '        End If
        '    End If
        'Next

        If importazioni.ContainsKey(tipo) Then
            Return importazioni(tipo)
        End If

        Return enum_Import_App.Nessuno

    End Function

    Public Function Leggi_CaricaDati_APP(ByRef SincroDatiApp As Boolean, ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim configurazione = objConfigSiti.Leggi_Valore(0, "CaricaDatiApp", "", "", objParametri_Server)

        ' impostare la chiave a "2" per compatibilità con app Xamarin
        SincroDatiApp = configurazione <> "2"
        Return configurazione <> "0"

    End Function

    Public Function Leggi_RestoreDati_APP(ByRef objParametri_Utenti As AgronicaCoreParametri, Optional ByVal tipo As enum_Dati_App = 0) As Boolean

        Dim restore As Boolean = False
        Dim impostazioni = Leggi_Impostazioni_APP(objParametri_Utenti)
        If impostazioni IsNot Nothing AndAlso Not String.IsNullOrEmpty(impostazioni.RestoreDati) Then
            If impostazioni.RestoreDati = "0" Then
                Return False
            ElseIf impostazioni.RestoreDati = "1" OrElse tipo = 0 Then
                Return True
            Else
                Return impostazioni.RestoreDati.Split(",").Contains(tipo)
            End If
        End If

        Return restore

    End Function

    Public Function Leggi_Impostazioni_APP(ByRef objParametri_Utenti As AgronicaCoreParametri) As ImpostazioniAPP

        Dim impostazioniAPP As ImpostazioniAPP = Nothing
        Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim impostazioni = objImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI, objParametri_Utenti, 2)

        If Not String.IsNullOrEmpty(impostazioni) Then
            impostazioniAPP = JsonConvert.DeserializeObject(Of ImpostazioniAPP)(impostazioni)
        End If

        Return impostazioniAPP

    End Function

    Public Function Leggi_Dati_Import(ByVal Tipo As String, objParametri_Server As AgronicaCoreParametri, Optional ByVal Piva As String = "") As DataTable

        Dim objappDati As New AgronicaCoreContabDAL.APP_Dati_R
        Return objappDati.Leggi_DatiAPP("", Tipo, True, "", "", objParametri_Server, Piva)

    End Function

    Public Function Check_Dati_APP(ByVal ID As String, ByVal Tipo As String, objParametri_Server As AgronicaCoreParametri) As String

        Dim riferimento As String = ""
        Dim objappDati As New AgronicaCoreContabDAL.APP_Dati_R
        Dim dati = objappDati.Leggi_DatiAPP(ID, Tipo, False, "", "", objParametri_Server)

        If dati.Rows.Count > 0 Then
            riferimento = dati.Rows(0).Item("Riferimento")
            If String.IsNullOrEmpty(riferimento) Then
                If IsDBNull(dati.Rows(0).Item("Importato_Data")) Then
                    Return "ERRORE"
                End If
            End If
        End If

        Return riferimento

    End Function

    Public Function Leggi_Dati_APP(ByVal ID As String, ByVal Tipo As String, objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim objappDati As New AgronicaCoreContabDAL.APP_Dati_R
        Return objappDati.Leggi_DatiAPP(ID, Tipo, False, "", "", objParametri_Server)

    End Function

    Public Function Leggi_Dati_APP(ByVal Tipo As String, ByVal Piva As String, ByVal Data As String, objParametri_Server As AgronicaCoreParametri) As DataTable


        Dim objappDati As New AgronicaCoreContabDAL.APP_Dati_R
        Dim Username As String = objParametri_Server.UsernameOperazione

        If Date.TryParseExact(Data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, Nothing) Then
            Data = Date.ParseExact(Data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
        Else
            Data = AGRODATAINIZIO
        End If

        Return objappDati.Leggi_DatiAPP("", Tipo, False, "cancellato=0", "datainvio", objParametri_Server, Piva, Data, Username)

    End Function

    Public Function Leggi_Dati_APP_Storico(ByVal Tipo As String, ByVal Piva As String, ByVal Data As String, objParametri_Server As AgronicaCoreParametri) As DataTable


        Dim objappDati As New AgronicaCoreContabDAL.APP_Dati_R

        If Date.TryParseExact(Data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, Nothing) Then
            Data = Date.ParseExact(Data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
        Else
            Data = AGRODATAINIZIO
        End If

        Return objappDati.Leggi_DatiAPP("", Tipo, False, "cancellato=0", "datainvio", objParametri_Server, Piva, Data, "")

    End Function

    Public Function Consulta_Sincro_Dati_App(ByVal Tipi As List(Of String),
                                             ByVal Data_Inizio As Date,
                                             ByVal Data_Fine As Date,
                                             ByVal FiltroImportati As Integer,
                                             objParametri_Server As AgronicaCoreParametri,
                                             objParametri_Utenti As AgronicaCoreParametri,
                                             Optional ByVal DatiAggiuntivi As Boolean = False
                                             ) As List(Of ConsultaSincroLog)

        Dim objappDati As New AgronicaCoreContabDAL.APP_Dati_R
        Dim result As New List(Of ConsultaSincroLog)

        If Data_Inizio = #1/1/0001 12:00:00 AM# Then
            Data_Inizio = Date.Now()
            Data_Fine = AGRODATAFINE
        End If

        Dim dt = objappDati.Consulta_Sincro_Dati_App(Tipi,
                                                     FiltroImportati,
                                                     "",
                                                     "",
                                                     objParametri_Server,
                                                     objParametri_Utenti,
                                                     Data_Inizio,
                                                     Data_Fine,
                                                     DatiAggiuntivi)

        Dim righe = dt.ToExpandoObject()

        If righe.Count > 0 Then

            For Each row In righe

                Dim riga As New ConsultaSincroLog
                riga.utente = If(IsDBNull(row("utente")), "", row("utente"))
                riga.azienda_cod = If(IsDBNull(row("azienda_cod")), "", row("azienda_cod"))
                riga.azienda_des = If(IsDBNull(row("azienda_des")), "", row("azienda_des"))
                riga.data_sincro = If(IsDBNull(row("data_sincro")), AGRODATAINIZIO, row("data_sincro"))
                riga.Dati = If(DatiAggiuntivi = False OrElse IsDBNull(row("Dati")), "", row("Dati"))
                riga.tipo_dato = If(IsDBNull(row("Tipo")), 0, row("Tipo"))
                riga.Riferimento = If(IsDBNull(row("Riferimento")), "", row("Riferimento"))
                riga.descrizione = ""
                riga.tipo_aggiornamento = If(IsDBNull(row("cancellato")), "0", row("cancellato"))
                riga.stato_applicazione = 0
                riga.errori = If(IsDBNull(row("importato_errore")), "", row("importato_errore"))

                If IsNothing(riga.data_sincro) OrElse riga.data_sincro = CostantiPersonalizzate.AGRODATAINIZIO Then
                    riga.stato_applicazione = enum_Stato_Applicazione.NonImportato
                Else
                    If riga.errori = "" Then
                        If riga.tipo_dato <> enum_Dati_App.Attivita AndAlso Not DictOrigineAttivitaToImport.ContainsKey(riga.tipo_dato) Then
                            riga.stato_applicazione = enum_Stato_Applicazione.Importato
                        Else
                            If Split(riga.Riferimento, "|")(0).Equals("0") Then
                                riga.stato_applicazione = enum_Stato_Applicazione.ImportatoBrogliaccio
                            Else
                                riga.stato_applicazione = enum_Stato_Applicazione.ImportatoQuaderno
                            End If
                        End If
                    Else
                        riga.stato_applicazione = enum_Stato_Applicazione.NonImportato
                    End If

                End If

                If Not IsNothing(riga.Dati) AndAlso Not riga.Dati.Equals("") Then

                    Try

                        Select Case riga.tipo_dato

                            Case enum_Dati_App.Attivita,
                                enum_Dati_App.Ricette,
                                 enum_Dati_App.Rilievi
                                Dim ricette As RicettePerScarico = JsonConvert.DeserializeObject(Of RicettePerScarico)(riga.Dati)
                                riga.descrizione = ricette.Ricette(0).Ricetta_Des + ",<br>" + Gias.Operazione + ": " + ricette.RicetteOperazioni(0).Ricetta_Operazione_Des

                            Case enum_Dati_App.AttivitaCdG
                                Dim ricette As RicettePerScarico = JsonConvert.DeserializeObject(Of RicettePerScarico)(riga.Dati)
                                riga.descrizione = ricette.Attivita(0).Note

                            Case enum_Dati_App.Visite
                                Dim visite = JsonConvert.DeserializeObject(Of VisitePerScarico)(riga.Dati)
                                riga.descrizione = visite.VisiteDettagli(0).Descrizione

                            Case enum_Dati_App.Documenti
                                Dim documento = JsonConvert.DeserializeObject(Of DocumentoPerScarico)(riga.Dati)
                                riga.descrizione = documento.Descrizione

                            Case enum_Dati_App.Movimenti
                                Dim listaMovimenti = JsonConvert.DeserializeObject(Of MovimentoDiMagazzino)(riga.Dati)
                                riga.descrizione = Gias.Magazzino + ": " + listaMovimenti.Magazzino.descrizione + ",<br>" + Gias.Prodotto + ": " + listaMovimenti.Prodotto.descrizione

                            Case enum_Dati_App.Acquisti
                                Dim acquisto = JsonConvert.DeserializeObject(Of Acquisto)(riga.Dati)
                                riga.descrizione = Gias.nrDocumento + ": " + acquisto.numDoc + ",<br>" + Gias.Data + ": " + acquisto.dataDoc.ToString("d")

                            Case enum_Dati_App.Manutenzioni
                                Dim listaManutenzioni = JsonConvert.DeserializeObject(Of Manutenzione)(riga.Dati)
                                riga.descrizione = listaManutenzioni.descrizione + ",<br>" + Gias.Macchina + ": " + listaManutenzioni.macchina.descrizione

                            Case Else
                                If DictOrigineAttivitaToImport.ContainsKey(riga.tipo_dato) Then
                                    Dim ricette As RicettePerScarico = JsonConvert.DeserializeObject(Of RicettePerScarico)(riga.Dati)
                                    riga.descrizione = ricette.Ricette(0).Ricetta_Des + ",<br>" + Gias.Operazione + ": " + ricette.RicetteOperazioni(0).Ricetta_Operazione_Des
                                End If
                        End Select

                    Catch ex As Exception

                        riga.descrizione = ""

                    End Try

                End If

                result.Add(riga)

            Next

        End If

        Return result

    End Function

    Public Function Leggi_Riferimento_APP(ByVal ID As String, ByRef Tipo As String, objParametri_Server As AgronicaCoreParametri, ByRef cancellato As Boolean) As String

        Dim riferimento As String = ""
        Dim objappDati As New AgronicaCoreContabDAL.APP_Dati_R
        Dim dt = objappDati.Leggi_DatiAPP(ID, "", False, "", "", objParametri_Server)

        If dt.Rows.Count > 0 Then
            Dim dr = dt.Rows(0)
            Tipo = dr.Item("Tipo")
            riferimento = dr.Item("Riferimento")
            If IsNothing(dr.Item("Cancellato")) Then
                cancellato = False
            Else
                cancellato = CShort(dr.Item("Cancellato")) = 1
            End If
        End If

        Return riferimento

    End Function

    Public Function LeggiGuidEVersioneAppDati(guid As String, objParameteriServer As AgronicaCoreParametri) As (id As String, versione As String)

        Dim id As String = ""
        Dim versione As String = ""

        Dim objappDati As New AgronicaCoreContabDAL.APP_Dati_R
        Dim dt = objappDati.LeggiIdEVersione(guid, objParameteriServer)

        If dt.Rows.Count > 0 Then
            id = dt.Rows(0).Item("ID")
            versione = dt.Rows(0).Item("Versione")
        End If

        Return (id, versione)
    End Function

    Public Function EsisteRicettaConGuid(guid As String, objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim ricette_Operazioni_R As New AgronicaCoreContabDAL.Ricette_Operazioni_R
        Dim dt = ricette_Operazioni_R.Leggi1SeRicettaConGuidEsiste(guid, objParametri_Server)

        Return dt IsNot Nothing AndAlso dt.Rows.Count > 0

    End Function

    Public Function Leggi_GUID_VERSIONE_APP(ByVal Codice As String, ByVal Tipo As String, objParametri_Server As AgronicaCoreParametri) As (id As String, versione As String)

        Dim guid As String = ""
        Dim versione As String = ""

        Dim objappDati As New AgronicaCoreContabDAL.APP_Dati_R
        Dim dt = objappDati.Leggi_DatiAPP("", Tipo, False, " codice = '" & Codice & "'", "", objParametri_Server)

        If dt.Rows.Count > 0 Then
            guid = dt.Rows(0).Item("ID")
            versione = dt.Rows(0).Item("Versione")
        End If

        Return (guid, versione)

    End Function

    Public Function Aggiorna_Dati_APP(ByVal ID As String, ByVal Tipo As String,
                                      ByVal Errori As String, ByVal Riferimento As String,
                                      ByVal Cancellato As Boolean, ByVal Importazione As Boolean,
                                      ByRef objParametri_server As AgronicaCoreParametri,
                                      Optional ByVal User_Agent As String = "", Optional ByVal versione As String = "",
                                      Optional ByVal riferimentoPianificata As String = Nothing) As Boolean

        Dim appDatiScrivi As New AgronicaCoreContabDAL.APP_Dati_W
        If Tipo = enum_Dati_App.Attivita OrElse Tipo = enum_Dati_App.AttivitaDemetra OrElse Tipo = enum_Dati_App.Movimenti OrElse Tipo = enum_Dati_App.MovimentiDemetra OrElse Tipo = enum_Dati_App.Acquisti OrElse Tipo = enum_Dati_App.AcquistiDemetra OrElse Tipo = enum_Dati_App.Ricette OrElse Tipo = enum_Dati_App.RicetteDemetra Then
            Return appDatiScrivi.Aggiorna_DatiAPP_v2(ID, Tipo, Errori, Riferimento, Cancellato, Importazione, objParametri_server, User_Agent, versione, riferimentoPianificata)
        Else
            Return appDatiScrivi.Aggiorna_DatiAPP(ID, Tipo, Errori, Riferimento, Cancellato, Importazione, objParametri_server, User_Agent, versione, riferimentoPianificata)
        End If

    End Function

    Public Function Cancella_Dati_APP(ByVal ID As String, ByVal Tipo As String, ByRef objParametri_server As AgronicaCoreParametri) As Boolean

        Dim appDatiScrivi As New AgronicaCoreContabDAL.APP_Dati_W
        Dim result As Boolean = True

        If Not String.IsNullOrEmpty(ID) Then
            If Tipo = enum_Dati_App.Attivita OrElse Tipo = enum_Dati_App.AttivitaDemetra OrElse Tipo = enum_Dati_App.Movimenti OrElse Tipo = enum_Dati_App.MovimentiDemetra OrElse Tipo = enum_Dati_App.Acquisti OrElse Tipo = enum_Dati_App.AcquistiDemetra OrElse Tipo = enum_Dati_App.Ricette OrElse Tipo = enum_Dati_App.RicetteDemetra Then
                result = appDatiScrivi.Cancella_DatiAPP_v2(ID, objParametri_server)
            Else
                result = appDatiScrivi.Cancella_DatiAPP(ID, Tipo, objParametri_server)
            End If
        End If

        Return result

    End Function

    Public Function Scrivi_Dati_APP(ByRef ID As String, ByVal Tipo As String, ByVal Dati As String,
                                    ByVal Riferimento As String, ByVal Cancellato As Boolean,
                                    ByRef objParametri_server As AgronicaCoreParametri,
                                    Optional ByVal Piva As String = "",
                                    Optional ByVal Codice As String = "",
                                    Optional ByVal User_Agent As String = "",
                                    Optional ByVal versione As String = "",
                                    Optional ByVal riferimentoPianificata As String = Nothing) As Boolean

        Dim appDatiScrivi As New AgronicaCoreContabDAL.APP_Dati_W
        Dim cancellazione As Boolean = False
        Dim esito As Boolean = True

        If String.IsNullOrEmpty(ID) Then
            ID = Guid.NewGuid().ToString()
        Else
            cancellazione = True
        End If

        ' cancellazione dati app
        If cancellazione Then
            Dim appDatiRead As New AgronicaCoreContabDAL.APP_Dati_R
            Dim dt = appDatiRead.LeggiTipoDaAppDati(ID, objParametri_server)

            If Tipo = enum_Dati_App.Attivita OrElse Tipo = enum_Dati_App.AttivitaDemetra OrElse Tipo = enum_Dati_App.Movimenti OrElse Tipo = enum_Dati_App.MovimentiDemetra OrElse Tipo = enum_Dati_App.Acquisti OrElse Tipo = enum_Dati_App.AcquistiDemetra OrElse Tipo = enum_Dati_App.Ricette OrElse Tipo = enum_Dati_App.RicetteDemetra Then
                esito = appDatiScrivi.Cancella_DatiAPP_v2(ID, objParametri_server)
            Else
                esito = appDatiScrivi.Cancella_DatiAPP(ID, Tipo, objParametri_server)
            End If

            If (dt IsNot Nothing AndAlso dt.Rows.Count > 0) Then
                Dim tipoEsistente = dt.Rows(0)("Tipo")
                If (tipoEsistente = enum_Dati_App.Attivita AndAlso Tipo = enum_Dati_App.AttivitaDemetra) Then 'aggiornamento da parte di demetra di un'attività inserita/modificata da app
                    Dim ricetteWrite As New AgronicaCoreContabDAL.Ricette_W
                    ricetteWrite.RimuoviDatiDaTabelleApp(ID, objParametri_server)
                End If
            End If
        End If

        ' inserimento dati app
        If esito Then
            esito = appDatiScrivi.Scrivi_DatiAPP(ID, Tipo, Dati, Riferimento, Cancellato, objParametri_server, Piva, Codice, User_Agent,
                                                 versione:=versione, riferimentoPianificata:=riferimentoPianificata)
        End If

        Return esito

    End Function

    Public Function Aggiorna_Agenda_Dati_APP(ByVal IdAgenda As Integer,
                                                ByVal Ricetta_Cod As Integer,
                                                ByVal Ricetta_Operazione_Cod As Integer,
                                                ByVal objParametri_server As AgronicaCoreParametri) As Boolean
        Dim esito As Boolean = True

        Dim objRicette_Operazioni_R As New AgronicaCoreContabDAL.Ricette_Operazioni_R
        Dim dt = objRicette_Operazioni_R.Leggi(Ricetta_Cod, Ricetta_Operazione_Cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)

        If dt.Rows.Count > 0 Then
            Dim APP_Ricetta_Operazione_ID As String = dt.Rows(0).Item("APP_Ricetta_Operazione_ID")

            If Not String.IsNullOrEmpty(APP_Ricetta_Operazione_ID) Then
                Dim id As String = APP_Ricetta_Operazione_ID.Split("|"c)(0)

                Dim appDatiRead As New AgronicaCoreContabDAL.APP_Dati_R
                Dim dtTipo = appDatiRead.LeggiTipoDaAppDati(id, objParametri_server)
                If dtTipo IsNot Nothing AndAlso dtTipo.Rows.Count > 0 Then

                    Dim tipo As String = dtTipo.Rows(0)("Tipo")
                    Dim appDatiScrivi As New AgronicaCoreContabDAL.APP_Dati_W
                    If (tipo = "10") Then
                        esito = appDatiScrivi.Aggiorna_Riferimento_DatiAPP(id, IdAgenda & "|" & Ricetta_Operazione_Cod, objParametri_server)
                    Else
                        esito = appDatiScrivi.Aggiorna_Riferimento_DatiAPP(id, IdAgenda & "|" & Ricetta_Cod, objParametri_server)
                    End If

                End If

            End If

        End If

        Return esito

    End Function

    Private Function GeneraNumDoc(ByVal Num_Doc As String, ByRef Num_Doc_Sin As String, ByRef Num_Doc_Des As String) As Integer

        Dim Num_Doc_Tmp As String = ""

        For Each c As Char In Num_Doc
            If IsNumeric(c) Then
                If Num_Doc_Des = "" Then
                    Num_Doc_Tmp &= c
                Else
                    Num_Doc_Des &= c
                End If
            Else
                If Num_Doc_Tmp = "" Then
                    Num_Doc_Sin &= c
                Else
                    Num_Doc_Des &= c
                End If
            End If
        Next

        If IsNumeric(Num_Doc_Tmp) Then
            Return Num_Doc_Tmp
        End If

        Return 0

    End Function

    Public Function GetProgrRegistrazione(ByVal piva As String, ByVal dataDoc As Date, ByRef objParametri_Server As AgronicaCoreParametri) As Integer

        Dim objSeqProgr As New Sequenza_Progressivi_R
        Dim progrRegistrazione As Integer = 0

        Try
            progrRegistrazione = objSeqProgr.Nuovo_Progressivo_UpdateImmediato(
                piva, Year(dataDoc), enum_SequenzaProgressiviTipi.NumeroRegistrazioneOperazioniContabili, "", "", 0, objParametri_Server)
        Catch ex As Exception
            Throw New Exception("[GetProgrRegistrazione] : " & ex.Message)
        End Try

        Return progrRegistrazione

    End Function

    Public Function GetIndirizzoFornitore(ByVal piva As String, ByVal codContatto As String, ByRef objParametri_Server As AgronicaCoreParametri) As Integer

        Dim objContattiIndirizzi As New ContattiXIndirizzi_R
        Dim codIndirizzo As Integer = objContattiIndirizzi.CodIndirizzo_from_CodContatto(piva, codContatto, 0, objParametri_Server)

        If codIndirizzo = 0 Then
            Dim objImpreseIndirizzi As New ImpresexIndirizzi_R
            codIndirizzo = objImpreseIndirizzi.CodIndirizzo_from_Piva(codContatto, objParametri_Server)
        End If

        Return codIndirizzo

    End Function

    Public Function Genera_Agenda_Da_AcquistoAPP(ByRef acquistoAPP As Acquisto, ByVal Id_Agenda As Integer, ByRef objParametri_Server As AgronicaCoreParametri) As Operazione_Agenda

        Dim Piva = acquistoAPP.centroAziendale.primaryKey.partitaIva
        Dim Sa_Cod = acquistoAPP.centroAziendale.primaryKey.codice
        Dim Lav_Cod = LAVCOD_BOLLA_RICEVUTA
        Dim Cau_Mov = CAU_CARICO
        Dim Num_Doc = acquistoAPP.numDoc
        Dim Data_Doc = acquistoAPP.dataDoc
        Dim Data_Spedizione = acquistoAPP.data
        Dim Cod_RisUm = acquistoAPP.fornitore.codice
        Dim Contatto = acquistoAPP.fornitore.contatto
        Dim Fornitore As String = If(Contatto Is Nothing, Cod_RisUm, Contatto.ragione_Sociale)
        Dim Des_Lib = "DDT Ricevuto (n. " & Num_Doc & " Rif: " & Fornitore & ")"
        Dim Mov_Desc = "Carico di Articoli relativi al D.D.T n. " & Num_Doc & " del " & Data_Doc.Date.ToShortDateString
        Dim Note_Doc = acquistoAPP.note

        ' ricavo numero documento, prefisso e suffisso
        Dim Num_Doc_Sin As String = ""
        Dim Num_Doc_Des As String = ""
        Dim Num_Doc_Int As Integer = GeneraNumDoc(Num_Doc, Num_Doc_Sin, Num_Doc_Des)
        Dim Prog_Reg As Integer = GetProgrRegistrazione(Piva, Data_Doc, objParametri_Server)
        Dim Cod_IndirizzoRisUm As Integer = GetIndirizzoFornitore(Contatto.primaryKey.partitaIva, Contatto.primaryKey.codice, objParametri_Server)

        Dim Tot_Doc As Decimal = 0
        For Each movimentoAPP In acquistoAPP.movimenti
            Tot_Doc += movimentoAPP.Qta * movimentoAPP.Prezzo
        Next

        '#######################################################
        '##################   AGENDA   #########################
        '#######################################################

        Dim Agenda As New Operazione_Agenda With {
                .Tipo_Operazione = enum_TipoOperazioneDB.Scrittura,
                .Id_Agenda = Id_Agenda,
                .Piva = Piva,
                .Sa_Cod = 0,
                .Lav_Cod = Lav_Cod,
                .Des_Lib = Des_Lib,
                .Data = Data_Doc,
                .Modulo = 0,
                .Tipo_Accettazione = 0,
                .Blocco_Flag = 0,
                .Blocco_Data = AGRODATAINIZIO,
                .Blocco_Username = "",
                .Id_Attivita = 0,
                .Audit_Cod = 0
            }

        '-----------------------------------------------------------------------
        'Dim Movimento As Movimento
        'Dim Movimento_Dettaglio As Movimento_Dettaglio
        'Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        'Dim Movimento_Destinazione As Movimento_Destinazione
        'Dim Movimento_Dettaglio_Tecnico_Extra As Movimento_Dettaglio_Tecnico_Extra

        '#######################################################
        '################   MOVIMENTO TESTATA    ###############
        '#######################################################

        Dim Movimento_Testata = New Movimento With {
                .Piva = Piva,
                .Sa_Cod = 0,
                .Id_Agenda = Id_Agenda,
                .Cod_Risum = Cod_RisUm,
                .Cau_Mov = CAU_REGISTRAZIONI,
                .Mov_Desc = "",
                .Data = Data_Doc,
                .Scadenza = AGRODATAFINE,
                .Doc_Numero_Sin = Num_Doc_Sin,
                .Doc_Numero = Num_Doc_Int,
                .Doc_Numero_Des = Num_Doc_Des,
                .Doc_Numero_Visualizzato = Num_Doc,
                .Num_Protocollo_Decimal = Tot_Doc,
                .Cod_IndirizzoRisUm = Cod_IndirizzoRisUm,
                .Cod_Destinazione = 0,
                .Cod_IndirizzoDestinazione = 0,
                .Mezzo = 0,
                .Cod_Vettore = 0,
                .Cod_IndirizzoVettore = 0,
                .Causale_Trasporto = "ACQUISTO", ' ""
                .Causale_Trasporto_Cod = 2, ' 0
                .Aspetto = "VISIBILE", ' ""
                .Peso = 0,
                .Ora = Data_Spedizione,
                .Colli = 0,
                .Extra_Str = Note_Doc,
                .Extra_Int = 0,
                .Extra_Date = AGRODATAINIZIO,
                .Tipo_Sconto = 0,
                .Natura_Beni = "",
                .Tara_Veicolo = 0,
                .Tara_Imballi = 0,
                .Tipo_Peso = enum_TipoPeso.Peso_Lordo,
                .Modalita = 0,
                .Username_Note = "",
                .Progr_Protocollo = 0,
                .Progr_Registrazione = Prog_Reg,
                .Data_Registrazione = Data_Doc,
                .ChkLayOut_Bypass_Fatturato = 0,
                .ChkLayOut_Join_Prodotti = 0,
                .Cod_RisUm_Altro = 0,
                .ChkLayOut_Peso = 0,
                .ChkLayOut_Prezzo = 0,
                .ChkFiltro_Varietale = 0,
                .Disciplinare_PubblicoPrivato = 0,
                .Sezionale_Cod = 0,
                .ChkLayOut_Litri = 0,
                .Cod_RisUm_Aggiuntivo = 0,
                .Cod_Indirizzo_Aggiuntivo = 0,
                .ChkLayOut_Riscontrato = 0
            }

        '#######################################################
        '###############   MOVIMENTO TESTATA EXTRA   ###########
        '#######################################################

        Dim Movimento_Testata_Extra = New Movimento_Dettaglio_Tecnico_Extra With {
                .Piva = Piva,
                .Sa_Cod = 0,
                .Id_Agenda = Id_Agenda,
                .Id_Mov = Movimento_Testata.Id_Mov,
                .Id_Mov_Det = 0,
                .Regione = "",
                .ASL = "",
                .Serie = "",
                .Numero = "",
                .Mac_Cod = 0,
                .Cod_RisUm = 0,
                .Trasportatore = "",
                .Mezzo_Trasporto = "",
                .Targa = "",
                .N_Immatricolazione = "",
                .N_Immatricolazione_Rimorchio = "",
                .N_Autorizzazione_Trasporto = "",
                .Data_Rilascio_Autorizzazione = Data_Doc,
                .Peso = 0,
                .Validita_Inizio = Data_Doc,
                .Validita_Fine = AGRODATAFINE,
                .Codice_Prodotto = 0,
                .Colore = 0,
                .Zona_Viticola = "",
                .Manipolazioni = 0,
                .Precisazioni = "",
                .Annotazioni = "",
                .Num_Contenitori = 0,
                .Marche_Contenitori = "",
                .Des_Contenitori = "",
                .Tipo_Documento = "",
                .Id_Cod_Autorita = 0,
                .Luogo_Partenza = "",
                .Luogo_Consegna = "",
                .Data_Spedizione = AGRODATAINIZIO,
                .Indicazioni_Complementari = "",
                .Titolo_Alcol = 0,
                .Codice_NC = "",
                .Num_Riferimento = "",
                .Data_Dichiarazione = AGRODATAINIZIO,
                .Garanzia = "",
                .Certificati = "",
                .Durata_Viaggio = "",
                .Peso_Lordo = 0,
                .Num_Colli = 0,
                .Contenitore_Cod = 0,
                .Imballaggio_Cod = 0,
                .Agente_Cod = 0,
                .Provvigione = 0,
                .Tipo_Trasporto = 0,
                .Unita_Trasporto = 0,
                .Codice_Alternativo = "",
                .Id_Gestione_Vettore = 0,
                .Ritenuta_Acconto_Cod = 0,
                .Ritenuta_Acconto = 0,
                .Enasarco_Cod = 0,
                .Enasarco = 0,
                .ACCDAA_Cod_Risum_Destinatario = 0,
                .ACCDAA_Cod_Risum_Destinazione = 0,
                .ACCDAA_Cod_IndirizzoRisum_Destinatario = 0,
                .ACCDAA_Cod_IndirizzoRisum_Destinazione = 0
            }

        Movimento_Testata.Movimenti_Dettagli_Tecnici_Extra.Add(Movimento_Testata_Extra)

        '#######################################################
        '################   MOVIMENTO DETTAGLIO    #############
        '#######################################################

        Dim Movimento = New Movimento With {
                .Piva = Piva,
                .Sa_Cod = 0,
                .Id_Agenda = Id_Agenda,
                .Cod_Risum = Cod_RisUm,
                .Cau_Mov = Cau_Mov,
                .Mov_Desc = Mov_Desc,
                .Data = Data_Doc,
                .Scadenza = AGRODATAFINE,
                .Doc_Numero = Num_Doc_Int, '0
                .Num_Protocollo_Decimal = 0,
                .Cod_IndirizzoRisUm = Cod_IndirizzoRisUm,
                .Cod_Destinazione = 0,
                .Cod_IndirizzoDestinazione = 0,
                .Mezzo = 0,
                .Cod_Vettore = 0,
                .Cod_IndirizzoVettore = 0,
                .Causale_Trasporto = "",
                .Aspetto = "",
                .Peso = 0,
                .Ora = Data_Spedizione,
                .Colli = 0,
                .Extra_Str = "",
                .Extra_Int = 0,
                .Extra_Date = #12/30/1899#,
                .Tipo_Sconto = 0,
                .Doc_Numero_Des = "",
                .Natura_Beni = "",
                .Tara_Veicolo = 0,
                .Tara_Imballi = 0,
                .Tipo_Peso = 0,
                .Modalita = 0,
                .Username_Note = "",
                .Doc_Numero_Sin = "",
                .Progr_Protocollo = 0,
                .Progr_Registrazione = 0,
                .Data_Registrazione = AGRODATAINIZIO,
                .ChkLayOut_Bypass_Fatturato = 0,
                .ChkLayOut_Join_Prodotti = 0,
                .Cod_RisUm_Altro = 0,
                .ChkLayOut_Peso = 0,
                .ChkLayOut_Prezzo = 0,
                .ChkFiltro_Varietale = 0,
                .Disciplinare_PubblicoPrivato = 0,
                .Sezionale_Cod = 0,
                .Causale_Trasporto_Cod = 0,
                .ChkLayOut_Litri = 0,
                .Cod_RisUm_Aggiuntivo = 0,
                .Cod_Indirizzo_Aggiuntivo = 0,
                .ChkLayOut_Riscontrato = 0
            }


        'Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        For Each movimentoAPP In acquistoAPP.movimenti

            Dim Piva_Dest = movimentoAPP.Magazzino.primaryKey.centroAziendalePK.partitaIva
            Dim Sa_Cod_Dest = movimentoAPP.Magazzino.primaryKey.centroAziendalePK.codice
            Dim Magazzino_Cod = movimentoAPP.Magazzino.primaryKey.codice
            Dim Elem_Cod = movimentoAPP.Prodotto.elemCod
            Dim prodotto As Integer = movimentoAPP.Prodotto.codice
            If (Elem_Cod = SEMENTI OrElse Elem_Cod = TRASFORMATI_VEGETALI) AndAlso prodotto > 0 Then
                prodotto = -prodotto
            End If
            Dim Pro_Cod = If(prodotto > 0, prodotto, 0)
            Dim Mat_Cod = If(prodotto < 0, -prodotto, 0)
            Dim Mov_Det_Des = movimentoAPP.Prodotto.descrizione
            Dim Lotto = movimentoAPP.Lotto
            Dim Udm_Cod = movimentoAPP.UdM.codice
            Dim Qta = movimentoAPP.Qta
            Dim Prezzo = movimentoAPP.Prezzo
            Dim Note = movimentoAPP.Note
            Dim Causale = enum_Pendenza.DocBolla

            '#######################################################
            '################   MOVIMENTI DETTAGLIO    #############
            '#######################################################

            Dim Movimento_Dettaglio = New Movimento_Dettaglio With {
                .Piva = Piva_Dest,
                .Sa_Cod = Sa_Cod_Dest,
                .Id_Agenda = Id_Agenda,
                .Id_Mov = Movimento.Id_Mov,
                .Elem_Cod = Elem_Cod,
                .Pro_Cod = Pro_Cod,
                .Mat_Cod = Mat_Cod,
                .Mov_Det_Des = Mov_Det_Des,
                .Udm_Cod = Udm_Cod,
                .Qta = Qta,
                .Cod_Iva = -1,
                .Sconto = 0,
                .Prezzo_Unitario = Prezzo,
                .Cod_Conto = 0,
                .Cod_Progetto = 0,
                .Fase_Cod = 0,
                .Contabilizzato = CONTABILE,
                .Pendente = Causale,
                .Validita_Inizio = CDate(Data_Doc),
                .Validita_Fine = AGRODATAFINE,
                .Cal_Cod = 0,
                .Extra_Str = Note,
                .Extra_Int = 0,
                .Extra_Date = AGRODATAINIZIO,
                .Anno = Year(CDate(Data_Doc)),
                .Ric_Cod = 2,
                .Imponibile = -Prezzo * Qta,
                .Iva = 0,
                .Lotto = Lotto,
                .Jolly_Int = MagazzinoMovimentato,
                .Imponibile_Netto = -Prezzo * Qta,
                .Prezzo_Unitario_Netto = Prezzo,
                .Udm_Cod_Extra = 0,
                .Qta_Extra = 0,
                .Prezzo_Effettivo = 0,
                .ChkIva_Manuale = 0,
                .Cod_IvaIndetraibile = 0,
                .Qta_Extra_Totale = 0,
                .Tara = 0,
                .ChkLayOut_Hide = 0,
                .Variazione = 0,
                .Listino_Cod = 0,
                .Sconto_Listino = 0,
                .Sconto_Modalita = 0,
                .Mezzo_Det = -1,
                .Mat_Cod_Alias = 0,
                .Sconto_Testo = "",
                .Ric_Cod_Pat = 2,
                .Cod_Conto_Pat = CONTO_PAT_DEBITI_VS_FORNITORI,
                .TempoCarenza = 0,
                .DoseEtichetta = "",
                .Turno_Cod = 0,
                .ID_Attivita = 0,
                .Veg_Cod = 0,
                .Iva_Indetraibile = 0,
                .Iva_Indetraibile_Perc = 0,
                .PrincipiAttivi = "",
                .CLassiTossicologiche = "",
                .DoseEtichetta_Value = "",
                .Iva_Deto_Cod = 0,
                .Qta_Dettaglio1 = 0,
                .Qta_Dettaglio2 = 0,
                .Dettagli_Blocco_Flag = 0,
                .Dettagli_Blocco_Username = 0,
                .Dettagli_Blocco_Data = AGRODATAINIZIO,
                .Qualifica_Cod = 0,
                .Tariffa_Cod = 0,
                .Ordine_Det = 1,
                .Deroga_Cod = enum_Deroga_Teleregistri.Regola_Base,
                .Prezzo_Livello = 0,
                .Rif_Esterno = "",
                .Rif_Esterno_2 = ""
            }

            '#######################################################
            '################   MOVIMENTI DESTINAZIONE    ##########
            '#######################################################

            Dim Movimento_Destinazione = New Movimento_Destinazione With {
                .Piva = Piva_Dest,
                .Sa_Cod = Sa_Cod_Dest,
                .Id_Agenda = Id_Agenda,
                .Id_Mov = Movimento.Id_Mov,
                .Id_Mov_Det = Movimento_Dettaglio.Id_Mov_Det,
                .Appezza = 0,
                .Id_Destinazione = Magazzino_Cod,
                .Tipo = MAGAZZINO,
                .Qta = Qta,
                .Qta2 = Qta,
                .Data = Data_Doc,
                .Tipo_Scorta = 0,
                .Scorta_Min = 0,
                .mov_destinazioni_graphickey = "",
                .Qta_Dest1 = 0,
                .Qta_Dest2 = 0,
                .QuotaDistribuzione = 0
            }

            Movimento_Dettaglio.Movimenti_Destinazioni.Add(Movimento_Destinazione)

            '#######################################################
            '##############   MOVIMENTO TECNICO EXTRA   ############
            '#######################################################

            Dim Movimento_Dettaglio_Tecnico_Extra = New Movimento_Dettaglio_Tecnico_Extra With {
                .Piva = Piva_Dest,
                .Sa_Cod = Sa_Cod_Dest,
                .Id_Agenda = Id_Agenda,
                .Id_Mov = Movimento.Id_Mov,
                .Id_Mov_Det = Movimento_Dettaglio.Id_Mov_Det,
                .Regione = "",
                .ASL = "",
                .Serie = "",
                .Numero = "",
                .Mac_Cod = 0,
                .Cod_RisUm = 0,
                .Trasportatore = "",
                .Mezzo_Trasporto = "",
                .Targa = "",
                .N_Immatricolazione = "",
                .N_Immatricolazione_Rimorchio = "",
                .N_Autorizzazione_Trasporto = "",
                .Data_Rilascio_Autorizzazione = AGRODATAINIZIO,
                .Peso = 0,
                .Validita_Inizio = Data_Doc,
                .Validita_Fine = AGRODATAFINE,
                .Codice_Prodotto = 0,
                .Colore = 0,
                .Zona_Viticola = "",
                .Manipolazioni = 0,
                .Precisazioni = "",
                .Annotazioni = "",
                .Num_Contenitori = 0,
                .Marche_Contenitori = "",
                .Des_Contenitori = "",
                .Tipo_Documento = "",
                .Id_Cod_Autorita = 0,
                .Luogo_Partenza = "",
                .Luogo_Consegna = "",
                .Data_Spedizione = AGRODATAINIZIO,
                .Indicazioni_Complementari = "",
                .Titolo_Alcol = 0,
                .Codice_NC = "",
                .Num_Riferimento = "",
                .Data_Dichiarazione = AGRODATAINIZIO,
                .Garanzia = "",
                .Certificati = "",
                .Durata_Viaggio = "",
                .Peso_Lordo = 0,
                .Num_Colli = 0,
                .Contenitore_Cod = 0,
                .Imballaggio_Cod = 0,
                .Agente_Cod = 0,
                .Provvigione = 0,
                .Tipo_Trasporto = 0,
                .Unita_Trasporto = 0,
                .Codice_Alternativo = "",
                .Id_Gestione_Vettore = 0,
                .Ritenuta_Acconto_Cod = 0,
                .Ritenuta_Acconto = 0,
                .Enasarco_Cod = 0,
                .Enasarco = 0,
                .ACCDAA_Cod_Risum_Destinatario = 0,
                .ACCDAA_Cod_Risum_Destinazione = 0,
                .ACCDAA_Cod_IndirizzoRisum_Destinatario = 0,
                .ACCDAA_Cod_IndirizzoRisum_Destinazione = 0
            }

            Movimento_Dettaglio.Movimenti_Dettagli_Tecnici_Extra.Add(Movimento_Dettaglio_Tecnico_Extra)

            Movimento.Movimenti_Dettagli.Add(Movimento_Dettaglio)

        Next

        Agenda.Movimenti.Add(Movimento_Testata)

        Agenda.Movimenti.Add(Movimento)

        Return Agenda

    End Function


    Public Function Genera_Agenda_Da_MovimentoAPP(ByRef movimentoAPP As MovimentoDiMagazzino, Optional ByVal Id_Agenda As Integer = 0, Optional ByVal Progressivo_Gias As Integer = 0) As Operazione_Agenda

        Dim BaseCode As Integer = 0
        Dim TopCode As Integer = 200000000

        ' per l'agenda non è richiesto basecode e topcode
        If Progressivo_Gias <> 0 Then
            UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, Progressivo_Gias)
        End If

        Dim Piva = movimentoAPP.Magazzino.primaryKey.centroAziendalePK.partitaIva
        Dim Sa_Cod = movimentoAPP.Magazzino.primaryKey.centroAziendalePK.codice
        Dim Magazzino_Cod = movimentoAPP.Magazzino.primaryKey.codice
        Dim Lav_Cod = If(movimentoAPP.Tipo = 1, LAVCOD_CARICO, LAVCOD_SCARICO)
        Dim Cau_Mov = If(movimentoAPP.Tipo = 1, CAU_CARICO, CAU_SCARICO)
        Dim Des_Lib = If(movimentoAPP.Tipo = 1, "Carico", "Scarico") & " di Magazzino (" & movimentoAPP.Prodotto.descrizione & ")"
        Dim Mov_Desc = movimentoAPP.Note 'movmag.Descrizione
        Dim Elem_Cod = movimentoAPP.Prodotto.elemCod
        Dim prodotto As Integer = movimentoAPP.Prodotto.codice
        If (Elem_Cod = SEMENTI OrElse Elem_Cod = TRASFORMATI_VEGETALI) AndAlso prodotto > 0 Then
            prodotto = -prodotto
        End If
        Dim Pro_Cod = If(prodotto > 0, prodotto, 0)
        Dim Mat_Cod = If(prodotto < 0, -prodotto, 0)
        Dim Mov_Det_Des = movimentoAPP.Prodotto.descrizione
        Dim Lotto = movimentoAPP.Lotto
        Dim Udm_Cod = movimentoAPP.UdM.codice
        Dim Qta = movimentoAPP.Qta
        Dim Prezzo = movimentoAPP.Prezzo
        Dim Causale = enum_Pendenza.GiacenzeIniziali
        Dim N As Decimal = 0
        Dim P2O5 As Decimal = 0
        Dim K2O As Decimal = 0
        Dim Cu As Decimal = 0

        '-----------------------------------------------------------------------
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        Dim Movimento_Destinazione As Movimento_Destinazione

        '#######################################################
        '##################   AGENDA   #########################
        '#######################################################

        Dim Agenda As New Operazione_Agenda With {
            .Tipo_Operazione = enum_TipoOperazioneDB.Scrittura,
            .Id_Agenda = Id_Agenda,
            .Data = movimentoAPP.Data,
            .Piva = Piva,
            .Sa_Cod = 0,
            .Lav_Cod = Lav_Cod,
            .Des_Lib = Des_Lib,
            .BaseCode = BaseCode,
            .TopCode = TopCode
        }
        Agenda.Movimenti = New List(Of Movimento)

        '#######################################################
        '###########   MOVIMENTO DI CARICO / SCARICO    ########
        '#######################################################

        Movimento = New Movimento With {
            .Id_Agenda = Id_Agenda,
            .Piva = Piva,
            .Sa_Cod = 0,
            .Data = movimentoAPP.Data.Date,
            .Lav_Cod = Lav_Cod,
            .Cau_Mov = Cau_Mov,
            .Mov_Desc = Mov_Desc,
            .Ora = movimentoAPP.Data,
            .Data_Registrazione = movimentoAPP.Data,
            .Scadenza = AGRODATAFINE,
            .BaseCode = BaseCode,
            .TopCode = TopCode
        }
        Agenda.Movimenti.Add(Movimento)
        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        '#######################################################
        '################   MOVIMENTI DETTAGLIO    #############
        '#######################################################

        Movimento_Dettaglio = New Movimento_Dettaglio With {
            .Id_Agenda = Id_Agenda,
            .Piva = Piva,
            .Sa_Cod = Sa_Cod,
            .Data = movimentoAPP.Data,
            .Lav_Cod = Lav_Cod,
            .Cau_Mov = Cau_Mov,
            .Mov_Det_Des = Mov_Det_Des,
            .Elem_Cod = Elem_Cod,
            .Pro_Cod = Pro_Cod,
            .Mat_Cod = Mat_Cod,
            .Cod_Progetto = 0,
            .Fase_Cod = 0,
            .Lotto = Lotto,
            .Cal_Cod = 0,
            .Udm_Cod = Udm_Cod,
            .Udm_Cod_Extra = 0,
            .Qta = Qta,
            .Qta_Extra = 0,
            .Pendente = Causale,
            .Prezzo_Effettivo = 0,
            .Prezzo_Unitario = Prezzo,
            .Prezzo_Unitario_Netto = Prezzo,
            .Sconto = 0,
            .Imponibile = Prezzo * Qta,
            .Imponibile_Netto = Prezzo * Qta,
            .Cod_Iva = 0,
            .Iva = 0,
            .Anno = 0,
            .Ric_Cod = 0,
            .Cod_Conto = 0,
            .Extra_Int = 0,
            .Extra_Str = "",
            .Extra_Date = AGRODATAINIZIO,
            .Contabilizzato = NONCONTABILE,
            .Rif_Esterno = "",
            .Rif_Esterno_2 = "",
            .Validita_Inizio = movimentoAPP.Data,
            .BaseCode = BaseCode,
            .TopCode = TopCode
        }
        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)
        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

        '#######################################################
        '################   MOVIMENTI DESTINAZIONE    ##########
        '#######################################################

        Movimento_Destinazione = New Movimento_Destinazione With {
            .Id_Agenda = Id_Agenda,
            .Piva = Piva,
            .Sa_Cod = Sa_Cod,
            .Appezza = 0,
            .Id_Destinazione = Magazzino_Cod,
            .Tipo = MAGAZZINO,
            .Qta = Qta,
            .Data = movimentoAPP.Data,
            .BaseCode = BaseCode,
            .TopCode = TopCode
        }
        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Destinazioni.Add(Movimento_Destinazione)

        '#######################################################
        '################   MOVIMENTO TECNICO    ###############
        '#######################################################

        If Cau_Mov = CAU_CARICO AndAlso Elem_Cod = FERTILIZZANTI Then
            Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico With {
                .Id_Agenda = Id_Agenda,
                .Piva = Piva,
                .Sa_Cod = Sa_Cod,
                .N = N,
                .P = P2O5,
                .K = K2O,
                .Cu = Cu,
                .Extra_Int = 0,
                .Data = movimentoAPP.Data,
                .BaseCode = BaseCode,
                .TopCode = TopCode
            }
            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)
        End If

        Return Agenda

    End Function

    Function Scrivi_Agenda(ByRef agenda As Operazione_Agenda, ByVal cancellato As Boolean, ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim objAgendaHelper As New Agenda_Operazione_Helper
        Dim Id_Agenda As Integer = agenda.Id_Agenda

        Try

            'apri transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            If Id_Agenda <> 0 Then
                objAgendaHelper.Cancella(agenda.Piva, agenda.Sa_Cod, agenda.Id_Agenda, False, objParametri, logCancellazione:=cancellato)
            End If

            If Not cancellato Then
                Id_Agenda = objAgendaHelper.Scrivi(agenda, objParametri)
            End If

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception

            'commit transazione rollback
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri)

        Finally

            'chiudi connessione
            ConnessioniTransazioni.ChiudiConnessione(objParametri)

        End Try

        Return Id_Agenda

    End Function

End Class
