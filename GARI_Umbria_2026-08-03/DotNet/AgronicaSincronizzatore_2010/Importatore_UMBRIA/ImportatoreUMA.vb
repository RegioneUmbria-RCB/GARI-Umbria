Imports System.IO
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreUmaDal
Imports AgronicaCoreVarieBIZ

Public Class ImportatoreUMA
    Inherits DataProvider

    Private _pathFileLog As String
    Private _logDescrizioneUtente As String
    Private _logStreamWriter As StreamWriter

    Public Function AvviaImportazione(ByVal pathExcel As String,
                                      ByVal directoryLog As String,
                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                      ByRef objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard

        Dim methodInfo = Reflection.MethodBase.GetCurrentMethod()
        Dim NomeRoutine As String = methodInfo.ReflectedType.FullName & "." & methodInfo.Name
        Dim nomeImport = "UMA_RichiesteCarburanti"

        Dim r As New RispostaStandard()
        r.RispostaOK = True
        Dim listaOk As New List(Of String)
        Dim listaErrori As New List(Of String)
        Try

            'Operazioni per il log: controllo di avere una cartella, eventualmente creo una sottocartella specifica e dichiaro il nome file per questa importazione
            If directoryLog = "" Then
                directoryLog = "C:\GIASLAN"
            End If
            Dim directoryLogImport = directoryLog & "\Importazione_" & nomeImport & "_Log"
            If Not Directory.Exists(directoryLogImport) Then
                Directory.CreateDirectory(directoryLogImport)
            End If
            _pathFileLog = directoryLogImport & "\" & nomeImport & "__" & Now.ToString("yyyy-MM-dd_(hh.mm.ss)") & ".txt"
            _logDescrizioneUtente = objParametri_Server.LogDescrizioneUtente
            'Inizio a scrivere sul file
            AggiungiLog(NomeRoutine, "Avvio importazione " & pathExcel)

            Dim excelProvider As String
            If Environment.Is64BitProcess Then
                excelProvider = "Microsoft.ACE.OLEDB.12.0"
            Else
                excelProvider = "Microsoft.Jet.OLEDB.4.0"
            End If

            Dim stringaConnExcel = String.Format("PROVIDER={0};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';data source='{1}'", excelProvider, pathExcel)

            Dim dtExcel As New DataTable()
            Dim errCaricamentoDatiExcel As String = ""

            Crea_DtDati_Da_Excel(stringaConnExcel, errCaricamentoDatiExcel, dtExcel)

            AggiungiLog(NomeRoutine, "Completamento lettura dati da excel")

            Dim listaFascicoli As New List(Of Dictionary(Of String, Object))
            Dim numFascicoliIgnorati As Integer = 0

            If errCaricamentoDatiExcel.Length = 0 AndAlso dtExcel.Rows.Count > 0 Then
                '-------- LEGGIMI -----------------
                'L'excel utilizzato dall'importazione contiene in ogni riga un record da inserire nella tabella "uma_richieste_lavorazioni",
                'la quale rappresenta il livello di maggior dettaglio. Occorre quindi fare delle aggregazioni per calcolare i record 
                'delle richieste e delle testate
                'Da sommare: carburante
                'Da prendere il max: superficie
                'Per richieste: occorre raggruppare per piva, num/anno pratica e gruppo_colturale_uma
                'Per richieste_testata: occorre raggruppare per piva, num/anno pratica

                'DA VERIFICARE Quanto segue per le richieste terzisti - AD ORA LE IGNORO
                'Nelle testate ho la piva del terzista
                'Nelle richieste ho la piva dell'azienda per la quale viene effettuata la lavorazione
                '---------------------------------

                'Per le colonne che sono considerate string dall'excel, ma che rappresentano double per il codice, parto del seguente requisito:
                '1) Non viene usato il separatore delle migliaia
                'Come separatore dei decimali, necessito della virgola, pertanto, nel caso la stringa abbia il punto lo sostituisco, altrimenti
                'la stringa è già formattata correttamente con la virgola, in questo modo sono coerente con entrambe le situazioni iniziali

                'Per Debug:
                'For Each rowExcel As DataRow In dtExcel.Rows
                '    Try
                '        Dim dummy = rowExcel.Field(Of String)("data_inizio_lavorazione")
                '        Dim dummy2 = rowExcel.Field(Of Date)("data_fine_lavorazione")
                '    Catch ex As Exception
                '        Dim mesErrore = ex.Message
                '    End Try
                'Next

                Dim queryRichiesteTestata =
                    From richLav In dtExcel.AsEnumerable()
                    Where richLav.Field(Of String)("tipo_richiesta").Trim() = "0"
                    Group richLav By
                        piva = richLav.Field(Of String)("piva").Trim(),
                        cuaa = richLav.Field(Of String)("cuaa").Trim(),
                        annoPrat = richLav.Field(Of String)("anno").Trim(),
                        numPrat = richLav.Field(Of Double)("numero_richiesta"),
                        tipoRich = richLav.Field(Of String)("tipo_richiesta").Trim()
                        Into g = Group
                    Select New With {
                        .Piva = piva,
                        .Cuaa = cuaa,
                        .Anno = annoPrat,
                        .Numero = numPrat,
                        .TipoRichiesta = tipoRich,
                        .CarburanteCalcolato = g.Sum(Function(richLav) CDbl(richLav.Field(Of String)("carburante_calcolato").Trim().Replace(".", ","))),
                        .CarburanteAssegnato = g.Sum(Function(richLav) CDbl(richLav.Field(Of String)("carburante_assegnato").Trim().Replace(".", ",")))
                    }

                Dim queryRichieste =
                    From richLav In dtExcel.AsEnumerable()
                    Where richLav.Field(Of String)("tipo_richiesta").Trim() = "0"
                    Group richLav By
                        piva = richLav.Field(Of String)("piva").Trim(),
                        cuaa = richLav.Field(Of String)("cuaa").Trim(),
                        annoPrat = richLav.Field(Of String)("anno").Trim(),
                        numPrat = richLav.Field(Of Double)("numero_richiesta"),
                        tipoRich = richLav.Field(Of String)("tipo_richiesta").Trim(),
                        gruppoColt = richLav.Field(Of String)("codice_gruppo_colturale_uma").Trim()
                        Into g = Group
                    Select New With {
                        .Piva = piva,
                        .Cuaa = cuaa,
                        .Anno = annoPrat,
                        .Numero = numPrat,
                        .TipoRichiesta = tipoRich,
                        .GruppoColturale = gruppoColt,
                        .CarburanteCalcolato = g.Sum(Function(richLav) CDbl(richLav.Field(Of String)("carburante_calcolato").Trim().Replace(".", ","))),
                        .CarburanteAssegnato = g.Sum(Function(richLav) CDbl(richLav.Field(Of String)("carburante_assegnato").Trim().Replace(".", ","))),
                        .SupTotale = g.Max(Function(richLav) CDbl(richLav.Field(Of String)("totale_superficie_uma_lavorazione").Trim().Replace(".", ","))),
                        .SupPendenzaA = g.Max(Function(richLav) CDbl(richLav.Field(Of String)("zona_pendenza_a_lavorazione").Trim().Replace(".", ","))),
                        .SupPendenzaB = g.Max(Function(richLav) CDbl(richLav.Field(Of String)("zona_pendenza_b_lavorazione").Trim().Replace(".", ","))),
                        .SupTessituraNormale = g.Max(Function(richLav) CDbl(richLav.Field(Of String)("zona_tessitura_normale_lavorazione").Trim().Replace(".", ","))),
                        .SupTessituraMedia = g.Max(Function(richLav) CDbl(richLav.Field(Of String)("zona_tessitura_media_lavorazione").Trim().Replace(".", ","))),
                        .SupTessituraTenace = g.Max(Function(richLav) CDbl(richLav.Field(Of String)("zona_tessitura_tenace_lavorazione").Trim().Replace(".", ",")))
                    }

                '----------------------------------------------------------------------------------------------------------
                '--------------------------------------- PREPARO STRUTTURA DATI  ------------------------------------------
                ' La struttura è pensata come segue:
                ' una lista di oggetti rappresentanti ognuno un entità atomica composta dalla pratica, 
                ' dalla testata delle richieste, da queste ultime e dalle lavorazioni; entità che ho chiamato "fascicolo".
                ' La lista contiene tutti i dati che vengono inseriti a db ed ogni fascicolo è gestito sotto transazione
                ' Ogni singolo oggetto, rappresentato come dictionary ha la seguente struttura
                ' { 
                '   Pratica => Pratica, 
                '   Testata => Uma_Richieste_Testata, 
                '   Richieste => List(Of Uma_Richieste), 
                '   Lavorazioni => List(Of Uma_Richieste_Lavorazioni) 
                ' }
                '----------------------------------------------------------------------------------------------------------

                AggiungiLog(NomeRoutine, "Inizio creazione struttura dati")

                Dim handleMacrousixLavorazioni As New UMA_Configurazione_MacrousixLavorazioni_R()
                Dim handlePratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R()

                For Each rowAggregatiRichiesta In queryRichiesteTestata

                    Dim praticaAnno As String = rowAggregatiRichiesta.Anno
                    Dim praticaNumero As Double = rowAggregatiRichiesta.Numero
                    Dim praticaPiva As String = rowAggregatiRichiesta.Piva

                    Dim filtroAgg = String.Format("ANNO = {0} AND Numero = '{1}'", praticaAnno, praticaNumero)
                    Dim dtPratica = handlePratiche.Leggi_2(0, praticaPiva, "", 0, 0, 0, enum_Servizi.Gestione_UMA, AGRODATAINIZIO, AGRODATAFINE, filtroAgg, "", objParametri_Server, False)
                    If dtPratica.Rows.Count > 0 Then
                        'Nel caso la pratica con anno e numero attuali sia già presente su database la ignoro
                        numFascicoliIgnorati += 1
                        Dim mesPraticaDuplicata = String.Format(" Azienda {0} Pratica {1}/{2} ignorata perché già inserita", praticaPiva, praticaNumero, praticaAnno)
                        listaErrori.Add(mesPraticaDuplicata)
                        AggiungiLog(NomeRoutine, mesPraticaDuplicata)
                        Continue For
                    End If

                    Dim dictionaryFascicolo As New Dictionary(Of String, Object)

                    Dim pratica As New Pratiche With {
                        .Piva_SuperUser = objParametri_Server.PivaSuperUser,
                        .Pratica_Des = rowAggregatiRichiesta.Cuaa & " - " & enum_Servizi.Gestione_UMA,
                        .Piva = praticaPiva,
                        .Cuaa = rowAggregatiRichiesta.Cuaa,
                        .Servizio_Cod = enum_Servizi.Gestione_UMA,
                        .Anno = praticaAnno,
                        .Numero = praticaNumero,
                        .Username_Creazione = objParametri_Server.SuperUserUsername,
                        .Username_Modifica = objParametri_Server.SuperUserUsername,
                        .Data_Creazione = DateTime.Now,
                        .Data_Modifica = DateTime.Now,
                        .Validita_Inizio = New Date(praticaAnno, 1, 1),
                        .Validita_Fine = New Date(praticaAnno, 12, 31)
                    }

                    dictionaryFascicolo.Add("Pratica", pratica)

                    Dim richiestaTestata As New UMA_Richieste_Testata With {
                        .Piva_SuperUser = objParametri_Server.PivaSuperUser,
                        .Piva = rowAggregatiRichiesta.Piva,
                        .Tipo_Richiesta = rowAggregatiRichiesta.TipoRichiesta,
                        .Carburante_Calcolato = rowAggregatiRichiesta.CarburanteCalcolato,
                        .Carburante_Richiesto = rowAggregatiRichiesta.CarburanteAssegnato,
                        .Carburante_Approvato = rowAggregatiRichiesta.CarburanteAssegnato,
                        .Avanzamento_Richiesta = 1,
                        .Carburante_Richiesto_Benzina = 0,
                        .Carburante_Richiesto_Gasolio = 0,
                        .Carburante_Richiesto_Gasolio_Serra = 0,
                        .Macchine_Impiegate = "",
                        .Username_Creazione = objParametri_Server.SuperUserUsername,
                        .Username_Modifica = objParametri_Server.SuperUserUsername,
                        .Data_Creazione = DateTime.Now,
                        .Data_Modifica = DateTime.Now,
                        .Validita_Inizio = AGRODATAINIZIO,
                        .Validita_Fine = AGRODATAFINE
                    }

                    dictionaryFascicolo.Add("Testata", richiestaTestata)

                    Dim richiestePerTestata =
                        From rich In queryRichieste
                        Where rich.Piva = pratica.Piva And
                            rich.Anno = pratica.Anno And rich.Numero = pratica.Numero

                    Dim listaRichieste As New List(Of UMA_Richieste)
                    For Each rowRich In richiestePerTestata

                        Dim richiesta As New UMA_Richieste With {
                            .Piva_SuperUser = objParametri_Server.PivaSuperUser,
                            .Piva = rowRich.Piva,
                            .Gruppo_Colturale_UMA = rowRich.GruppoColturale,
                            .Totale_Superficie_UMA = rowRich.SupTotale,
                            .Zona_Pendenza_A_UMA = rowRich.SupPendenzaA,
                            .Zona_Pendenza_B_UMA = rowRich.SupPendenzaB,
                            .Zona_Tessitura_Normale_UMA = rowRich.SupTessituraNormale,
                            .Zona_Tessitura_Media_UMA = rowRich.SupTessituraMedia,
                            .Zona_Tessitura_Tenace_UMA = rowRich.SupTessituraTenace,
                            .Carburante_Calcolato = rowRich.CarburanteCalcolato,
                            .Carburante_Richiesto = rowRich.CarburanteAssegnato,
                            .Carburante_Approvato = rowRich.CarburanteAssegnato,
                            .Totale_Superficie_UMA_Edit = rowRich.SupTotale,
                            .Zona_Pendenza_A_UMA_Edit = rowRich.SupPendenzaA,
                            .Zona_Pendenza_B_UMA_Edit = rowRich.SupPendenzaB,
                            .Zona_Tessitura_Normale_UMA_Edit = rowRich.SupTessituraNormale,
                            .Zona_Tessitura_Media_UMA_Edit = rowRich.SupTessituraMedia,
                            .Zona_Tessitura_Tenace_UMA_Edit = rowRich.SupTessituraTenace,
                            .Username_Creazione = objParametri_Server.SuperUserUsername,
                            .Username_Modifica = objParametri_Server.SuperUserUsername,
                            .Data_Creazione = DateTime.Now,
                            .Data_Modifica = DateTime.Now,
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE
                        }

                        listaRichieste.Add(richiesta)

                    Next

                    dictionaryFascicolo.Add("Richieste", listaRichieste)

                    'A livello logico le Lavorazioni sono dipendenti da una Richiesta, mentre a livello strutturale sono direttamente dipendenti nell'excel da
                    'numero/anno pratica
                    'mentre nel db da richiesta_cod che è la chiave di Testata
                    'pertanto non ho bisogno di inserire le lavorazioni nella struttura del mio dictionary come figlie delle Richieste
                    Dim lavorazioniPerTestata =
                        From richLav In dtExcel.AsEnumerable()
                        Where richLav.Field(Of String)("piva").Trim() = pratica.Piva And
                            richLav.Field(Of String)("anno").Trim() = pratica.Anno And richLav.Field(Of Double)("numero_richiesta") = pratica.Numero And
                            richLav.Field(Of String)("tipo_richiesta").Trim() = "0"

                    Dim listaLavorazioni As New List(Of UMA_Richieste_Lavorazioni)
                    For Each drExcel In lavorazioniPerTestata

                        Dim gruppoColturaleUma As String = drExcel.Field(Of String)("codice_gruppo_colturale_uma").Trim()
                        Dim lavorazioneUma As String = drExcel.Field(Of String)("codice_lavorazione").Trim()

                        'Calcolo il codice Lavorazione_Gias e Id_Attivita a partire da Lavorazione_Uma e Gruppo_Colturale
                        Dim dtMacrousixLav = handleMacrousixLavorazioni.Leggi("", gruppoColturaleUma, lavorazioneUma, 0, 0, objParametri_Server)
                        Dim lavGias = ""
                        Dim idAttivita = 0
                        If dtMacrousixLav IsNot Nothing AndAlso dtMacrousixLav.Rows.Count > 0 Then
                            'Nota: ad ora riteniamo corretto prelevare la prima corrispondenza trovata nella tabella di collegamento
                            lavGias = dtMacrousixLav.Rows(0)("Lav_Cod")
                            idAttivita = dtMacrousixLav.Rows(0)("Id_Attivita")
                        End If

                        Dim richiestaLavorazione As New UMA_Richieste_Lavorazioni With {
                            .Piva_SuperUser = objParametri_Server.PivaSuperUser,
                            .Piva = drExcel.Field(Of String)("piva").Trim(),
                            .Gruppo_Colturale_UMA = gruppoColturaleUma,
                            .Lavorazione_UMA = lavorazioneUma,
                            .Lavorazione_GIAS = lavGias,
                            .Id_Attivita = idAttivita,
                            .Tipo_Carburante = drExcel.Field(Of String)("carburante").Trim(),
                            .Superficie_Maggiorazione_Trasferimenti = CDbl(drExcel.Field(Of String)("superficie_maggiorazione_trasferimenti").Trim().Replace(".", ",")),
                            .Nr_Lavorazioni_Previste = drExcel.Field(Of Double)("n_lavorazioni"),
                            .Nr_Lavorazioni_Richieste = drExcel.Field(Of Double)("n_lavorazioni"),
                            .Fabbisogno_Calcolato = CDbl(drExcel.Field(Of String)("carburante_calcolato").Trim().Replace(".", ",")),
                            .Fabbisogno_Richiesto = CDbl(drExcel.Field(Of String)("carburante_assegnato").Trim().Replace(".", ",")),
                            .Fabbisogno_Assegnato = CDbl(drExcel.Field(Of String)("carburante_assegnato").Trim().Replace(".", ",")),
                            .Totale_Superficie_UMA = CDbl(drExcel.Field(Of String)("totale_superficie_uma_lavorazione").Trim().Replace(".", ",")),
                            .Zona_Pendenza_A_UMA = CDbl(drExcel.Field(Of String)("zona_pendenza_a_lavorazione").Trim().Replace(".", ",")),
                            .Zona_Pendenza_B_UMA = CDbl(drExcel.Field(Of String)("zona_pendenza_b_lavorazione").Trim().Replace(".", ",")),
                            .Zona_Tessitura_Normale_UMA = CDbl(drExcel.Field(Of String)("zona_tessitura_normale_lavorazione").Trim().Replace(".", ",")),
                            .Zona_Tessitura_Media_UMA = CDbl(drExcel.Field(Of String)("zona_tessitura_media_lavorazione").Trim().Replace(".", ",")),
                            .Zona_Tessitura_Tenace_UMA = CDbl(drExcel.Field(Of String)("zona_tessitura_tenace_lavorazione").Trim().Replace(".", ",")),
                            .Note_Approvatore = "",
                            .Note_Compilatore = "",
                            .Mesi = 0,
                            .Qta_Manuale = 0,
                            .Username_Creazione = objParametri_Server.SuperUserUsername,
                            .Username_Modifica = objParametri_Server.SuperUserUsername,
                            .Data_Creazione = DateTime.Now,
                            .Data_Modifica = DateTime.Now,
                            .Validita_Inizio = drExcel.Field(Of Date)("data_inizio_lavorazione"),
                            .Validita_Fine = drExcel.Field(Of Date)("data_fine_lavorazione")
                        }

                        listaLavorazioni.Add(richiestaLavorazione)

                    Next

                    dictionaryFascicolo.Add("Lavorazioni", listaLavorazioni)

                    listaFascicoli.Add(dictionaryFascicolo)

                    AggiungiLog(NomeRoutine, String.Format(" Azienda {0} trovata pratica {1}/{2}", pratica.Piva, pratica.Numero, pratica.Anno))
                Next

                AggiungiLog(NomeRoutine, "Completamento creazione struttura dati. Inizio insert su database")
                'handleLog.Scrivi_LOG(directoryLogImport, _nomeFileLog, objParametri_Server.LogDescrizioneUtente, NomeRoutine, 
                '                 "Completamento creazione struttura dati. Inizio insert su database")

                '-----------------------------------------
                '---- INSERT EFFETTIVO DEI DATI  ---------
                '-----------------------------------------  

                Dim bizUmaRichieste As New AgronicaCoreUmaBiz.UMA_Richieste()
                For Each dictfascicolo In listaFascicoli

                    Dim rispInsert = bizUmaRichieste.InsertPraticaRichiesteLavorazioni(dictfascicolo, objParametri_Server, objParametri_Utenti)

                    Dim objPratica = CType(dictfascicolo("Pratica"), Pratiche)
                    If rispInsert.RispostaOK Then
                        listaOk.Add(String.Format(" Azienda {0} Pratica {1}/{2}", objPratica.Piva, objPratica.Numero, objPratica.Anno))
                        AggiungiLog(NomeRoutine, String.Format(" Azienda {0} Pratica {1}/{2} inserita", objPratica.Piva, objPratica.Numero, objPratica.Anno))
                    Else
                        Dim mesErrore = String.Format("Azienda {0} Pratica {1}/{2}: {3}", objPratica.Piva, objPratica.Numero, objPratica.Anno, rispInsert.Errore)
                        listaErrori.Add(mesErrore)
                        AggiungiLog(NomeRoutine, mesErrore)
                    End If

                Next

            Else 'Errori excel

                If errCaricamentoDatiExcel.Length > 0 Then
                    listaErrori.Add(errCaricamentoDatiExcel)
                    AggiungiLog(NomeRoutine, errCaricamentoDatiExcel)
                End If

                If dtExcel.Rows.Count = 0 Then
                    listaErrori.Add("L'excel non contiene dati")
                    AggiungiLog(NomeRoutine, "L'excel non contiene dati")
                End If

            End If

            If listaOk.Count > 0 Then
                Dim mesOk = String.Format(
                    "Inserite con successo {0} pratiche/testate su {1} presenti e considerate nel file. Ignorate {2} pratiche/testate già presenti su database",
                    listaOk.Count, listaFascicoli.Count, numFascicoliIgnorati
                )
                r.RispostaStringa = mesOk
                AggiungiLog(NomeRoutine, vbCrLf & mesOk)
            End If
            If listaErrori.Count > 0 Then
                r.Errore = Newtonsoft.Json.JsonConvert.SerializeObject(listaErrori)
            End If

            Return r

        Catch ex As Exception
            Throw ex
        Finally
            _logStreamWriter.Close()
        End Try

    End Function

    Public Function AvviaImportazionePraticheApprovate(ByVal pathExcel As String, ByVal directoryLog As String, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim methodInfo = Reflection.MethodBase.GetCurrentMethod()
        Dim NomeRoutine As String = methodInfo.ReflectedType.FullName & "." & methodInfo.Name
        Dim nomeImport = "UMA_PratTestateCarburanti"

        Dim r As New RispostaStandard()
        r.RispostaOK = True
        Dim listaOk As New List(Of String)
        Dim listaErrori As New List(Of String)
        Try
            'Operazioni per il log: controllo di avere una cartella, eventualmente creo una sottocartella specifica e dichiaro il nome file per questa importazione
            If directoryLog = "" Then
                directoryLog = "C:\GIASLAN"
            End If
            Dim directoryLogImport = directoryLog & "\Importazione_" & nomeImport & "_Log"
            If Not Directory.Exists(directoryLogImport) Then
                Directory.CreateDirectory(directoryLogImport)
            End If
            _pathFileLog = directoryLogImport & "\" & nomeImport & "__" & Now.ToString("yyyy-MM-dd_(hh.mm.ss)") & ".txt"
            _logDescrizioneUtente = objParametri_Server.LogDescrizioneUtente
            'Inizio a scrivere sul file
            AggiungiLog(NomeRoutine, "Avvio importazione " & pathExcel)

            Dim excelProvider As String
            If Environment.Is64BitProcess Then
                excelProvider = "Microsoft.ACE.OLEDB.12.0"
            Else
                excelProvider = "Microsoft.Jet.OLEDB.4.0"
            End If

            Dim stringaConnExcel = String.Format("PROVIDER={0};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';data source='{1}'", excelProvider, pathExcel)

            Dim dtExcel As New DataTable()
            Dim errCaricamentoDatiExcel As String = ""

            Crea_DtDati_Da_Excel(stringaConnExcel, errCaricamentoDatiExcel, dtExcel)

            AggiungiLog(NomeRoutine, "Completamento lettura dati da excel")

            Dim numTotalePratiche As Integer
            Dim numPratInErrore As Integer
            Dim numPratIgnorateAzDupl As Integer
            Dim numPratIgnorateAzScon As Integer
            Dim numPratIgnoratePrecIns As Integer
            Dim numPratIgnorateAzSuben As Integer
            Dim incrementoPratCorrente As Integer


            If errCaricamentoDatiExcel.Length = 0 AndAlso dtExcel.Rows.Count > 0 Then

                Dim handlePratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R()
                Dim handleImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read()
                Dim handleRichiesteBIZ As New AgronicaCoreUmaBiz.UMA_Richieste()

                Dim impresaPiva As String = ""
                Dim impresaRagSog As String = ""
                Dim impresaCuaa As String = ""
                Dim praticaAnno As Integer
                Dim praticaNumero As Integer
                Dim richiestaTipo As Integer
                Dim richiestaTipoDesc As String = "'Per conto di' non definito"

                'Ordino il datatable prima per cuaa, poi per il tipo dell'ultima richiesta,
                'mettendo prima le richieste di tipo "richiesta iniziale" e dopo quelle di tipo "integrazione"
                Dim dvExcel = dtExcel.DefaultView
                dvExcel.Sort = "cuaa ASC, tipo_dichiarazione_ultima DESC"
                dtExcel = dvExcel.ToTable()

                'Per Debug
                'Dim query =
                '    From drExcel In dtExcel.AsEnumerable
                '    Group drExcel By
                '        cuaa = drExcel.Field(Of String)("cuaa").Trim()
                '        Into g = Group
                '    Where g.Count() > 1
                '    Select New With {
                '        .Cuaa = cuaa,
                '        .Occorrenze = g.Count()
                '    }
                'Dim listaQuery = query.ToList()

                For Each drExcel As DataRow In dtExcel.Rows

                    Select Case drExcel.Field(Of Double)("tipo_assegnazioni")
                        Case 1
                            incrementoPratCorrente = 1
                            richiestaTipo = 0
                            richiestaTipoDesc = "C/PROPRIO"

                        Case 2
                            incrementoPratCorrente = 1
                            richiestaTipo = -1
                            richiestaTipoDesc = "C/TERZI"

                        Case 3
                            'Nel caso in cui ho una riga in C/PROPRIO + C/TERZI la splitto in due pratiche/testate diverse
                            incrementoPratCorrente = 2
                            richiestaTipo = -999 'Non usata, in questo caso
                            richiestaTipoDesc = "C/PROPRIO + C/TERZI"
                    End Select

                    numTotalePratiche += incrementoPratCorrente

                    '----------------------------------------------------------------------------------------------
                    impresaCuaa = drExcel.Field(Of String)("cuaa").Trim()
                    impresaRagSog = drExcel.Field(Of String)("denominazione").Trim()

                    If drExcel.Field(Of String)("cuaa_subentrante") IsNot Nothing AndAlso
                        drExcel.Field(Of String)("cuaa_subentrante").Trim() <> "" Then
                        numPratIgnorateAzSuben += incrementoPratCorrente

                        Dim mesImpresaSuben = String.Format(
                            " Azienda Cuaa.{0} {1} ignorata per la presenza di azienda subentrante",
                            impresaCuaa,
                            impresaRagSog)
                        listaErrori.Add(mesImpresaSuben)
                        AggiungiLog(NomeRoutine, mesImpresaSuben)

                        Continue For
                    End If

                    Try
                        impresaPiva = handleImprese.Piva_From_CUAA(impresaCuaa, objParametri_Server)
                    Catch ex As Exception
                        numPratIgnorateAzDupl += incrementoPratCorrente

                        Dim mesImpresaDuplicata = String.Format(
                            " Azienda Cuaa.{0} {1} ignorata perché più imprese su database presentano questo cuaa",
                            impresaCuaa,
                            impresaRagSog)
                        listaErrori.Add(mesImpresaDuplicata)
                        AggiungiLog(NomeRoutine, mesImpresaDuplicata)

                        Continue For
                    End Try

                    If impresaPiva = "" Then
                        numPratIgnorateAzScon += incrementoPratCorrente

                        Dim mesImpresaSconosciuta = String.Format(" Azienda Cuaa.{0} {1} ignorata perché non presente a database", impresaCuaa, impresaRagSog)
                        listaErrori.Add(mesImpresaSconosciuta)
                        AggiungiLog(NomeRoutine, mesImpresaSconosciuta)

                        Continue For
                    End If

                    praticaAnno = drExcel.Field(Of String)("anno")
                    praticaNumero = drExcel.Field(Of String)("progr_pratica_iniziale")

                    Dim filtroAgg = String.Format("ANNO = {0} AND Numero = '{1}'", praticaAnno, praticaNumero)
                    Dim dtPratica = handlePratiche.Leggi_2(0, impresaPiva, "", 0, 0, 0, enum_Servizi.Gestione_UMA, AGRODATAINIZIO, AGRODATAFINE, filtroAgg, "", objParametri_Server, False)
                    If dtPratica.Rows.Count > 0 Then
                        'Nel caso la pratica con anno e numero attuali sia già presente su database la ignoro
                        numPratIgnoratePrecIns += incrementoPratCorrente

                        Dim mesPraticaDuplicata = String.Format(
                            "Azienda Cuaa.{0} Piva.{1} Pratica {2}/{3} ({4}) ignorata perché già inserita",
                            impresaCuaa,
                            impresaPiva,
                            praticaNumero,
                            praticaAnno,
                            richiestaTipoDesc)
                        listaErrori.Add(mesPraticaDuplicata)
                        AggiungiLog(NomeRoutine, mesPraticaDuplicata)

                        Continue For
                    End If
                    '--------------------------------------------------------------------------------------------------

                    'Queste liste sono sostanzialmente fittizie, in quanto potranno contenere solo uno o due elementi perché sto eseguendo le insert mano a mano che trovo le pratiche
                    Dim listaPratiche As New List(Of Pratiche)
                    Dim listaTestate As New List(Of UMA_Richieste_Testata)

                    If drExcel.Field(Of Double)("tipo_assegnazioni") = 3 Then
                        '---------------------- CONTO PROPRIO ------------------------------------------
                        Dim praticaCP As Pratiche = Nothing
                        Dim testataCP As UMA_Richieste_Testata = Nothing
                        PreparaOggettiPraticaTestata(drExcel, impresaPiva, 0, objParametri_Server, praticaCP, testataCP)
                        listaPratiche.Add(praticaCP)
                        listaTestate.Add(testataCP)

                        '---------------------- CONTO TERZI -------------------------------------------
                        Dim praticaCT As Pratiche = Nothing
                        Dim testataCT As UMA_Richieste_Testata = Nothing
                        PreparaOggettiPraticaTestata(drExcel, impresaPiva, -1, objParametri_Server, praticaCT, testataCT)
                        listaPratiche.Add(praticaCT)
                        listaTestate.Add(testataCT)

                    Else
                        Dim praticaC As Pratiche = Nothing
                        Dim testataC As UMA_Richieste_Testata = Nothing
                        PreparaOggettiPraticaTestata(drExcel, impresaPiva, richiestaTipo, objParametri_Server, praticaC, testataC)
                        listaPratiche.Add(praticaC)
                        listaTestate.Add(testataC)
                    End If

                    '----------------------------------------------------------------------------------------------------------------------------
                    Dim rispInsert = handleRichiesteBIZ.InsertPraticheApprovate(listaPratiche, listaTestate, objParametri_Server, objParametri_Utenti)

                    If rispInsert.RispostaOK Then

                        If drExcel.Field(Of Double)("tipo_assegnazioni") = 3 Then
                            Dim mesOk = String.Format(
                                " Azienda Cuaa.{0} Piva.{1} Pratica {2}/{3} ({4})",
                                listaPratiche(0).Cuaa,
                                listaPratiche(0).Piva,
                                listaPratiche(0).Numero,
                                listaPratiche(0).Anno,
                                "C/PROPRIO")
                            listaOk.Add(mesOk)
                            AggiungiLog(NomeRoutine, mesOk & " inserita")

                            mesOk = String.Format(
                                " Azienda Cuaa.{0} Piva.{1} Pratica {2}/{3} ({4})",
                                listaPratiche(0).Cuaa,
                                listaPratiche(0).Piva,
                                listaPratiche(0).Numero,
                                listaPratiche(0).Anno,
                                "C/TERZI")
                            listaOk.Add(mesOk)
                            AggiungiLog(NomeRoutine, mesOk & " inserita")

                        Else
                            Dim mesOk = String.Format(
                                " Azienda Cuaa.{0} Piva.{1} Pratica {2}/{3} ({4})",
                                listaPratiche(0).Cuaa,
                                listaPratiche(0).Piva,
                                listaPratiche(0).Numero,
                                listaPratiche(0).Anno,
                                richiestaTipoDesc)
                            listaOk.Add(mesOk)
                            AggiungiLog(NomeRoutine, mesOk & " inserita")
                        End If

                    Else
                        numPratInErrore += incrementoPratCorrente

                        Dim mesErrore = String.Format(
                            " Azienda Cuaa.{0} Piva.{1} Pratica {2}/{3} ({4}): {5}",
                            listaPratiche(0).Cuaa,
                            listaPratiche(0).Piva,
                            listaPratiche(0).Numero,
                            listaPratiche(0).Anno,
                            richiestaTipoDesc,
                            rispInsert.Errore)
                        listaErrori.Add(mesErrore)
                        AggiungiLog(NomeRoutine, mesErrore)
                    End If

                    '----------------------------------------------------------------------------------------------------------------------------
                Next

            End If

            If listaOk.Count > 0 Then
                Dim mesOk = String.Format(
                    "A fronte di un excel con {0} righe sono state trovate {1} pratiche. " &
                    "Pratiche inserite con successo {2}. " &
                    "Pratiche in errore {3}. " &
                    "Ignorate {4} pratiche di aziende con cuaa appartenente a più imprese " &
                    "Ignorate {5} pratiche di aziende non presenti a database " &
                    "Ignorate {6} pratiche già presenti su database" &
                    "Ignorate {7} pratiche a causa di azienda subentrante",
                    dtExcel.Rows.Count,
                    numTotalePratiche,
                    listaOk.Count,
                    numPratInErrore,
                    numPratIgnorateAzDupl,
                    numPratIgnorateAzScon,
                    numPratIgnoratePrecIns,
                    numPratIgnorateAzSuben
                )
                r.RispostaStringa = mesOk
                AggiungiLog(NomeRoutine, vbCrLf & mesOk)
            End If
            If listaErrori.Count > 0 Then
                r.Errore = Newtonsoft.Json.JsonConvert.SerializeObject(listaErrori)
            End If

            Return r

        Catch ex As Exception
            Throw ex
        Finally
            _logStreamWriter.Close()
        End Try

    End Function

    Private Sub PreparaOggettiPraticaTestata(ByVal drExcel As DataRow,
                                             ByVal impresaPiva As String,
                                             ByVal richiestaTipo As Integer,
                                             ByVal objParametri_Server As AgronicaCoreParametri,
                                             ByRef objPratica As Pratiche,
                                             ByRef objTestata As UMA_Richieste_Testata)

        Dim impresaCuaa = drExcel.Field(Of String)("cuaa").Trim()
        Dim praticaAnno = drExcel.Field(Of String)("anno")

        Dim handleUmaSetup As New UMASetup_R()
        Dim dtUmaSetup = handleUmaSetup.LeggiSetup(praticaAnno, objParametri_Server)
        Dim percentualeDecurtamento = dtUmaSetup.Rows(0).Field(Of Double)("Per_Riduzione")
        Dim valMoltDecurt = (100 - percentualeDecurtamento) / 100

        objPratica = New Pratiche With {
            .Piva_SuperUser = objParametri_Server.PivaSuperUser,
            .Pratica_Des = impresaCuaa & " - " & enum_Servizi.Gestione_UMA,
            .Piva = impresaPiva,
            .Cuaa = impresaCuaa,
            .Servizio_Cod = enum_Servizi.Gestione_UMA,
            .Anno = praticaAnno,
            .Numero = drExcel.Field(Of String)("progr_pratica_iniziale"),
            .Username_Creazione = objParametri_Server.SuperUserUsername,
            .Username_Modifica = objParametri_Server.SuperUserUsername,
            .Data_Creazione = DateTime.Now,
            .Data_Modifica = DateTime.Now,
            .Validita_Inizio = New Date(praticaAnno, 1, 1),
            .Validita_Fine = New Date(praticaAnno, 12, 31)
        }

        'Conto proprio = 0, Conto terzi = -1
        Dim suffissoNomeColonnaPerTipo As String = IIf(richiestaTipo = -1, "t", "p")

        Dim assegnatoBenzCP = Math.Round(drExcel.Field(Of Double)("assegnato_benzina_c" & suffissoNomeColonnaPerTipo), 4)
        Dim richBenzCP = Math.Round(assegnatoBenzCP / valMoltDecurt, 4)
        Dim assegnatoGasCP = Math.Round(drExcel.Field(Of Double)("assegnato_gasolio_c" & suffissoNomeColonnaPerTipo), 4)
        Dim richGasCP = Math.Round(assegnatoGasCP / valMoltDecurt, 4)
        Dim assegnatoSerCP = Math.Round(drExcel.Field(Of Double)("assegnato_gasolio_serra_c" & suffissoNomeColonnaPerTipo), 4)
        Dim richSerCP = Math.Round(assegnatoSerCP / valMoltDecurt, 4)

        Dim assegnatoCP = assegnatoBenzCP + assegnatoGasCP + assegnatoSerCP
        Dim richiestoCP = richBenzCP + richGasCP + richSerCP

        objTestata = New UMA_Richieste_Testata With {
            .Piva_SuperUser = objParametri_Server.PivaSuperUser,
            .Piva = impresaPiva,
            .Tipo_Richiesta = richiestaTipo,
            .Carburante_Calcolato = richiestoCP,
            .Carburante_Richiesto = richiestoCP,
            .Carburante_Approvato = assegnatoCP,
            .Avanzamento_Richiesta = 0,
            .Carburante_Richiesto_Benzina = richBenzCP,
            .Carburante_Richiesto_Gasolio = richGasCP,
            .Carburante_Richiesto_Gasolio_Serra = richSerCP,
            .Macchine_Impiegate = "",
            .Rimanenza_Benzina = drExcel.Field(Of Double)("rimanenza_ap_benzina_c" & suffissoNomeColonnaPerTipo),
            .Rimanenza_Gasolio = drExcel.Field(Of Double)("rimanenza_ap_gasolio_c" & suffissoNomeColonnaPerTipo),
            .Rimanenza_Gasolio_Serra = drExcel.Field(Of Double)("rimanenza_ap_gasserra_c" & suffissoNomeColonnaPerTipo),
            .Inviato = 0,
            .Username_Creazione = objParametri_Server.SuperUserUsername,
            .Username_Modifica = objParametri_Server.SuperUserUsername,
            .Data_Creazione = DateTime.Now,
            .Data_Modifica = DateTime.Now,
            .Validita_Inizio = AGRODATAINIZIO,
            .Validita_Fine = AGRODATAFINE
        }
    End Sub

    Public Function AvviaImportazioneVendite(ByVal pathExcel As String, ByVal directoryLog As String, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim methodInfo = Reflection.MethodBase.GetCurrentMethod()
        Dim NomeRoutine As String = methodInfo.ReflectedType.FullName & "." & methodInfo.Name
        Dim nomeImport = "UMA_VenditeCarburanti"

        Dim r As New RispostaStandard()
        r.RispostaOK = True
        Dim listaOk As New List(Of String)
        Dim listaErrori As New List(Of String)

        Try
            'Operazioni per il log: controllo di avere una cartella, eventualmente creo una sottocartella specifica e dichiaro il nome file per questa importazione
            If directoryLog = "" Then
                directoryLog = "C:\GIASLAN"
            End If
            Dim directoryLogImport = directoryLog & "\Importazione_" & nomeImport & "_Log"
            If Not Directory.Exists(directoryLogImport) Then
                Directory.CreateDirectory(directoryLogImport)
            End If
            _pathFileLog = directoryLogImport & "\" & nomeImport & "__" & Now.ToString("yyyy-MM-dd_(hh.mm.ss)") & ".txt"
            _logDescrizioneUtente = objParametri_Server.LogDescrizioneUtente
            'Inizio a scrivere sul file
            AggiungiLog(NomeRoutine, "Avvio importazione " & pathExcel)

            Dim excelProvider As String
            If Environment.Is64BitProcess Then
                excelProvider = "Microsoft.ACE.OLEDB.12.0"
            Else
                excelProvider = "Microsoft.Jet.OLEDB.4.0"
            End If

            Dim stringaConnExcel = String.Format("PROVIDER={0};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';data source='{1}'", excelProvider, pathExcel)

            Dim dtExcel As New DataTable()
            Dim errCaricamentoDatiExcel As String = ""

            Crea_DtDati_Da_Excel(stringaConnExcel, errCaricamentoDatiExcel, dtExcel)

            AggiungiLog(NomeRoutine, "Completamento lettura dati da excel")

            Dim numTotaleVendite As Integer
            Dim numVenInErrore As Integer
            Dim numVenIgnorateAzDupl As Integer
            Dim numVenIgnorateAzScon As Integer
            'Dim numPratIgnoratePrecIns As Integer 'Sto gestendo questo caso facendo l'update del record al posto di ignorarlo
            Dim incrementoVenCorrente As Integer

            If errCaricamentoDatiExcel.Length = 0 AndAlso dtExcel.Rows.Count > 0 Then
                Dim handleImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read()

                Dim impresaCuaa As String = ""
                Dim impresaPiva As String = ""
                Dim impresaRagSog As String = ""
                Dim venditaData As Date
                Dim venditaNumero As String
                Dim venditaTipo As Integer
                Dim venditaTipoDesc As String = "'Per conto di' non definito"

                'Ordino il datatable prima per cuaa, poi per la data del documento di vendita. EDIT: L'excel si presenta già ordinato in questo modo
                'Dim dvExcel = dtExcel.DefaultView
                'dvExcel.Sort = "cuaa ASC, data_ddt ASC"
                'dtExcel = dvExcel.ToTable()

                Const COD_GASOLIO As Integer = 2
                Const COD_BENZINA As Integer = 3
                Const COD_GASOLIOSERRA As Integer = 8

                For Each drExcel As DataRow In dtExcel.Rows

                    'So di avere, come prerequisito dell'excel un solo carburante e di un solo tipo per uno o entrambi c/proprio e terzi

                    Dim benzCP = CDbl(drExcel.Field(Of String)("benzina_c_proprio").Trim().Replace(".", ","))
                    Dim gasCP = CDbl(drExcel.Field(Of String)("gasolio_c_proprio").Trim().Replace(".", ","))
                    Dim serCP = CDbl(drExcel.Field(Of String)("gasolio_serra_c_proprio").Trim().Replace(".", ","))

                    Dim carbTotCP = benzCP + gasCP + serCP

                    Dim benzCT = CDbl(drExcel.Field(Of String)("benzina_c_terzi").Trim().Replace(".", ","))
                    Dim gasCT = CDbl(drExcel.Field(Of String)("gasolio_c_terzi").Trim().Replace(".", ","))
                    Dim serCT = CDbl(drExcel.Field(Of String)("gasolio_serra_c_terzi").Trim().Replace(".", ","))

                    Dim carbTotCT = benzCT + gasCT + serCT

                    'Ottengo il tipo di utilizzo del carburante
                    If carbTotCP <> 0 AndAlso carbTotCT <> 0 Then
                        'Devo splittare la riga in due record, per c/proprio e c/terzi
                        incrementoVenCorrente = 2

                        venditaTipo = -999
                        venditaTipoDesc = "C/PROPRIO + C/TERZI"
                    Else
                        incrementoVenCorrente = 1

                        venditaTipo = IIf(carbTotCP = 0, -1, 0)
                        venditaTipoDesc = IIf(venditaTipo = 0, "C/PROPRIO", "C/TERZI")
                    End If

                    numTotaleVendite += incrementoVenCorrente


                    impresaCuaa = drExcel.Field(Of String)("cuaa").Trim()
                    impresaRagSog = drExcel.Field(Of String)("denominazione").Trim()

                    Try
                        impresaPiva = handleImprese.Piva_From_CUAA(impresaCuaa, objParametri_Server)
                    Catch ex As Exception
                        numVenIgnorateAzDupl += incrementoVenCorrente

                        Dim mesImpresaDuplicata = String.Format(
                            " Azienda Cuaa.{0} {1} ignorata perché più imprese su database presentano questo cuaa",
                            impresaCuaa,
                            impresaRagSog)
                        listaErrori.Add(mesImpresaDuplicata)
                        AggiungiLog(NomeRoutine, mesImpresaDuplicata)

                        Continue For
                    End Try

                    If impresaPiva = "" Then
                        numVenIgnorateAzScon += incrementoVenCorrente

                        Dim mesImpresaSconosciuta = String.Format(" Azienda Cuaa.{0} {1} ignorata perché non presente a database", impresaCuaa, impresaRagSog)
                        listaErrori.Add(mesImpresaSconosciuta)
                        AggiungiLog(NomeRoutine, mesImpresaSconosciuta)

                        Continue For
                    End If


                    Dim benzTot = CDbl(drExcel.Field(Of String)("benzina_totale_vendita").Trim().Replace(".", ","))
                    Dim gasTot = CDbl(drExcel.Field(Of String)("gasolio_totale_vendita").Trim().Replace(".", ","))
                    Dim serTot = CDbl(drExcel.Field(Of String)("gasolio_serra_totale_vendita").Trim().Replace(".", ","))

                    'Ottengo qual è il carburante utilizzato
                    Dim carbTipo As Integer
                    If gasTot <> 0 Then
                        carbTipo = COD_GASOLIO
                    ElseIf serTot <> 0 Then
                        carbTipo = COD_GASOLIOSERRA
                    Else
                        carbTipo = COD_BENZINA
                    End If

                    Dim listaInsert As New List(Of UMA_Vendite)

                    venditaData = drExcel.Field(Of DateTime)("data_ddt")
                    venditaNumero = drExcel.Field(Of String)("numero_ddt").Trim()

                    Dim tipoDocExcel = drExcel.Field(Of String)("cod_tipo_doc").Trim()
                    Dim tipoDocDB As Integer
                    Select Case tipoDocExcel
                        Case "001" 'DAS Documento Accompagnatorio Semplificato
                            tipoDocDB = 0

                        Case "002" 'Fattura Accompagnatoria
                            tipoDocDB = 1

                        Case Else
                            tipoDocDB = -1
                    End Select

                    'Nota: Il .Trim() va in errore se l'oggetto è Nothing, ho solo questa colonna attualmente che può essere nulla, quindi non controllo anche le altre
                    Dim noteRivenditore As String = ""
                    If drExcel.Field(Of String)("note_rivenditore") IsNot Nothing Then
                        noteRivenditore = drExcel.Field(Of String)("note_rivenditore").Trim()
                    End If

                    If venditaTipo = -999 Then
                        Dim carbLtCP As Double
                        Select Case carbTipo
                            Case COD_GASOLIO
                                carbLtCP = gasCP
                            Case COD_GASOLIOSERRA
                                carbLtCP = serCP
                            Case Else
                                carbLtCP = benzCP
                        End Select

                        Dim objUmaVenditeCP As New UMA_Vendite With {
                            .PivaSuperUser = objParametri_Server.PivaSuperUser,
                            .PIVA_Venditore = drExcel.Field(Of String)("venditore_piva"),
                            .PIVA_Cliente = impresaPiva,
                            .Anno = drExcel.Field(Of String)("anno").Trim(),
                            .Conto_Proprio_Terzi = 0,
                            .Tipo_Carburante = carbTipo,
                            .Lt = carbLtCP,
                            .Tipo_Documento = tipoDocDB,
                            .Data_Documento = venditaData,
                            .Nr_Documento = venditaNumero,
                            .Note_Rivenditore = noteRivenditore,
                            .Data_Creazione = drExcel.Field(Of DateTime)("data_convalida"),
                            .Data_Modifica = drExcel.Field(Of DateTime)("data_convalida"),
                            .Username_Creazione = objParametri_Server.SuperUserUsername,
                            .Username_Modifica = objParametri_Server.SuperUserUsername
                        }

                        Dim carbLtCT As Double
                        Select Case carbTipo
                            Case COD_GASOLIO
                                carbLtCT = gasCT
                            Case COD_GASOLIOSERRA
                                carbLtCT = serCT
                            Case Else
                                carbLtCT = benzCT
                        End Select

                        Dim objUmaVenditeCT As New UMA_Vendite With {
                            .PivaSuperUser = objParametri_Server.PivaSuperUser,
                            .PIVA_Venditore = drExcel.Field(Of String)("venditore_piva"),
                            .PIVA_Cliente = impresaPiva,
                            .Anno = drExcel.Field(Of String)("anno"),
                            .Conto_Proprio_Terzi = -1,
                            .Tipo_Carburante = carbTipo,
                            .Lt = carbLtCT,
                            .Tipo_Documento = tipoDocDB,
                            .Data_Documento = venditaData,
                            .Nr_Documento = venditaNumero,
                            .Note_Rivenditore = noteRivenditore,
                            .Data_Creazione = drExcel.Field(Of DateTime)("data_convalida"),
                            .Data_Modifica = drExcel.Field(Of DateTime)("data_convalida"),
                            .Username_Creazione = objParametri_Server.SuperUserUsername,
                            .Username_Modifica = objParametri_Server.SuperUserUsername
                        }

                        listaInsert.Add(objUmaVenditeCP)
                        listaInsert.Add(objUmaVenditeCT)

                    Else
                        Dim carbLt As Double
                        Select Case carbTipo
                            Case COD_GASOLIO
                                carbLt = gasTot
                            Case COD_GASOLIOSERRA
                                carbLt = serTot
                            Case Else
                                carbLt = benzTot
                        End Select

                        listaInsert.Add(
                            New UMA_Vendite With {
                                .PivaSuperUser = objParametri_Server.PivaSuperUser,
                                .PIVA_Venditore = drExcel.Field(Of String)("venditore_piva"),
                                .PIVA_Cliente = impresaPiva,
                                .Anno = drExcel.Field(Of String)("anno").Trim(),
                                .Conto_Proprio_Terzi = venditaTipo,
                                .Tipo_Carburante = carbTipo,
                                .Lt = carbLt,
                                .Tipo_Documento = tipoDocDB,
                                .Data_Documento = venditaData,
                                .Nr_Documento = venditaNumero,
                                .Note_Rivenditore = noteRivenditore,
                                .Data_Creazione = drExcel.Field(Of DateTime)("data_convalida"),
                                .Data_Modifica = drExcel.Field(Of DateTime)("data_convalida"),
                                .Username_Creazione = objParametri_Server.SuperUserUsername,
                                .Username_Modifica = objParametri_Server.SuperUserUsername
                            }
                        )

                    End If

                    'Inserisco la singola o le due vendite trovate
                    Dim handleUmaVendite As New UMA_Vendite_W()
                    Dim mesErroreScrittura = handleUmaVendite.Scrivi(listaInsert, objParametri_Server)

                    If mesErroreScrittura = "" Then

                        If venditaTipo = -999 Then
                            Dim mesOk = String.Format(
                                " Azienda Cuaa.{0} Piva.{1} Vendita {2} - {3} ({4})",
                                impresaCuaa,
                                impresaPiva,
                                venditaNumero,
                                venditaData.ToString("d"),
                                "C/PROPRIO")
                            listaOk.Add(mesOk)
                            AggiungiLog(NomeRoutine, mesOk & " scritta")

                            mesOk = String.Format(
                                " Azienda Cuaa.{0} Piva.{1} Vendita {2} - {3} ({4})",
                                impresaCuaa,
                                impresaPiva,
                                venditaNumero,
                                venditaData.ToString("d"),
                                "C/TERZI")
                            listaOk.Add(mesOk)
                            AggiungiLog(NomeRoutine, mesOk & " scritta")

                        Else
                            Dim mesOk = String.Format(
                                " Azienda Cuaa.{0} Piva.{1} Vendita {2} - {3} ({4})",
                                impresaCuaa,
                                impresaPiva,
                                venditaNumero,
                                venditaData.ToString("d"),
                                venditaTipoDesc)
                            listaOk.Add(mesOk)
                            AggiungiLog(NomeRoutine, mesOk & " scritta")
                        End If

                    Else
                        numVenInErrore += incrementoVenCorrente

                        listaErrori.Add(mesErroreScrittura)
                        AggiungiLog(NomeRoutine, mesErroreScrittura)

                        'Dim mesErrore = String.Format(
                        '    " Azienda Cuaa.{0} Piva.{1} Vendita {2}/{3} ({4}): {5}",
                        '    impresaCuaa,
                        '    impresaPiva,
                        '    venditaNumero,
                        '    venditaData,
                        '    venditaTipoDesc,
                        '    mesErroreScrittura)
                        'listaErrori.Add(mesErrore)
                        'AggiungiLog(NomeRoutine, mesErrore)
                    End If

                Next
            End If

            If listaOk.Count > 0 Then
                Dim mesOk = String.Format(
                    "A fronte di un excel con {0} righe sono state trovate {1} vendite. " &
                    "Vendite scritte con successo {2}. " &
                    "Vendite in errore {3}. " &
                    "Ignorate {4} vendite di aziende con cuaa appartenente a più imprese. " &
                    "Ignorate {5} vendite di aziende non presenti a database. ",
                    dtExcel.Rows.Count,
                    numTotaleVendite,
                    listaOk.Count,
                    numVenInErrore,
                    numVenIgnorateAzDupl,
                    numVenIgnorateAzScon
                )
                r.RispostaStringa = mesOk
                AggiungiLog(NomeRoutine, vbCrLf & mesOk)
            End If
            If listaErrori.Count > 0 Then
                r.Errore = Newtonsoft.Json.JsonConvert.SerializeObject(listaErrori)
            End If

            Return r

        Catch ex As Exception
            Throw ex
        Finally
            _logStreamWriter.Close()
        End Try

    End Function

    Public Function AvviaImportazioneMacchine(ByVal pathExcel As String, ByVal directoryLog As String, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim methodInfo = Reflection.MethodBase.GetCurrentMethod()
        Dim NomeRoutine As String = methodInfo.ReflectedType.FullName & "." & methodInfo.Name
        Dim nomeImport = "SIAR_UMA_Macchine"

        Dim r As New RispostaStandard()
        r.RispostaOK = True
        Dim listaOk As New List(Of String)
        Dim listaErrori As New List(Of String)

        Try
            'Operazioni per il log: controllo di avere una cartella, eventualmente creo una sottocartella specifica e dichiaro il nome file per questa importazione
            If directoryLog = "" Then
                directoryLog = "C:\GIASLAN"
            End If
            Dim directoryLogImport = directoryLog & "\Importazione_" & nomeImport & "_Log"
            If Not Directory.Exists(directoryLogImport) Then
                Directory.CreateDirectory(directoryLogImport)
            End If
            _pathFileLog = directoryLogImport & "\" & nomeImport & "__" & Now.ToString("yyyy-MM-dd_(hh.mm.ss)") & ".txt"
            _logDescrizioneUtente = objParametri_Server.LogDescrizioneUtente
            'Inizio a scrivere sul file
            AggiungiLog(NomeRoutine, "Avvio importazione " & pathExcel)

            Dim excelProvider As String
            If Environment.Is64BitProcess Then
                excelProvider = "Microsoft.ACE.OLEDB.12.0"
            Else
                excelProvider = "Microsoft.Jet.OLEDB.4.0"
            End If

            Dim stringaConnExcel = String.Format("PROVIDER={0};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';data source='{1}'", excelProvider, pathExcel)

            Dim dtExcel As New DataTable()
            Dim errCaricamentoDatiExcel As String = ""

            Crea_DtDati_Da_Excel(stringaConnExcel, errCaricamentoDatiExcel, dtExcel)

            AggiungiLog(NomeRoutine, "Completamento lettura dati da excel")

            'Variabili di log
            Dim numMekInErrore As Integer
            Dim numMekIgnorateAzDupl As Integer
            Dim numMekIgnorateAzScon As Integer
            Dim numMekIgnoratePrecIns As Integer
            'fine var. di log

            'L'excel è ordinato per cuaa e datacarico
            If errCaricamentoDatiExcel.Length = 0 AndAlso dtExcel.Rows.Count > 0 Then

                Dim handleReadMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R()
                Dim handleImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read()

                Dim listaMekPerAzienda As New List(Of AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine)

                For Each drExcel As DataRow In dtExcel.Rows

                    Dim impresaPiva As String = ""
                    Dim impresaRagSog As String = "" 'TODO Calcolare con una lettura oppure togliere

                    ' Verifico l'esistenza e l'unicità della impresa corrente
                    Dim impresaCuaa As String = drExcel.Field(Of String)("cuaa").Trim()

                    Try
                        impresaPiva = handleImprese.Piva_From_CUAA(impresaCuaa, objParametri_Server)
                    Catch ex As Exception
                        numMekIgnorateAzDupl += 1

                        Dim mesImpresaDuplicata = String.Format(
                            "Azienda Cuaa.{0} {1} ignorata perché più imprese su database presentano questo cuaa",
                            impresaCuaa,
                            impresaRagSog)
                        listaErrori.Add(mesImpresaDuplicata)
                        AggiungiLog(NomeRoutine, mesImpresaDuplicata)

                        Continue For
                    Finally
                        'Se la lista ha degli elementi e il record attuale appartiene ad una azienda diversa da quella presente nella lista,
                        'allora effettuo l'insert degli elementi attualmente presenti poi svuoto la lista
                        If listaMekPerAzienda.Count > 0 AndAlso listaMekPerAzienda(0).partitaIva <> impresaPiva Then

                            InserisciMacchineDaLista(objParametri_Server, listaMekPerAzienda, listaOk, listaErrori, numMekInErrore)

                        End If
                    End Try

                    If impresaPiva = "" Then
                        numMekIgnorateAzScon += 1

                        Dim mesImpresaSconosciuta = String.Format("Azienda Cuaa.{0} {1} ignorata perché non presente a database", impresaCuaa, impresaRagSog)
                        listaErrori.Add(mesImpresaSconosciuta)
                        AggiungiLog(NomeRoutine, mesImpresaSconosciuta)

                        Continue For
                    End If

                    'Prelevo dati identificativi della macchina e verifico se è già presente su database
                    Dim targa As String = If(drExcel.Field(Of String)("targa"), "").Trim()
                    Dim matricola As String = If(drExcel.Field(Of String)("matricola_macchina"), "").Trim()
                    Dim modello As String = If(drExcel.Field(Of String)("modello_macchina"), "").Trim()

                    'La macchina può essere identificata in modo univoco dalle colonne
                    'cuaa/piva
                    'cod_tipo_macchina
                    Dim siarCodTipo As String = If(drExcel.Field(Of String)("cod_tipo_macchina"), "").Trim()
                    Dim classCod As New AgronicaCoreModelsSTD.metaschema.Macchine
                    'TODO Specificare i valori corretti
                    Select Case siarCodTipo
                        Case "00001"
                            classCod.codice = "15.03"

                        Case "00002"
                            classCod.codice = "16.02"

                        Case "00003"
                            classCod.codice = "15.08"

                        Case "00004"
                            classCod.codice = "09.06.02"

                        Case "00005"
                            classCod.codice = "15"

                        Case "00006"
                            classCod.codice = "15.03"

                        Case "00007"
                            classCod.codice = "15"

                        Case "00008"
                            classCod.codice = "08.01"

                        Case "00009"
                            classCod.codice = "08.01"

                        Case "00010"
                            classCod.codice = "01.04"

                        Case "00011"
                            classCod.codice = "02.05"

                        Case "00012"
                            classCod.codice = "07.01"

                        Case "00013"
                            classCod.codice = "05.04"

                        Case "00014"
                            classCod.codice = "05.04"

                        Case "00015"
                            classCod.codice = "12.01"

                        Case "00016"
                            classCod.codice = "02.02"

                        Case "00017"
                            classCod.codice = "10.01"

                        Case "00018"
                            classCod.codice = "01"

                        Case "00019"
                            classCod.codice = "08.11"

                        Case "00020"
                            classCod.codice = "11.10"

                        Case "00021"
                            classCod.codice = "11.05"

                        Case "00022"
                            classCod.codice = "11.04"

                        Case "00999"
                            classCod.codice = "15"

                        Case Else

                    End Select

                    Dim filtroAggLeggiMacchine = ""

                    Dim dtMacchina = handleReadMacchine.Leggi(
                        impresaPiva,
                        0,
                        False,
                        classCod.codice,
                        targa,
                        "",
                        modello,
                        "",
                        0,
                        "",
                        False,
                        0,
                        "",
                        False,
                        AGRODATAINIZIO,
                        AGRODATAFINE,
                        filtroAggLeggiMacchine,
                        "",
                        objParametri_Server,
                        matricola)

                    If dtMacchina.Rows.Count > 0 Then
                        numMekIgnoratePrecIns += 1
                        Dim mesDuplicata = String.Format("Macchina {0}: {1} ({2} - {3}) ignorata perché già inserita", impresaPiva, modello, targa, matricola)
                        listaErrori.Add(mesDuplicata)
                        AggiungiLog(NomeRoutine, mesDuplicata)
                        Continue For
                    End If

                    'Ricavo gli altri dati della macchina
                    Dim validitaInizio As Date? = If(drExcel.Field(Of Date?)("datacarico"), AGRODATAINIZIO)
                    Dim validitaFine As Date? = If(drExcel.Field(Of Date?)("data_scarico"), AGRODATAFINE)
                    Dim marca As String = If(drExcel.Field(Of String)("marca"), "").Trim()

                    Dim pesoStr As String = drExcel.Field(Of String)("peso")
                    Dim peso As Double = 0
                    If pesoStr IsNot Nothing AndAlso pesoStr.Trim().Length > 0 Then
                        peso = pesoStr.Trim().Replace(".", ",")
                    End If

                    Dim motoreMarca As String = If(drExcel.Field(Of String)("motore"), "").Trim()
                    Dim motoreModello As String = If(drExcel.Field(Of String)("modello_motore"), "").Trim()
                    Dim motoreMatricola As String = If(drExcel.Field(Of String)("matricola_motore"), "").Trim()

                    Dim siarCodPossesso As String = If(drExcel.Field(Of String)("cod_possesso"), "").Trim()
                    Dim titoloPossesso As New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso
                    Select Case siarCodPossesso
                        Case "L"  'Leasing
                            titoloPossesso.codice = 0

                        Case "N"  'A Nolo
                            titoloPossesso.codice = 0

                        Case "P" 'Proprietario
                            titoloPossesso.codice = 1

                        Case "U"  'Utilizzatore
                            titoloPossesso.codice = 0

                        Case Else
                            titoloPossesso.codice = 1

                    End Select

                    Dim siarCodCarburante As String = If(drExcel.Field(Of String)("cod_carburante"), "").Trim()
                    Dim tipoAlimentazione As New AgronicaCoreModelsSTD.metaschema.Carburante
                    Select Case siarCodCarburante
                        Case "A" 'Altro
                            tipoAlimentazione.codice = 0

                        Case "B" 'Benzina
                            tipoAlimentazione.codice = 3

                        Case "G" 'Gasolio
                            tipoAlimentazione.codice = 2

                        Case "O" 'Olio
                            tipoAlimentazione.codice = 0

                        Case "P" 'Gasolio Serra
                            tipoAlimentazione.codice = 8

                        Case "S" 'Senza Carburante
                            tipoAlimentazione.codice = 0

                    End Select


                    Dim dittaVuota As New AgronicaCoreModelsSTD.metaschema.DittaMacchina With {
                        .codice = 0,
                        .descrizione = ""
                    }
                    Dim macchinaDett1 As New AgronicaCoreModelsSTD.metaschema.MacchineDettaglio1 With {
                        .codice = "",
                        .descrizione = ""
                    }
                    Dim macchinaDett2 As New AgronicaCoreModelsSTD.metaschema.MacchineDettaglio2 With {
                        .codice = "",
                        .descrizione = ""
                    }

                    Dim intervalloTemp As New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(validitaInizio.Value, validitaFine.Value)

                    Dim statoUtilizzo As String = ""
                    If validitaFine.Value < AGRODATAFINE Then
                        statoUtilizzo = "Dismesso"
                    End If

                    Dim tipoTarga As New AgronicaCoreModelsSTD.metaschema.TipoTarga With {
                        .codice = 0,
                        .descrizione = ""
                    }
                    Dim unitaMisura As New AgronicaCoreModelsSTD.metaschema.UnitaDiMisura With {
                        .codice = 0,
                        .descrizione = ""
                    }

                    'Attualmente non sono gestiti i seguenti dati:
                    'peso, dati del motore
                    Dim mekStd As New AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine With {
                        .visibilitaPubblica = False,
                        .partitaIva = impresaPiva,
                        .descrizione = marca & " - " & modello,
                        .marca = dittaVuota,
                        .modello = modello,
                        .finalita = Nothing, 'Non usata dalla funzione di scrittura
                        .tipo = classCod,
                        .dettaglio_1 = macchinaDett1,
                        .dettaglio_2 = macchinaDett2,
                        .codice = 0,
                        .codice_stringa = "",
                        .validita = intervalloTemp,
                        .Data_Carico = validitaInizio.Value,
                        .Data_Scarico = validitaFine.Value,
                        .titolo_Possesso = titoloPossesso,
                        .proprietario = "",
                        .CUAA_Proprietario = "",
                        .targa = targa,
                        .tipo_Targa = tipoTarga,
                        .telaio = "",
                        .n_Immatricolazione = matricola,
                        .data_Immatricolazione = AGRODATAINIZIO,
                        .n_Immatricolazione_Rimorchio = "",
                        .N_Autorizzazione_Trasporto = "",
                        .data_Rilascio_Autorizzazione = AGRODATAINIZIO,
                        .alimentazione = tipoAlimentazione,
                        .taratura_Ugello = 0,
                        .data_Ultima_Taratura = AGRODATAINIZIO,
                        .scadenza_Taratura = AGRODATAFINE,
                        .stato_Utilizzo = statoUtilizzo,
                        .potenza = 0,
                        .unita_Misura = unitaMisura,
                        .note = "",
                        .flag_cancellazione = False
                    }

                    listaMekPerAzienda.Add(mekStd)

                Next

                'All'uscita dal foreach occorre inserire le macchine dell'ultima azienda
                If listaMekPerAzienda.Count > 0 Then
                    InserisciMacchineDaLista(objParametri_Server, listaMekPerAzienda, listaOk, listaErrori, numMekInErrore)
                End If

            End If

            If listaOk.Count > 0 Then
                Dim mesOk = String.Format(
                    "L'excel contiene {0} macchine. " &
                    "Macchine scritte con successo {1}. " &
                    "Macchine in errore {2}. " &
                    "Ignorate {3} macchine di aziende con cuaa appartenente a più imprese. " &
                    "Ignorate {4} macchine di aziende non presenti a database. " &
                    "Ignorate {5} macchine già presenti a database. ",
                    dtExcel.Rows.Count,
                    listaOk.Count,
                    numMekInErrore,
                    numMekIgnorateAzDupl,
                    numMekIgnorateAzScon,
                    numMekIgnoratePrecIns
                )
                r.RispostaStringa = mesOk
                AggiungiLog(NomeRoutine, vbCrLf & mesOk)
            End If
            If listaErrori.Count > 0 Then
                r.Errore = Newtonsoft.Json.JsonConvert.SerializeObject(listaErrori)
            End If

            Return r

        Catch ex As Exception
            Throw ex
        Finally
            _logStreamWriter.Close()
        End Try

    End Function

    Private Function InserisciMacchineDaLista(ByRef objParametri_Server As AgronicaCoreParametri,
                                              ByRef listaMacchine As List(Of AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine),
                                              ByRef listaOk As List(Of String),
                                              ByRef listaErrori As List(Of String),
                                              ByRef numMekInErrore As Integer) As Integer

        Dim methodInfo = Reflection.MethodBase.GetCurrentMethod()
        Dim NomeRoutine As String = methodInfo.ReflectedType.FullName & "." & methodInfo.Name

        AggiungiLog(NomeRoutine, String.Format("Inizio inserimento macchine per azienda {0}", listaMacchine(0).partitaIva))

        Dim transactionOptions = New TransactionOptions()
        transactionOptions.IsolationLevel = IsolationLevel.ReadCommitted
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim GiasContext As Gias_DeveloperServer_Entities = Nothing

        Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

            GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.Connection.Open()

            Dim mekLog As New AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine

            Try
                For Each mek In listaMacchine
                    mekLog = mek
                    AgronicaCoreContabDAL.EFMacchine.Macchina_Scrivi_EF(mek,
                                                                        objParametri_Server,
                                                                        objParametri_Server.SuperUserUsername,
                                                                        GiasContext,
                                                                        False)

                    Dim mesOk = String.Format("Macchina {0} ({1} - {2})", mek.modello, mek.targa, mek.n_Immatricolazione)
                    listaOk.Add(mesOk)
                    AggiungiLog(NomeRoutine, mesOk & " inserita")

                Next

                ' COMMIT Effettivo
                scope.Complete()

            Catch ex As Exception
                ' Rollback
                scope.Dispose()

                numMekInErrore += 1
                Dim mesErr = String.Format(
                                        "L'inserimento delle macchine per l'azienda {0} è stato interrotto per il seguente errore sulla macchina {1} ({2} - {3}). Messaggio: {4}",
                                        mekLog.partitaIva,
                                        mekLog.modello,
                                        mekLog.targa,
                                        mekLog.n_Immatricolazione,
                                        Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, " | "))
                listaErrori.Add(mesErr)
                AggiungiLog(NomeRoutine, mesErr)

            Finally
                If GiasContext.Database.Connection.State = ConnectionState.Open Then
                    GiasContext.Database.Connection.Close()
                End If

                AggiungiLog(NomeRoutine, String.Format("Fine inserimento macchine per azienda {0}", listaMacchine(0).partitaIva))

                listaMacchine.Clear()
            End Try

        End Using


        Return 0
    End Function

    Private Sub Crea_DtDati_Da_Excel(ByVal StringaConnessione As String, ByRef Messaggio As String, ByRef Dt As DataTable)

        Try

            Dim ds As New DataSet
            Dim MyConnection As New OleDb.OleDbConnection(StringaConnessione)
            MyConnection.Open()
            Dim dtSheet = MyConnection.GetSchema("Tables")
            Dim firstSheet As String = ""
            For Each sheet In dtSheet.Rows
                If Not sheet("TABLE_NAME").ToString().Contains("Legenda") Then
                    firstSheet = sheet("TABLE_NAME").ToString()
                    Exit For
                End If
            Next

            Dim da As New OleDb.OleDbDataAdapter("select * from [" & firstSheet & "]", MyConnection)
            da.Fill(ds, "fileXls")
            MyConnection.Close()

            Dt = ds.Tables(0)

        Catch ex As Exception
            Messaggio = "Errore all'apertura del file excel: " & ex.Message
        End Try

    End Sub

    Private Sub AggiungiLog(ByVal nomeRoutine As String, ByVal messaggio As String)
        If _pathFileLog <> "" Then
            Dim testo = String.Format("{0} {{{1}}} : [{2}] : {3}", Now.ToString("G"), _logDescrizioneUtente, nomeRoutine, messaggio)

            If _logStreamWriter Is Nothing OrElse _logStreamWriter.BaseStream Is Nothing Then
                _logStreamWriter = File.AppendText(_pathFileLog)
            End If
            _logStreamWriter.WriteLine(testo)
            _logStreamWriter.Flush()
        End If
    End Sub

End Class
