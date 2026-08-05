Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.IO
Imports AgronicaCoreScadenziario
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreVarieDAL

Public Class Alert_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Cancella_Allegati_Contatto(ByVal Piva As String, ByVal Cod_Contatto As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Optional ByVal PATH As String = "") As Boolean

        Dim objDocumenti As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
        Dim allegati As DataTable = objDocumenti.Leggi_Allegati_Entita(Piva, Cod_Contatto, 0, 0, 0, 0, 0, 0, "", "", objParametri)
        Dim esito As Boolean = True

        For Each allegato In allegati.Rows
            If Not Cancella_Allegato(allegato.Item("ID_Elenco"), PATH, objParametri) Then
                esito = False
            End If
        Next

        Return esito

    End Function

    Public Function Cancella_Allegato(ByVal ID_Elenco As Integer,
                                      ByVal PATH As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          Optional fromGestioneAllegati As Boolean = False,
                                          Optional scriviLog As Boolean = True,
                                          Optional noteXlog As String = Nothing,
                                          Optional origine As enum_SistemiEsterni = enum_SistemiEsterni.gias
                                      ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Entita_R.Cancella_Allegato()"

        Dim esito As Boolean
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim ID_Tipologia As Integer = 0
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            'parto con la cancellazione dell'allegato
            Dim objElenco_R As New AgronicaCoreScadenziario.Alert_Elenco_R
            Dim DT_Elenco As DataTable = objElenco_R.Leggi(ID_Elenco, 0, objParametri)
            ID_Tipologia = DT_Elenco(0)("ID_Tipologia")

            'leggo l Entità
            Dim objEntita_R As New AgronicaCoreScadenziario.Alert_Entita_R
            Dim DT_Entita As DataTable = objEntita_R.Leggi(DT_Elenco.Rows(0).Item("ID_Alert_Entita"), objParametri)

            'scrivo il log
            Dim Descrizione_scadenza As String = DT_Elenco.Rows(0).Item("Descrizione_scadenza")
            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim ID_Alert_Log As Integer = objSeq.NuovoId_Tabella("Alert_Log", 0, 2000000000, objParametri)

            Dim objLog As New AgronicaCoreScadenziario.Alert_Log_W
            objLog.Scrivi(ID_Alert_Log, DT_Entita(0)("Piva"), ID_Elenco, ID_Tipologia, Descrizione_scadenza, enum_TipoOperazioneDB.Cancellazione, AGRODATAINIZIO, AGRODATAFINE, objParametri)


            'controllo se ho un allegato associato
            If Not IsDBNull(DT_Entita.Rows(0).Item("Allegati_Documenti_cod")) AndAlso DT_Entita.Rows(0).Item("Allegati_Documenti_cod") <> 0 Then

                'leggo l'allegato 
                Dim objAllegato_R As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
                Dim DT_Allegato As DataTable = objAllegato_R.Leggi(DT_Entita.Rows(0).Item("Allegati_Documenti_cod"),
                                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                    "", "", objParametri)
                'controllo se esiste il file 
                If Not String.IsNullOrEmpty(PATH) Then
                    If Not IsDBNull(DT_Allegato.Rows(0).Item("Sottocartella")) And Not IsDBNull(DT_Allegato.Rows(0).Item("Allegati_documenti_nomefile")) Then

                        Dim sottocartella As String = DT_Allegato.Rows(0).Item("Sottocartella")
                        Dim nomefile As String = DT_Allegato.Rows(0).Item("Allegati_documenti_nomefile")

                        If File.Exists(PATH & sottocartella & "/" & nomefile) Then
                            File.Delete(PATH & sottocartella & "/" & nomefile)
                        End If
                    End If
                End If

                'cancello il record Allegati
                Dim objAllegati_W As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
                objAllegati_W.Cancella(DT_Entita.Rows(0).Item("Allegati_Documenti_cod"), "", objParametri)

            End If

            'cancello il record Elenco
            Dim objELenco_W As New AgronicaCoreScadenziario.Alert_Elenco_W
            objELenco_W.Cancella(ID_Elenco, objParametri)

            'cancello il record Entita
            Dim objEntita_W As New AgronicaCoreScadenziario.Alert_Entita_W
            objEntita_W.Cancella(DT_Elenco.Rows(0).Item("ID_Alert_Entita"), objParametri)

            'ANNULLO LE VECCHIE PROGARMMAZIONI MAIL
            Dim mp_W As New AgronicaCoreMailBIZ.Mail_Programmazione_W
            mp_W.CancellaProgrammazioniFromChiave(objParametri, enum_MailTipo.Scadenze_InScadenza, ID_Elenco)


            If DT_Elenco.Rows(0).Item("ID_Tipologia") = enum_ID_Area_Tipologia.Patentino_trattamenti AndAlso
                    DT_Entita.Rows(0).Item("Cod_Contatto") <> "" AndAlso scriviLog Then
                'Se è andato tutto a buonfine, aggiorno il log dei contatti
                Dim NoteLog As String = String.Empty
                If origine = enum_SistemiEsterni.gias Then
                    NoteLog = "Cancellazione Patentino da " & If(fromGestioneAllegati, "GestioneAllegati.aspx (New_Contatto_Edit)", "")
                Else
                    NoteLog = "Cancellazione Patentino attraverso " & noteXlog
                End If
                Chiama_Scrivi_Log_Contatti(DT_Entita.Rows(0).Item("Piva"), DT_Entita.Rows(0).Item("Cod_Contatto"), NoteLog, objParametri, origine)
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            esito = True

        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            esito = False

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return esito

    End Function

    Public Function Scrivi_Allegato(ByVal ID_Tipologia As Integer,
                                    ByVal objEntita As AgronicaCoreScadenziario.Alert_Entita,
                                    ByVal Num_Documento As String,
                                    ByVal Ente_Rilascio As String,
                                    ByVal Data_Rilascio As Date,
                                    ByVal Data_Scadenza As Date,
                                    ByVal Descrizione_scadenza As String,
                                    ByVal Percorso As String,
                                    ByVal Nome_Allegato As String,
                                    ByVal File_Allegato As String,
                                    ByVal Validita_inizio As Date,
                                    ByVal Validita_fine As Date,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    ByVal note As String,
                                    ByRef ret_ID_Elenco As Integer,
                                    ByVal Validazione_Flag As Integer,
                                    ByVal Username_Upload As String,
                                    ByVal Data_Upload As Date,
                                        Optional fromGestioneAllegati As Boolean = False,
                                        Optional isUpdate As Boolean = False,
                                        Optional ByVal noteXLog As String = "",
                                        Optional ByVal origine As Integer = enum_SistemiEsterni.gias
                                    ) As String


        Const nomeRoutine = "AgronicaCoreScadenziario_BIZ.ALERT.Scrivi_Allegati()"

        Dim strErr As String = ""
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean

        Try

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim objScriviElenco As New AgronicaCoreScadenziario.Alert_Elenco_W
            Dim objScriviEntita As New AgronicaCoreScadenziario.Alert_Entita_W

            Dim Base As Integer = 0
            Dim Top As Integer = 2000000000
            objEntita.PivaSuperUser = objParametri.PivaSuperUser

            Dim objTipo As New AgronicaCoreScadenziario.Alert_Tipologia_R

            Dim fileByteArray_Test As Byte()

            Dim dtTipologia As DataTable = objTipo.Leggi_CatCod(ID_Tipologia, objParametri)
            Dim pathDestinazione As String = Percorso & dtTipologia.Rows(0).Item("Sottocartella") & "\"
            Dim nomeTipologia As String = dtTipologia.Rows(0).Item("nome")
            Dim CatCod As String = dtTipologia.Rows(0).Item("Cat_Cod")
            Dim Sottocartella As String = ""
            If Not IsDBNull(dtTipologia.Rows(0).Item("Sottocartella")) Then
                Sottocartella = dtTipologia.Rows(0).Item("Sottocartella")
            End If

            Dim fileByteArray As Byte()

            If File_Allegato <> "" Then
                ' scrivo file allegato
                'If Not String.IsNullOrEmpty(Nome_Allegato) AndAlso Not String.IsNullOrEmpty(File_Allegato) Then
                fileByteArray = Convert.FromBase64String(File_Allegato)
                If Not Directory.Exists(pathDestinazione) Then
                    Directory.CreateDirectory(pathDestinazione)
                End If

                'Impostazione File_Name Univoco
                Dim ObjHelper As New FileSystemHelper
                Nome_Allegato = ObjHelper.NomeFileUnivoco(Nome_Allegato, True)
                Dim File_Name = pathDestinazione & Nome_Allegato

                IO.File.WriteAllBytes(File_Name, fileByteArray)

                'Controllo Effettiva Scrittura File
                fileByteArray_Test = My.Computer.FileSystem.ReadAllBytes(File_Name)

                If Not fileByteArray_Test.SequenceEqual(fileByteArray) Then
                    Throw New Exception("Il file " & Nome_Allegato & "non è stato salvato correttamente")
                End If
            End If

            ' creo il documento allegato
            Dim Allegati_Documenti_Cod As Integer
            Dim objDoc As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
            objDoc.Scrivi(objEntita.Piva, nomeTipologia, CatCod, Nome_Allegato, Num_Documento, Nothing, Sottocartella, Validita_inizio, Validita_fine, Allegati_Documenti_Cod, objParametri,
                      Validazione_Data:=Data_Rilascio,
                      Allegati_Documenti_Ente_Des:=Ente_Rilascio,
                      Validazione_Flag:=Validazione_Flag,
                      Username_Upload:=Username_Upload,
                      Data_Upload:=Data_Upload,
                      File_Allegato_DB:=fileByteArray)

            objEntita.Allegati_Documenti_Cod = Allegati_Documenti_Cod




            ' scrivo alert entita
            objEntita.ID_Alert_Entita = objSeq.NuovoId_Tabella("Alert_Entita", Base, Top, objParametri)
            objScriviEntita.Scrivi(objEntita, Validita_inizio, Validita_fine, objParametri)

            ' scrivo alert elenco
            Dim id_Elenco As Integer = objSeq.NuovoId_Tabella("Alert_Elenco", Base, Top, objParametri)
            objScriviElenco.Scrivi(id_Elenco, ID_Tipologia, objEntita.ID_Alert_Entita, Data_Scadenza, Descrizione_scadenza, False,
                                   Validita_inizio, Validita_fine, objParametri, note)

            Dim objLog As New AgronicaCoreScadenziario.Alert_Log_W
            Dim ID_Alert_Log As Integer = objSeq.NuovoId_Tabella("Alert_Log", 0, 2000000000, objParametri)
            objLog.Scrivi(ID_Alert_Log, objEntita.Piva, id_Elenco, ID_Tipologia, Descrizione_scadenza, enum_TipoOperazioneDB.Scrittura, AGRODATAINIZIO, AGRODATAFINE, objParametri)

            'PROGRAMMO L'INVIO DELLA MAIL
            programmaMailPerEvento(objParametri, id_Elenco, objEntita.ID_Alert_Entita, ID_Tipologia, objEntita, Data_Scadenza, Descrizione_scadenza, note)

            If ID_Tipologia = enum_ID_Area_Tipologia.Patentino_trattamenti AndAlso
                    objEntita.Cod_Contatto <> "" Then
                'Se è andato tutto a buonfine, aggiorno il log dei contatti
                If origine = enum_SistemiEsterni.gias Then
                    noteXLog = If(isUpdate, "Modifica", "Inserimento") & " Patentino da " & If(fromGestioneAllegati, "GestioneAllegati.aspx (New_Contatto_Edit)", "")
                Else
                    noteXLog = If(isUpdate, "Modifica", "Inserimento") & " Patentino attraverso " & noteXLog
                End If
                Chiama_Scrivi_Log_Contatti(objEntita.Piva, objEntita.Cod_Contatto, noteXLog, objParametri, origine)
            End If

            ret_ID_Elenco = id_Elenco

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception

            If Not objParametri.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            strErr = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, strErr)

        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return strErr

    End Function

    Public Function Scrivi(ByVal ID_Tipologia As Integer,
                           ByVal objEntita As AgronicaCoreScadenziario.Alert_Entita,
                           ByVal Data_Scadenza As Date,
                           ByVal Descrizione_scadenza As String,
                           ByVal Nome_Allegato As String,
                           ByVal PATH As String,
                           ByVal Validita_inizio As Date,
                           ByVal Validita_fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           ByVal note As String,
                           ByRef ret_ID_Elenco As Integer,
                            Optional ByVal EntitaxIndici As JArray = Nothing,
                            Optional ByVal Ente_Des As String = "",
                            Optional ByVal Validazione_Flag As Integer = 0,
                            Optional ByVal Username_Upload As String = "",
                            Optional ByVal Data_Upload As DateTime = #2/1/1900#,
                            Optional ByVal File_Allegato As String = "",
                            Optional ByVal Allegati_Documenti_Numero As String = "",
                            Optional ByVal SalvaAllegato As Integer = 0,
                            Optional ByVal ID_App As String = "",
                            Optional ByVal FileByteArrayQDC As Byte() = Nothing,
                            Optional ByVal ID_Area As Integer = 0,
                            Optional ByVal CompressoDaGIAS As Boolean = False,
                            Optional ByRef newAllegati_Documenti_Cod As List(Of String) = Nothing,
                            Optional ByVal Piva As String = "",
                            Optional ByVal WorkFlow_Documentale As String = "",
                            Optional ByVal Data_Rilascio As Date = AGRODATAFINE,
                            Optional ByVal Gestisci_FileAllegato As Boolean = True,
                            Optional ByVal Note_Log As String = "Modulo Documentale",
                            Optional ByVal Origine As Integer = enum_SistemiEsterni.gias,
                            Optional ByVal Allegati_Documenti_Data As Date = AGRODATAINIZIO
                           ) As String


        Const nomeRoutine = "AgronicaCoreScadenziario_BIZ.ALERT.Scrivi()"

        Dim strErr As String = ""
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Try

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim objScriviElenco As New AgronicaCoreScadenziario.Alert_Elenco_W
            Dim objScriviEntita As New AgronicaCoreScadenziario.Alert_Entita_W
            Dim objScriviEntitaxIndici As New AgronicaCoreScadenziario.Alert_Indice_W

            ' forzati
            Dim Base As Integer = 0
            Dim Top As Integer = 2000000000

            objEntita.PivaSuperUser = objParametri.PivaSuperUser

            Dim nomeTipologia As String = ""
            Dim CatCod As String = 0
            Dim Sottocartella As String = ""
            Dim pathDestinazione As String = ""
            Dim fileByteArray As Byte()
            Dim fileByteArray_Test As Byte()
            Dim fileByteArray_Test_BK As Byte()
            Dim ArrayEst() As String
            Dim Allegati_Documenti_Estensione As String = ""

            Dim Flag_Salvataggio_su_FS_Completato As Boolean = False

            Dim id_Elenco As Integer = objSeq.NuovoId_Tabella("Alert_Elenco", Base, Top, objParametri)

            'Lettura Categoria Documento
            Dim objTipo As New AgronicaCoreScadenziario.Alert_Tipologia_R
            Dim dtTipologia As DataTable = objTipo.Leggi_CatCod(ID_Tipologia, objParametri)

            'Controllo Esistenza Sottocartella
            If dtTipologia.Rows.Count > 0 Then

                nomeTipologia = dtTipologia.Rows(0).Item("nome")
                CatCod = dtTipologia.Rows(0).Item("Cat_Cod")

                If Not IsDBNull(dtTipologia.Rows(0).Item("Sottocartella")) Then
                    Sottocartella = dtTipologia.Rows(0).Item("Sottocartella")
                End If

            End If

            If (Not IsNothing(Nome_Allegato) AndAlso Nome_Allegato <> "") OrElse Not Gestisci_FileAllegato Then

                If Gestisci_FileAllegato Then
                    'Determino Estensione                
                    ArrayEst = Split(Nome_Allegato, ".")
                    If UBound(ArrayEst) > 0 Then
                        Allegati_Documenti_Estensione = ArrayEst(UBound(ArrayEst))
                    End If


                    If Not String.IsNullOrEmpty(File_Allegato) Or Not IsNothing(FileByteArrayQDC) Then

                        ''Creazione ByteArray
                        'fileByteArray = Convert.FromBase64String(File_Allegato)


                        'Creazione ByteArray
                        If Not IsNothing(FileByteArrayQDC) Then

                            fileByteArray = FileByteArrayQDC
                        Else

                            fileByteArray = Convert.FromBase64String(File_Allegato)

                        End If


                        'Path Base
                        pathDestinazione = FileSystemHelper.AggiungiSlashSeNonEsiste(PATH)

                        If Trim(Sottocartella) <> "" Then
                            pathDestinazione = FileSystemHelper.AggiungiSlashSeNonEsiste(PATH) & FileSystemHelper.AggiungiSlashSeNonEsiste(Sottocartella)
                        End If

                        Dim File_Name As String
                        File_Name = FileSystemHelper.AggiungiSlashSeNonEsiste(pathDestinazione) & Nome_Allegato

                        'Controllo Salvataggio Su FS
                        If SalvaAllegato = 0 Then

                            If Not Directory.Exists(pathDestinazione) Then
                                Directory.CreateDirectory(pathDestinazione)
                            End If

                            Dim BK_File_Name As String = String.Empty

                            If Not String.IsNullOrEmpty(Allegati_Documenti_Estensione) Then

                                Nome_Allegato = Nome_Allegato.Replace(".", "")

                                Dim index As Integer = Nome_Allegato.LastIndexOf(Allegati_Documenti_Estensione)

                                Nome_Allegato = Nome_Allegato.Insert(index, ".")

                            End If

                            'Impostazione File_Name Univoco
                            Dim ObjHelper As New FileSystemHelper
                            Nome_Allegato = ObjHelper.NomeFileUnivoco(Nome_Allegato, True, True)
                            File_Name = pathDestinazione & Nome_Allegato

                            IO.File.WriteAllBytes(File_Name, fileByteArray)

                            'Controllo Effettiva Scrittura File
                            fileByteArray_Test = My.Computer.FileSystem.ReadAllBytes(File_Name)

                            If Not fileByteArray_Test.SequenceEqual(fileByteArray) Then
                                Flag_Salvataggio_su_FS_Completato = False
                                Throw New Exception("Il file " & Nome_Allegato & "non è stato salvato correttamente")
                            Else
                                Flag_Salvataggio_su_FS_Completato = True
                            End If

                        End If

                    Else

                        Throw New Exception("Impossibile salvare l'Allegato " & Nome_Allegato)

                    End If

                End If


                'creo l'entita e il documento associato
                Dim Allegati_Documenti_Cod = 0
                Dim objDoc As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
                Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_W
                Dim objPraticheW As New AgronicaCoreProfilazioneDAL.Pratiche_W
                Dim Servizio_Cod As Integer = 0
                Dim pratica_Cod As Integer = 0
                'gestione workflow documentale Giacomo C. aprile 2023
                If WorkFlow_Documentale.Length > 0 Then
                    Dim workFlows = WorkFlow_Documentale.Split("|")
                    Dim area_Servizio = workFlows.Where(Function(x) x.Split("_").First.Equals(CStr(ID_Area)))
                    If area_Servizio.Count > 0 Then
                        Servizio_Cod = CInt(area_Servizio.First.Split("_").Last)
                    Else
                        Servizio_Cod = CInt(workFlows.Last.Split("_").Last)
                    End If
                End If

                If Servizio_Cod > 0 Then

                    Dim r = objPratiche.GeneraPratica(Piva,
                                      Servizio_Cod,
                                      "",
                                      "",
                                      Date.Now.ToString,
                                      "",
                                      objParametri,
                                      New AgronicaCoreParametri,
                                      ControllaEsistenzaPratica:=False,
                                      pratica_Cod,
                                      -1)

                End If

                objDoc.Scrivi(objEntita.Piva, nomeTipologia, CatCod, Nome_Allegato, Allegati_Documenti_Numero, Nothing, Sottocartella,
                              Validita_inizio, Validita_fine, Allegati_Documenti_Cod, objParametri, Data_Rilascio,,,,,,, Ente_Des, Validazione_Flag,
                              Username_Upload, Data_Upload, fileByteArray, SalvaAllegato, Allegati_Documenti_Estensione,
                              CompressoDaGIAS:=CompressoDaGIAS, pratica_Cod, Allegati_Documenti_Data:=Allegati_Documenti_Data)

                objEntita.Allegati_Documenti_Cod = Allegati_Documenti_Cod

                objPraticheW.AggiornaNumero(pratica_Cod, "Allegato " & Allegati_Documenti_Cod, "", objParametri)

                'scrivo l alert entita
                objEntita.ID_Alert_Entita = objSeq.NuovoId_Tabella("Alert_Entita", Base, Top, objParametri)
                objScriviEntita.Scrivi(objEntita, Validita_inizio, Validita_fine, objParametri)


            Else

                'Anche se non ho allegati, salvo l'entità
                objEntita.ID_Alert_Entita = objSeq.NuovoId_Tabella("Alert_Entita", Base, Top, objParametri)

                'scrivo l'alert entita
                objScriviEntita.Scrivi(objEntita, Validita_inizio, Validita_fine, objParametri)

            End If



            'Aggiungo un item nella tabella Elenco 
            objScriviElenco.Scrivi(id_Elenco, ID_Tipologia, objEntita.ID_Alert_Entita, Data_Scadenza, Descrizione_scadenza, False,
                                   Validita_inizio, Validita_fine, objParametri, note, ID_App)

            Dim objLog As New AgronicaCoreScadenziario.Alert_Log_W
            Dim ID_Alert_Log As Integer = objSeq.NuovoId_Tabella("Alert_Log", 0, 2000000000, objParametri)
            objLog.Scrivi(ID_Alert_Log, objEntita.Piva, id_Elenco, ID_Tipologia, Descrizione_scadenza,
                          enum_TipoOperazioneDB.Scrittura, AGRODATAINIZIO, AGRODATAFINE, objParametri)

            'PROGRAMMO L'INVIO DELLA MAIL
            programmaMailPerEvento(objParametri, id_Elenco, objEntita.ID_Alert_Entita, ID_Tipologia, objEntita, Data_Scadenza, Descrizione_scadenza, note)

            'GESTIONE INDICI
            If Not IsNothing(EntitaxIndici) Then

                'Cancellazione Preventiva
                objScriviEntitaxIndici.CancellaEntitaxIndice(objEntita.PivaSuperUser, objEntita.ID_Alert_Entita, objParametri)

                For Each obj As JObject In EntitaxIndici

                    objScriviEntitaxIndici.ScriviEntitaxIndice(objEntita.PivaSuperUser, objEntita.ID_Alert_Entita, obj("ID_Indice"), obj("ID_Indice_Det"), obj("Elenco_Val"), obj("Valore_Des"), AGRODATAINIZIO, AGRODATAFINE, objParametri)

                Next

            End If

            If newAllegati_Documenti_Cod IsNot Nothing Then
                newAllegati_Documenti_Cod.Add(objEntita.Allegati_Documenti_Cod)
            End If

            If ID_Tipologia = enum_ID_Area_Tipologia.Patentino_trattamenti AndAlso
                    objEntita.Cod_Contatto <> "" Then
                'Se è andato tutto a buonfine, aggiorno il log dei contatti
                Note_Log = "Inserimento Patentino attraverso " & Note_Log

                Chiama_Scrivi_Log_Contatti(objEntita.Piva, objEntita.Cod_Contatto, Note_Log, objParametri, Origine)
            End If

            ret_ID_Elenco = id_Elenco

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            strErr = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, strErr)

        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return strErr

    End Function

    Public Function Modifica(ByVal ID_Tipologia As Integer,
                             ByVal objEntita As Alert_Entita,
                             ByVal ID_Elenco As Integer,
                             ByVal ID_Alert_Entita As Integer,
                             ByVal Allegati_Documenti_Cod As Integer,
                             ByVal Data_Scadenza As Date,
                             ByVal Descrizione_scadenza As String,
                             ByVal Nome_Allegato As String,
                             ByVal PATH As String,
                             ByVal Validita_inizio As Date,
                             ByVal Validita_fine As Date,
                             ByRef objParametri As AgronicaCoreParametri,
                             ByVal note As String,
                             ByVal EntitaxIndici As JArray,
                             ByVal Validazione_Flag As Integer,
                             ByVal Username_Upload As String,
                             ByVal Data_Upload As DateTime,
                             ByVal File_Allegato As String,
                             ByVal bAllegato_Modificato As Boolean,
                             ByVal Allegati_Documenti_numero As String,
                             ByVal SalvaAllegato As Integer,
                             Optional ByVal FileByteArrayQDC As Byte() = Nothing,
                             Optional ByVal ID_Area As Integer = 0,
                             Optional ByVal CompressoDaGIAS As Boolean = False,
                             Optional ByRef newAllegati_Documenti_Cod As List(Of String) = Nothing,
                             Optional ByVal Data_Rilascio As Date = AGRODATAFINE,
                             Optional ByVal Ente_Rilascio As String = "",
                             Optional ByVal Gestisci_FileAllegato As Boolean = True,
                             Optional ByVal Note_Log As String = "Modulo Documentale",
                             Optional ByVal Origine As Integer = enum_SistemiEsterni.gias,
                             Optional ByVal Allegati_Documenti_Data As Date = AGRODATAINIZIO
                             ) As String

        Const nomeRoutine = "AgronicaCoreScadenziario_BIZ.ALERT.Modifica()"

        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean

        Try

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim objScriviElenco As New AgronicaCoreScadenziario.Alert_Elenco_W
            Dim objScriviEntita As New AgronicaCoreScadenziario.Alert_Entita_W
            Dim objScriviEntitaxIndici As New AgronicaCoreScadenziario.Alert_Indice_W
            Dim objDoc As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
            Dim objDoc_R As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R


            ' forzati
            Dim Base As Integer = 0
            Dim Top As Integer = 2000000000


            Dim nomeTipologia As String = ""
            Dim CatCod As String = 0
            Dim Sottocartella As String = ""
            Dim pathDestinazione As String = ""
            Dim fileByteArray As Byte()
            Dim ArrayEst() As String
            Dim Allegati_Documenti_Estensione As String = ""
            Dim fileByteArray_Test As Byte()
            Dim fileByteArray_Test_BK As Byte()

            Dim Flag_Salvataggio_su_FS_Completato As Boolean = False

            'Lettura Categoria Documento
            Dim objTipo As New AgronicaCoreScadenziario.Alert_Tipologia_R
            Dim dtTipologia As DataTable = objTipo.Leggi_CatCod(ID_Tipologia, objParametri)

            'Controllo Esistenza Sottocartella
            If dtTipologia.Rows.Count > 0 Then

                nomeTipologia = dtTipologia.Rows(0).Item("nome")
                CatCod = dtTipologia.Rows(0).Item("Cat_Cod")

                If Not IsDBNull(dtTipologia.Rows(0).Item("Sottocartella")) Then
                    Sottocartella = dtTipologia.Rows(0).Item("Sottocartella")
                End If

            End If

            If (Not IsNothing(Nome_Allegato) AndAlso Nome_Allegato <> "" And bAllegato_Modificato) OrElse Not Gestisci_FileAllegato Then

                If Gestisci_FileAllegato Then
                    'Determino Estensione                
                    ArrayEst = Split(Nome_Allegato, ".")
                    If UBound(ArrayEst) > 0 Then
                        Allegati_Documenti_Estensione = ArrayEst(UBound(ArrayEst))
                    End If

                    If Not String.IsNullOrEmpty(File_Allegato) Or Not IsNothing(FileByteArrayQDC) Then

                        'Creazione ByteArray
                        If Not IsNothing(FileByteArrayQDC) Then

                            fileByteArray = FileByteArrayQDC
                        Else

                            fileByteArray = Convert.FromBase64String(File_Allegato)

                        End If

                        'Path Base
                        pathDestinazione = FileSystemHelper.AggiungiSlashSeNonEsiste(PATH)

                        If Trim(Sottocartella) <> "" Then
                            pathDestinazione = FileSystemHelper.AggiungiSlashSeNonEsiste(PATH) & FileSystemHelper.AggiungiSlashSeNonEsiste(Sottocartella)
                        End If

                        If SalvaAllegato = 0 Then

                            If Not Directory.Exists(pathDestinazione) Then
                                Directory.CreateDirectory(pathDestinazione)
                            End If

                            Dim BK_File_Name As String = String.Empty

                            If Not String.IsNullOrEmpty(Allegati_Documenti_Estensione) Then

                                Nome_Allegato = Nome_Allegato.Replace(".", "")

                                Dim index As Integer = Nome_Allegato.LastIndexOf(Allegati_Documenti_Estensione)

                                Nome_Allegato = Nome_Allegato.Insert(index, ".")

                            End If

                            'Impostazione File_Name Univoco
                            Dim ObjHelper As New FileSystemHelper
                            Nome_Allegato = ObjHelper.NomeFileUnivoco(Nome_Allegato, True, True)
                            Dim File_Name = FileSystemHelper.AggiungiSlashSeNonEsiste(pathDestinazione) & Nome_Allegato

                            IO.File.WriteAllBytes(File_Name, fileByteArray)


                            'Controllo Effettiva Scrittura File
                            fileByteArray_Test = My.Computer.FileSystem.ReadAllBytes(File_Name)

                            If Not fileByteArray_Test.SequenceEqual(fileByteArray) Then
                                Flag_Salvataggio_su_FS_Completato = False
                                Throw New Exception("Il file " & Nome_Allegato & "non è stato salvato correttamente")
                            Else
                                Flag_Salvataggio_su_FS_Completato = True
                            End If

                        End If

                    Else

                        Throw New Exception("Impossibile salvare l'Allegato " & Nome_Allegato)

                    End If

                End If

            End If


            If (Not IsNothing(Nome_Allegato) AndAlso Nome_Allegato <> "") OrElse Not Gestisci_FileAllegato Then

                Select Case Allegati_Documenti_Cod

                    Case 0 'Inserimento Allegato da Modifica

                        objDoc.Scrivi(objEntita.Piva,
                                      Nome_Allegato,
                                      CatCod,
                                      Nome_Allegato,
                                      Allegati_Documenti_numero,
                                      Nothing,
                                      Sottocartella,
                                      Validita_inizio,
                                      Validita_fine,
                                      Allegati_Documenti_Cod,
                                      objParametri,
                                      Data_Rilascio,,,,,,,
                                      Ente_Rilascio,
                                      Validazione_Flag,
                                      Username_Upload,
                                      Data_Upload,
                                      fileByteArray,
                                      SalvaAllegato,
                                      Allegati_Documenti_Estensione,
                                      CompressoDaGIAS:=CompressoDaGIAS,
                                      Allegati_Documenti_Data:=Allegati_Documenti_Data)

                    Case Else

                        'Modifica Standard 
                        objDoc.Modifica(objEntita.Piva,
                                        Nome_Allegato,
                                        Nome_Allegato,
                                        Allegati_Documenti_numero,
                                        Sottocartella,
                                        Validita_inizio,
                                        Validita_fine,
                                        Allegati_Documenti_Cod,
                                        objParametri,
                                        Data_Rilascio,,,,,
                                        Ente_Rilascio,
                                        Validazione_Flag,
                                        Username_Upload,
                                        Data_Upload,
                                        fileByteArray,
                                        bAllegato_Modificato,
                                        SalvaAllegato,
                                        Allegati_Documenti_Estensione,
                                        CompressoDaGIAS:=CompressoDaGIAS,
                                        Allegati_Documenti_Data:=Allegati_Documenti_Data)

                End Select


            Else

                'Nota: ho bisogno di fare una lettura per verificare che il documento sia effettivamente esistente, poiché la gestione dei patentini inserisce il record anche senza documento (per gestire ente)
                If Allegati_Documenti_Cod <> 0 Then


                    Dim DT As DataTable = objDoc_R.Leggi(Allegati_Documenti_Cod, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

                    If DT.Rows.Count > 0 Then

                        If CStr(DT(0)("Allegati_Documenti_Nomefile")) <> "" Then

                            'Rimozione Allegato
                            objDoc.Cancella(Allegati_Documenti_Cod, "", objParametri)
                            Allegati_Documenti_Cod = 0

                        End If


                    End If

                End If



            End If


            'modifica l'alert entita
            objScriviEntita.Modifica(ID_Alert_Entita, Allegati_Documenti_Cod, objEntita, objParametri)


            'Modifico l'item nella tabella Elenco 
            objScriviElenco.Modifica(ID_Elenco, ID_Tipologia, Data_Scadenza, Descrizione_scadenza, note, objParametri)

            'Scrivo il log su tabella
            Dim objLog As New AgronicaCoreScadenziario.Alert_Log_W
            Dim ID_Alert_Log As Integer = objSeq.NuovoId_Tabella("Alert_Log", 0, 2000000000, objParametri)
            objLog.Scrivi(ID_Alert_Log, objEntita.Piva, ID_Elenco, ID_Tipologia, Descrizione_scadenza,
                          enum_TipoOperazioneDB.Modifica, AGRODATAINIZIO, AGRODATAFINE, objParametri)

            'objLog.Scrivi(ID_Alert_Log, objEntita.Piva, ID_Elenco, ID_Tipologia, "EDIT: " & Descrizione_scadenza & "_Nome_Allegato_" & Nome_Allegato & "_ID_Elenco_" & ID_Elenco & "_Flag_Salvataggio_su_FS_Completato_" & Flag_Salvataggio_su_FS_Completato,
            '              enum_TipoOperazioneDB.Modifica, AGRODATAINIZIO, AGRODATAFINE, objParametri)

            'ANNULLO LE VECCHIE PROGRAMMAZIONI MAIL
            Dim mp_W As New AgronicaCoreMailBIZ.Mail_Programmazione_W
            mp_W.CancellaProgrammazioniFromChiave(objParametri, enum_MailTipo.Scadenze_InScadenza, ID_Elenco)


            'PROGRAMMO L'INVIO DELLA MAIL
            programmaMailPerEvento(objParametri, ID_Elenco, ID_Alert_Entita, ID_Tipologia, objEntita, Data_Scadenza, Descrizione_scadenza, note)

            'GESTIONE INDICI
            If Not IsNothing(EntitaxIndici) Then

                'Cancellazione Preventiva
                objScriviEntitaxIndici.CancellaEntitaxIndice(objEntita.PivaSuperUser, ID_Alert_Entita, objParametri)

                For Each obj As JObject In EntitaxIndici

                    objScriviEntitaxIndici.ScriviEntitaxIndice(objEntita.PivaSuperUser, ID_Alert_Entita, obj("ID_Indice"), obj("ID_Indice_Det"), obj("Elenco_Val"), obj("Valore_Des"), AGRODATAINIZIO, AGRODATAFINE, objParametri)

                Next

            End If

            If newAllegati_Documenti_Cod IsNot Nothing Then
                newAllegati_Documenti_Cod.Add(objEntita.Allegati_Documenti_Cod)
            End If

            If ID_Tipologia = enum_ID_Area_Tipologia.Patentino_trattamenti AndAlso
                    objEntita.Cod_Contatto <> "" Then
                'Se è andato tutto a buonfine, aggiorno il log dei contatti
                Note_Log = "Modifica Patentino attraverso " & Note_Log
                Chiama_Scrivi_Log_Contatti(objEntita.Piva, objEntita.Cod_Contatto, Note_Log, objParametri, Origine)
            End If

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return messaggioErrore

    End Function

    Public Function ModificaOld(ByVal ID_Tipologia As Integer,
                           ByVal id_elenco As Integer,
                           ByVal Data_Scadenza As Date,
                           ByVal Descrizione_scadenza As String,
                           ByVal Validita_inizio As Date,
                           ByVal Validita_fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           ByVal note As String,
                           ByVal Id_Alert_Entita As Integer,
                           Optional ByVal EntitaxIndici As JArray = Nothing
                           ) As String


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_BIZ.ALERT.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder

        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                 FlagTransazioneLocale,
                                                                                 objParametri)

            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim objScriviElenco As New AgronicaCoreScadenziario.Alert_Elenco_W
            Dim objScriviEntita As New AgronicaCoreScadenziario.Alert_Entita_W
            Dim objScriviEntitaxIndici As New AgronicaCoreScadenziario.Alert_Indice_W

            ' forzati
            Dim Base, Top As Integer
            Base = 0
            Top = 2000000000


            'objEntita.PivaSuperUser = objParametri.PivaSuperUser


            'aggiungo l'eventuale allegato
            'parte degli allegati
            'Dim name As String
            'If Not IsNothing(Nome_Allegato) AndAlso Nome_Allegato <> "" Then
            'name = Nome_Allegato

            'Dim pathSorgente As String = PATH & "TEMP\"

            'Dim dtTipologia As DataTable
            'Dim objTipo As New AgronicaCoreScadenziario.Alert_Tipologia_R
            'dtTipologia = objTipo.Leggi_CatCod(ID_Tipologia, objParametri)


            'Dim pathDestinazione As String = PATH & dtTipologia.Rows(0).Item("Sottocartella") & "\"
            'Dim nome_destinazione As String = ""

            'If name.Contains(pathDestinazione) Then
            '    ' il file è già stato salvato nella cartella corretta
            '    ' quindi recupero solo il nome del file
            '    nome_destinazione = name.Replace(pathDestinazione, "")
            'Else
            '    If Not Directory.Exists(pathDestinazione) = True Then
            '        Directory.CreateDirectory(pathDestinazione)
            '    End If
            '    'controllo se esiste già

            '    Dim k As Integer
            '    For k = 1 To name.Split("_").Length - 1
            '        If nome_destinazione = "" Then
            '            nome_destinazione = name.Split("_")(k)
            '        Else
            '            nome_destinazione = nome_destinazione & "_" & name.Split("_")(k)
            '        End If
            '    Next

            '    If File.Exists(pathDestinazione & nome_destinazione) Then
            '        nome_destinazione = name
            '    End If
            '    File.Copy(pathSorgente & name, pathDestinazione & nome_destinazione)
            '    File.Delete(pathSorgente & name)

            'End If

            ' è possibile che le tabelle alert_entita e allegati documenti siano già stati scritti
            ' come ad esempio per le stampe. Quindi non vado a scrivere altri record, ma uso quelli e la nuova scadenza la lego a quelli

            'carico il nome della tipologia
            'Dim nomeTipologia As String = dtTipologia.Rows(0).Item("nome")
            'Dim CatCod As String = dtTipologia.Rows(0).Item("Cat_Cod")
            'Dim Allegati_Documenti_Cod As Integer

            'Dim strFiltro As String = " Allegati_Documenti_NomeFile='" & nome_destinazione & "' " &
            '                          " AND Sottocartella ='" & dtTipologia.Rows(0).Item("Sottocartella") & "' " &
            '                          " AND Allegati_Documenti_CatCod = " & CatCod

            'Dim dtDoc As DataTable
            'Dim objLeggiEntita As New AgronicaCoreScadenziario.Alert_Entita_R
            'dtDoc = objLeggiEntita.Leggi_con_documenti(0, strFiltro, "", objParametri)


            'If dtDoc.Rows.Count > 0 Then

            '    objEntita.Allegati_Documenti_Cod = dtDoc.Rows(0).Item("Allegati_Documenti_Cod")

            '    objEntita.ID_Alert_Entita = dtDoc.Rows(0).Item("Id_Alert_Entita")

            'Else

            '    'creo l'entita e il documento associato
            '    Dim objDoc As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
            '    objDoc.Scrivi(objEntita.Piva,
            '                   nomeTipologia,
            '                   CatCod,
            '                   nome_destinazione,
            '                   Nothing, Nothing,
            '                   dtTipologia.Rows(0).Item("Sottocartella"),
            '                   Validita_inizio, Validita_fine,
            '                   Allegati_Documenti_Cod,
            '                   objParametri)

            '    objEntita.Allegati_Documenti_Cod = Allegati_Documenti_Cod

            '    If objEntita.ID_Alert_Entita <= 0 Then
            '        objEntita.ID_Alert_Entita = objSeq.NuovoId_Tabella("Alert_Entita", Base, Top, objParametri)

            '        'scrivo l alert entita
            '        objScriviEntita.Scrivi(objEntita, Validita_inizio, Validita_fine, objParametri)
            '    Else
            '        objScriviEntita.Modifica(objEntita, objParametri)
            '    End If


            'End If


            'Else
            'Anche se non ho allegati, modifico l'entità
            'objScriviEntita.Modifica(objEntita, objParametri)

            'End If

            'Modifico l'item nella tabella Elenco 
            objScriviElenco.Modifica(id_elenco, ID_Tipologia, Data_Scadenza, Descrizione_scadenza, note, objParametri)


            'Leggo l'entità
            Dim ae_R As New AgronicaCoreScadenziario.Alert_Entita_R
            Dim objEntita As Alert_Entita = ae_R.LeggiOggetto(Id_Alert_Entita, objParametri)


            'Scrivo il log su tabella
            Dim objLog As New AgronicaCoreScadenziario.Alert_Log_W
            Dim ID_Alert_Log As Integer = objSeq.NuovoId_Tabella("Alert_Log", 0, 2000000000, objParametri)
            objLog.Scrivi(ID_Alert_Log, objEntita.Piva, id_elenco, ID_Tipologia, Descrizione_scadenza, enum_TipoOperazioneDB.Modifica, AGRODATAINIZIO, AGRODATAFINE, objParametri)

            'ANNULLO LE VECCHIE PROGARMMAZIONI MAIL
            Dim mp_W As New AgronicaCoreMailBIZ.Mail_Programmazione_W
            mp_W.CancellaProgrammazioniFromChiave(objParametri, enum_MailTipo.Scadenze_InScadenza, id_elenco)

            'PROGRAMMO L'INVIO DELLA MAIL
            programmaMailPerEvento(objParametri, id_elenco, objEntita.ID_Alert_Entita, ID_Tipologia, objEntita, Data_Scadenza, Descrizione_scadenza, note)

            'GESTIONE INDICI
            If Not IsNothing(EntitaxIndici) Then

                'Cancellazione Preventiva
                objScriviEntitaxIndici.CancellaEntitaxIndice(objEntita.PivaSuperUser, objEntita.ID_Alert_Entita, objParametri)

                For Each obj As JObject In EntitaxIndici

                    objScriviEntitaxIndici.ScriviEntitaxIndice(objEntita.PivaSuperUser, objEntita.ID_Alert_Entita, obj("ID_Indice"), obj("ID_Indice_Det"), obj("Elenco_Val"), obj("Valore_Des"), AGRODATAINIZIO, AGRODATAFINE, objParametri)

                Next

            End If

            If ID_Tipologia = enum_ID_Area_Tipologia.Patentino_trattamenti AndAlso
                    objEntita.Cod_Contatto <> "" Then
                'Se è andato tutto a buonfine, aggiorno il log dei contatti
                Dim NoteLog As String = "Modifica Patentino da modulo Documentale"
                Chiama_Scrivi_Log_Contatti(objEntita.Piva, objEntita.Cod_Contatto, NoteLog, objParametri)
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return MessaggioErrore

    End Function

    Public Function ModificaAutomatica(ByVal ID_Tipologia As Integer,
                                       ByVal id_elenco As Integer,
                                       ByVal Data_Scadenza As Date,
                                       ByVal Descrizione_scadenza As String,
                                       ByVal Note_Scadenza As String,
                                       ByVal ID_Alert_Entita As Integer,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       Optional ByVal ChkDocumento As Integer? = Nothing
                                       ) As String


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_BIZ.ALERT.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder

        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim objScriviElenco As New AgronicaCoreScadenziario.Alert_Elenco_W
            Dim objScriviEntita As New AgronicaCoreScadenziario.Alert_Entita_W

            ' forzati
            Dim Base, Top As Integer
            Base = 0
            Top = 2000000000


            'objEntita.PivaSuperUser = objParametri.PivaSuperUser


            'aggiungo l'eventuale allegato
            'parte degli allegati
            'Dim name As String
            'If Not IsNothing(Nome_Allegato) AndAlso Nome_Allegato <> "" Then
            'name = Nome_Allegato

            'Dim pathSorgente As String = PATH & "TEMP\"

            'Dim dtTipologia As DataTable
            'Dim objTipo As New AgronicaCoreScadenziario.Alert_Tipologia_R
            'dtTipologia = objTipo.Leggi_CatCod(ID_Tipologia, objParametri)


            'Dim pathDestinazione As String = PATH & dtTipologia.Rows(0).Item("Sottocartella") & "\"
            'Dim nome_destinazione As String = ""

            'If name.Contains(pathDestinazione) Then
            '    ' il file è già stato salvato nella cartella corretta
            '    ' quindi recupero solo il nome del file
            '    nome_destinazione = name.Replace(pathDestinazione, "")
            'Else
            '    If Not Directory.Exists(pathDestinazione) = True Then
            '        Directory.CreateDirectory(pathDestinazione)
            '    End If
            '    'controllo se esiste già

            '    Dim k As Integer
            '    For k = 1 To name.Split("_").Length - 1
            '        If nome_destinazione = "" Then
            '            nome_destinazione = name.Split("_")(k)
            '        Else
            '            nome_destinazione = nome_destinazione & "_" & name.Split("_")(k)
            '        End If
            '    Next

            '    If File.Exists(pathDestinazione & nome_destinazione) Then
            '        nome_destinazione = name
            '    End If
            '    File.Copy(pathSorgente & name, pathDestinazione & nome_destinazione)
            '    File.Delete(pathSorgente & name)

            'End If

            ' è possibile che le tabelle alert_entita e allegati documenti siano già stati scritti
            ' come ad esempio per le stampe. Quindi non vado a scrivere altri record, ma uso quelli e la nuova scadenza la lego a quelli

            'carico il nome della tipologia
            'Dim nomeTipologia As String = dtTipologia.Rows(0).Item("nome")
            'Dim CatCod As String = dtTipologia.Rows(0).Item("Cat_Cod")
            'Dim Allegati_Documenti_Cod As Integer

            'Dim strFiltro As String = " Allegati_Documenti_NomeFile='" & nome_destinazione & "' " &
            '                          " AND Sottocartella ='" & dtTipologia.Rows(0).Item("Sottocartella") & "' " &
            '                          " AND Allegati_Documenti_CatCod = " & CatCod

            'Dim dtDoc As DataTable
            'Dim objLeggiEntita As New AgronicaCoreScadenziario.Alert_Entita_R
            'dtDoc = objLeggiEntita.Leggi_con_documenti(0, strFiltro, "", objParametri)


            'If dtDoc.Rows.Count > 0 Then

            '    objEntita.Allegati_Documenti_Cod = dtDoc.Rows(0).Item("Allegati_Documenti_Cod")

            '    objEntita.ID_Alert_Entita = dtDoc.Rows(0).Item("Id_Alert_Entita")

            'Else

            '    'creo l'entita e il documento associato
            '    Dim objDoc As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
            '    objDoc.Scrivi(objEntita.Piva,
            '                   nomeTipologia,
            '                   CatCod,
            '                   nome_destinazione,
            '                   Nothing, Nothing,
            '                   dtTipologia.Rows(0).Item("Sottocartella"),
            '                   Validita_inizio, Validita_fine,
            '                   Allegati_Documenti_Cod,
            '                   objParametri)

            '    objEntita.Allegati_Documenti_Cod = Allegati_Documenti_Cod

            '    If objEntita.ID_Alert_Entita <= 0 Then
            '        objEntita.ID_Alert_Entita = objSeq.NuovoId_Tabella("Alert_Entita", Base, Top, objParametri)

            '        'scrivo l alert entita
            '        objScriviEntita.Scrivi(objEntita, Validita_inizio, Validita_fine, objParametri)
            '    Else
            '        objScriviEntita.Modifica(objEntita, objParametri)
            '    End If


            'End If


            'Else
            'Anche se non ho allegati, modifico l'entità
            'objScriviEntita.Modifica(objEntita, objParametri)

            'End If

            'Se previsto, aggiorno l'indicativo su Entità
            If Not IsNothing(ChkDocumento) Then
                objScriviEntita.Modifica_ChkDocumento(ChkDocumento, ID_Alert_Entita, objParametri)
            End If

            'Leggo l'entità
            Dim ae_R As New AgronicaCoreScadenziario.Alert_Entita_R
            Dim objEntita As Alert_Entita = ae_R.LeggiOggetto(ID_Alert_Entita, objParametri)


            'Modifico l'item nella tabella Elenco 
            objScriviElenco.ModificaAutomatica(id_elenco,
                                               Data_Scadenza,
                                               Descrizione_scadenza,
                                               objParametri)

            'Scrivo il log su tabella
            Dim objLog As New AgronicaCoreScadenziario.Alert_Log_W
            Dim ID_Alert_Log As Integer = objSeq.NuovoId_Tabella("Alert_Log", 0, 2000000000, objParametri)
            objLog.Scrivi(ID_Alert_Log, objEntita.Piva, id_elenco, ID_Tipologia, Descrizione_scadenza, enum_TipoOperazioneDB.Modifica, AGRODATAINIZIO, AGRODATAFINE, objParametri)

            'ANNULLO LE VECCHIE PROGRAMMAZIONI MAIL
            Dim mp_W As New AgronicaCoreMailBIZ.Mail_Programmazione_W
            mp_W.CancellaProgrammazioniFromChiave(objParametri, enum_MailTipo.Scadenze_InScadenza, id_elenco)


            'PROGRAMMO L'INVIO DELLA MAIL
            programmaMailPerEvento(objParametri, id_elenco, objEntita.ID_Alert_Entita, ID_Tipologia, objEntita, Data_Scadenza, Descrizione_scadenza, Note_Scadenza)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return MessaggioErrore

    End Function

    Public Function Cancella(ByVal ID_Elenco As Integer,
                             ByVal PATH As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Entita_R.Leggi()"

        Dim esito As Boolean
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim ID_Tipologia As Integer = 0
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            'parto con la cancellazione dell'allegato
            Dim objElenco_R As New AgronicaCoreScadenziario.Alert_Elenco_R
            Dim DT_Elenco As DataTable = objElenco_R.Leggi(ID_Elenco, 0, objParametri)
            ID_Tipologia = DT_Elenco(0)("ID_Tipologia")

            'leggo l Entità
            Dim objEntita_R As New AgronicaCoreScadenziario.Alert_Entita_R
            Dim DT_Entita As DataTable = objEntita_R.Leggi(DT_Elenco.Rows(0).Item("ID_Alert_Entita"), objParametri)


            'scrivo il log
            Dim Descrizione_scadenza As String = DT_Elenco.Rows(0).Item("Descrizione_scadenza")
            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim ID_Alert_Log As Integer = objSeq.NuovoId_Tabella("Alert_Log", 0, 2000000000, objParametri)

            Dim objLog As New AgronicaCoreScadenziario.Alert_Log_W
            objLog.Scrivi(ID_Alert_Log, DT_Entita(0)("Piva"), ID_Elenco, ID_Tipologia, Descrizione_scadenza, enum_TipoOperazioneDB.Cancellazione, AGRODATAINIZIO, AGRODATAFINE, objParametri)


            'controllo se ho un allegato associato
            If Not IsDBNull(DT_Entita.Rows(0).Item("Allegati_Documenti_cod")) AndAlso DT_Entita.Rows(0).Item("Allegati_Documenti_cod") <> 0 Then
                'leggo l'allegato 
                Dim objAllegato_R As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
                Dim DT_Allegato As DataTable = objAllegato_R.Leggi(DT_Entita.Rows(0).Item("Allegati_Documenti_cod"),
                                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                    "", "", objParametri)

                If DT_Allegato.Rows.Count > 0 Then

                    'controllo se esiste il file 
                    If Not IsDBNull(DT_Allegato.Rows(0).Item("Sottocartella")) And Not IsDBNull(DT_Allegato.Rows(0).Item("Allegati_documenti_nomefile")) Then

                        Dim sottocartella As String = DT_Allegato.Rows(0).Item("Sottocartella")
                        Dim nomefile As String = DT_Allegato.Rows(0).Item("Allegati_documenti_nomefile")

                        If File.Exists(PATH & sottocartella & "/" & nomefile) Then
                            File.Delete(PATH & sottocartella & "/" & nomefile)
                        Else
                            'Provo senza path
                            If File.Exists(sottocartella & "/" & nomefile) Then
                                File.Delete(sottocartella & "/" & nomefile)
                            End If
                        End If

                    End If

                    'cancello il record Allegati
                    Dim objAllegati_W As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
                    objAllegati_W.Cancella(DT_Entita.Rows(0).Item("Allegati_Documenti_cod"), "", objParametri)

                    If DT_Allegato.Rows(0).Item("Pratica_Cod") > 0 Then
                        'cancello la pratica associata
                        Dim objpratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_W
                        objpratiche.Elimina_Pratica(DT_Allegato.Rows(0).Item("Pratica_Cod"), objParametri, MessaggioErrore)
                    End If
                End If

            End If


            'cancello il record Elenco
            Dim objELenco_W As New AgronicaCoreScadenziario.Alert_Elenco_W
            objELenco_W.Cancella(ID_Elenco, objParametri)


            'cancello i record indicixentita
            Dim objEntitaxIndici_W As New AgronicaCoreScadenziario.Alert_Indice_W
            objEntitaxIndici_W.CancellaEntitaxIndice("", DT_Elenco.Rows(0).Item("ID_Alert_Entita"), objParametri)



            'cancello il record Entita
            Dim objEntita_W As New AgronicaCoreScadenziario.Alert_Entita_W
            objEntita_W.Cancella(DT_Elenco.Rows(0).Item("ID_Alert_Entita"), objParametri)

            'ANNULLO LE VECCHIE PROGARMMAZIONI MAIL
            Dim mp_W As New AgronicaCoreMailBIZ.Mail_Programmazione_W
            mp_W.CancellaProgrammazioniFromChiave(objParametri, enum_MailTipo.Scadenze_InScadenza, ID_Elenco)

            If DT_Elenco.Rows(0).Item("ID_Tipologia") = enum_ID_Area_Tipologia.Patentino_trattamenti AndAlso
                    DT_Entita.Rows(0).Item("Cod_Contatto") <> "" Then
                'Se è andato tutto a buonfine, aggiorno il log dei contatti
                Dim NoteLog As String = "Cancellazione Patentino da modulo Documentale"
                Chiama_Scrivi_Log_Contatti(DT_Entita.Rows(0).Item("Piva"), DT_Entita.Rows(0).Item("Cod_Contatto"), NoteLog, objParametri)
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            esito = True

        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            esito = False

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return esito

    End Function

    Private Function programmaMailPerEvento(ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByVal Id_Elenco As Integer,
                                            ByVal Id_Alert_Entita As Integer,
                                            ByVal ID_Tipologia As Integer,
                                            ByVal objEntita As AgronicaCoreScadenziario.Alert_Entita,
                                            ByVal Data_Scadenza As Date,
                                            ByVal Descrizione_scadenza As String,
                                            ByVal Note_Scadenza As String
                                            ) As Boolean

        'Estraggo l'area
        Dim objTipologia As New Alert_Tipologia_R
        Dim dtTipologia As DataTable = objTipologia.Leggi(0, ID_Tipologia, "", False, objParametri_Server)
        Dim id_area As Integer = dtTipologia.Rows(0).Item("id_area")

        'Leggo le regole per l'alerting
        Dim avv_R As New AgronicaCoreScadenziario_BIZ.Alert_Avvisi_R
        Dim alertAvv As List(Of Alert_Avvisi) = avv_R.leggi_Alert_Avvisi(objParametri_Server, Nothing, id_area, ID_Tipologia, "", enum_Alert_Eventi.ScadenzaInScadenza, Nothing)

        If alertAvv.Count > 0 Then

            'Estraggo la ragione sociale
            Dim DataSpedizione As Date = Data_Scadenza
            Dim imp_R As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim rag_Soc As String = imp_R.RagSoc_from_Piva(objEntita.Piva, objParametri_Server)

            'Estraggo la categorizzazione
            Dim objArea As New Alert_Area_R
            Dim dtArea As DataTable = objArea.Leggi("", id_area, objParametri_Server)

            Dim Area As String = dtArea.Rows(0).Item("Nome")
            Dim Tipologia As String = dtTipologia.Rows(0).Item("Nome")
            Dim categoria As String = Area & " - " & Tipologia

            Dim oggetto As String = "Scadenza per l'azienda '" & rag_Soc & "' "
            Dim datoPersonalizzato As String = ""

            Select Case ID_Tipologia
                Case enum_ID_Area_Tipologia.Analisi_terreno
                    Dim obj As New AgronicaCoreAnagrafeDAL.Analisi_Testata_R
                    Dim nomeAnalisi As String = obj.TestataDes_from_TestataCod(objEntita.Analisi_Testata_Cod, objParametri_Server)

                    oggetto &= "dell'analisi del terreno '" & nomeAnalisi & "'"
                    datoPersonalizzato = "<b>Analisi: </b>" & nomeAnalisi & "<br>"

                Case enum_ID_Area_Tipologia.Patentino_trattamenti
                    Dim obj As New AgronicaCoreAnagrafeDAL.Contatti_R
                    Dim dt As DataTable = obj.Contatti_Contatto_Leggi(objEntita.Piva, objEntita.Cod_Contatto, 0, 0, False, False, 0, 0, False, 0, ID_CF_NOFILTRO, 0, "", False, 0, 0, 0, 0, 0,
                                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)

                    Dim nomeContatto As String = (CStr(dt.Rows(0).Item("Rag_Soc")) & CStr(dt.Rows(0).Item("Cognome")) & " " & CStr(dt.Rows(0).Item("Nome")) & " (" & dt.Rows(0).Item("Rapporto_Des") & ")").Replace("""", "'")

                    oggetto &= "del patentino di '" & nomeContatto & "'"
                    datoPersonalizzato = "<b>Contatto: </b>" & nomeContatto & "<br>"

                Case enum_ID_Area_Tipologia.Carta_Identita
                    Dim obj As New AgronicaCoreAnagrafeDAL.Contatti_R
                    Dim dt As DataTable = obj.Contatti_Contatto_Leggi(objEntita.Piva, objEntita.Cod_Contatto, 0, 0, False, False, 0, 0, False, 0, ID_CF_NOFILTRO, 0, "", False, 0, 0, 0, 0, 0,
                                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)

                    Dim nomeContatto As String = (CStr(dt.Rows(0).Item("Rag_Soc")) & CStr(dt.Rows(0).Item("Cognome")) & " " & CStr(dt.Rows(0).Item("Nome")) & " (" & dt.Rows(0).Item("Rapporto_Des") & ")").Replace("""", "'")

                    oggetto &= "della carta di identità di '" & nomeContatto & "'"
                    datoPersonalizzato = "<b>Contatto: </b>" & nomeContatto & "<br>"

                Case enum_ID_Area_Tipologia.Taratura_ugelli
                    Dim obj As New AgronicaCoreContabDAL.Parco_Macchine_R
                    Dim nomeMacchina As String = obj.MacchinaDes_from_MacchinaCod(objEntita.Piva, objEntita.Mac_Cod, objParametri_Server)

                    oggetto &= "della taratura ugelli della macchina " & nomeMacchina & "'"
                    datoPersonalizzato = "<b>Macchina: </b>" & nomeMacchina & "<br>"

                Case enum_ID_Area_Tipologia.PUA
                    Dim obj As New AgronicaCorePUA_DAL.PUA_Testata_R
                    Dim dt As DataTable = obj.Leggi(0, objEntita.PUA_Cod, objEntita.Piva, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

                    Dim nomePUA As String = "dell'anno " & CStr(dt.Rows(0).Item("Pua_Anno"))

                    oggetto &= "del Pua '" & nomePUA & "'"
                    datoPersonalizzato = "<b>Pua: </b>" & nomePUA & "<br>"

                Case enum_ID_Area_Tipologia.Piano_Concimazione
                    Dim obj As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_R
                    Dim dt As DataTable = obj.Leggi(objEntita.PC_Testata_Cod, 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                    Dim nomePianoConcimazione As String = CStr(dt.Rows(0).Item("PC_Testata_Des"))

                    oggetto &= "del piano di concimazione '" & nomePianoConcimazione & "'"
                    datoPersonalizzato = "<b>Piano Concimazione: </b>" & nomePianoConcimazione & "<br>"

                Case Else
                    oggetto &= "di tipo '" & categoria & "'"
            End Select

            oggetto &= " il " & Data_Scadenza.ToShortDateString()

            Dim testoMail As String = "<b>Azienda: </b>" & rag_Soc & "<br>" &
                                      "<b>Categoria: </b>" & categoria & "<br>" &
                                      datoPersonalizzato &
                                      "<b>Data Scadenza: </b>" & Data_Scadenza.ToShortDateString() & "<br>" &
                                      "<b>Descrizione: </b>" & Descrizione_scadenza & "<br>" &
                                      "<b>Note: </b>" & Note_Scadenza

            Dim mp_W As New AgronicaCoreMailBIZ.Mail_Programmazione_W

            For Each avv As Alert_Avvisi In alertAvv

                'Aggiorno la data di spedizione in base ai giorni di attesa
                If IsNumeric(avv.GGAttesa) Then
                    DataSpedizione = DataSpedizione.AddDays(avv.GGAttesa)

                    'Se la data è già passata e non è un invio immediato, ignoro la regola di avviso
                    If avv.GGAttesa <> 0 AndAlso DataSpedizione < DateTime.Now Then
                        Continue For
                    End If

                End If

                'Inizializzo ed estraggo gli indirizzi mail
                Dim strMailA As String = avv.MailA.Trim()
                Dim strMailCC As String = avv.MailCC.Trim()
                Dim strMailCCN As String = ""
                Dim Filtro_RapConMail As String = avv.Filtro_RapCon.Trim()

                Dim res As Boolean = mp_W.ScriviNuovaMail(objParametri_Server, enum_MailTipo.Scadenze_InScadenza,
                                      Id_Elenco, avv.MailMittente, strMailA, strMailCC, strMailCCN,
                                      oggetto, testoMail, True, "", DataSpedizione,,,,, Filtro_RapConMail)

            Next

        End If

        Return True

    End Function

    Public Function Importa(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal ArrayID_Tipologia As String(), ByRef msgErr As String) As String

        Dim msgEsito As String = ""
        Dim msgEsitoContesto As String = ""

        Dim nomeFileLog = String.Format("ImportaScadenzeLog_{0}_{1}_{2}.txt",
                                        objParametri.SuperUserUsername,
                                        Now().ToString("yyyy"),
                                        Now().ToString("MM"))

        Dim objScritturaLog As New ScritturaLog(objParametri, nomeFileLog)

        objScritturaLog.ScriviLog("### Inizio Importazione Scadenze")

        'PATENTINI
        If ArrayID_Tipologia.Contains(enum_ID_Area_Tipologia.Patentino_trattamenti) Then

            Dim PatentiniErrori As String = ""

            objScritturaLog.ScriviLog("Importa_Patentini_Cancellati")
            Dim PatentiniCancellati As Integer = Importa_Patentini_Cancellati(objParametri, PatentiniErrori)

            objScritturaLog.ScriviLog("Importa_Patentini_Modificati")
            Dim PatentiniModificati As Integer = Importa_Patentini_Modificati(objParametri, PatentiniErrori)

            objScritturaLog.ScriviLog("Importa_Patentini_Nuovi")
            Dim PatentiniNuovi As Integer = Importa_Patentini_Nuovi(objParametri, PatentiniErrori)

            msgEsitoContesto = "Importazione Patentini: " & PatentiniNuovi & " nuovi, " & PatentiniModificati & " aggiornati, " & PatentiniCancellati & " eliminati"
            objScritturaLog.ScriviLog(msgEsitoContesto)

            msgEsito &= msgEsitoContesto & vbCrLf
            msgErr &= PatentiniErrori

        End If

        'TARATURA UGELLI
        If ArrayID_Tipologia.Contains(enum_ID_Area_Tipologia.Taratura_ugelli) Then

            Dim TaratureUgelliErrori As String = ""

            objScritturaLog.ScriviLog("Importa_TaratureUgelli_Cancellati")
            Dim TaratureUgelliCancellati As Integer = Importa_TaratureUgelli_Cancellati(objParametri, TaratureUgelliErrori)

            objScritturaLog.ScriviLog("Importa_TaratureUgelli_Modificati")
            Dim TaratureUgelliModificati As Integer = Importa_TaratureUgelli_Modificati(objParametri, TaratureUgelliErrori)

            objScritturaLog.ScriviLog("Importa_TaratureUgelli_Nuovi")
            Dim TaratureUgelliNuovi As Integer = Importa_TaratureUgelli_Nuovi(objParametri, TaratureUgelliErrori)

            msgEsitoContesto = "Importazione Tarature Ugelli: " & TaratureUgelliNuovi & " nuovi, " & TaratureUgelliModificati & " aggiornati, " & TaratureUgelliCancellati & " eliminati"
            objScritturaLog.ScriviLog(msgEsitoContesto)

            msgEsito &= msgEsitoContesto & vbCrLf
            msgErr &= TaratureUgelliErrori

        End If

        'ANALISI TERRENO
        If ArrayID_Tipologia.Contains(enum_ID_Area_Tipologia.Analisi_terreno) Then

            Dim AnalisiErrori As String = ""

            objScritturaLog.ScriviLog("Importa_AnalisiTerreno_Cancellati")
            Dim AnalisiCancellati As Integer = Importa_AnalisiTerreno_Cancellati(objParametri, AnalisiErrori)

            objScritturaLog.ScriviLog("Importa_AnalisiTerreno_Modificati")
            Dim AnalisiModificati As Integer = Importa_AnalisiTerreno_Modificati(objParametri, AnalisiErrori)

            objScritturaLog.ScriviLog("Importa_AnalisiTerreno_Nuovi")
            Dim AnalisiNuovi As Integer = Importa_AnalisiTerreno_Nuovi(objParametri, AnalisiErrori)

            msgEsitoContesto = "Importazione Analisi del Terreno: " & AnalisiNuovi & " nuovi, " & AnalisiModificati & " aggiornati, " & AnalisiCancellati & " eliminati"
            objScritturaLog.ScriviLog(msgEsitoContesto)

            msgEsito &= msgEsitoContesto & vbCrLf
            msgErr &= AnalisiErrori

        End If

        'CONTRATTI AFFITTO
        If ArrayID_Tipologia.Contains(enum_ID_Area_Tipologia.Contratti_Affitto) Then

            Dim ContrattiAffittoErrori As String = ""

            objScritturaLog.ScriviLog("Importa_ContrattiAffitto_Cancellati")
            Dim ContrattiAffittoCancellati As Integer = Importa_ContrattiAffitto_Cancellati(objParametri, ContrattiAffittoErrori)

            objScritturaLog.ScriviLog("Importa_ContrattiAffitto_Nuovi")
            Dim ContrattiAffittoNuovi As Integer = Importa_ContrattiAffitto_Nuovi(objParametri, ContrattiAffittoErrori)

            msgEsitoContesto = "Importazione Contratti di Affitto: " & ContrattiAffittoNuovi & " nuovi, " & ContrattiAffittoCancellati & " eliminati"
            objScritturaLog.ScriviLog(msgEsitoContesto)

            msgEsito &= msgEsitoContesto & vbCrLf
            msgErr &= ContrattiAffittoErrori

        End If

        'PARTICELLE DA AFFITTO CATASTALE
        If ArrayID_Tipologia.Contains(enum_ID_Area_Tipologia.Possesso_Particelle) Then

            Dim ParticelleImpreseErrori As String = ""

            objScritturaLog.ScriviLog("Importa_ParticelleImprese_Cancellati")
            Dim ParticelleImpreseCancellati As Integer = Importa_ParticelleImprese_Cancellati(objParametri, ParticelleImpreseErrori)

            objScritturaLog.ScriviLog("Importa_ParticelleImprese_Modificati")
            Dim ParticelleImpreseModificati As Integer = Importa_ParticelleImprese_Modificati(objParametri, ParticelleImpreseErrori)

            objScritturaLog.ScriviLog("Importa_ParticelleImprese_Nuovi")
            Dim ParticelleImpreseNuovi As Integer = Importa_ParticelleImprese_Nuovi(objParametri, ParticelleImpreseErrori)

            msgEsitoContesto = "Importazione Particelle Catastali: " & ParticelleImpreseNuovi & " nuovi, " & ParticelleImpreseModificati & " aggiornati, " & ParticelleImpreseCancellati & " eliminati"
            objScritturaLog.ScriviLog(msgEsitoContesto)

            msgEsito &= msgEsitoContesto & vbCrLf
            msgErr &= ParticelleImpreseErrori

        End If

        objScritturaLog.ScriviLog("### Fine Importazione Scadenze")

        Return msgEsito & If(msgErr = "", "", "<br>ERRORI RILEVATI:<br>" & msgErr)

    End Function

    Private Function Importa_Patentini_Nuovi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef msgErr As String) As Integer

        Dim imp_R As New Alert_Importazioni()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        Dim dtNuovi As DataTable = imp_R.Leggi_Patentini_Nuovi(objParametri)
        Dim nImportati As Integer = 0

        For Each dr As DataRow In dtNuovi.Rows

            Dim descr As String = "Patentino " & If(dr("patentino") = "", "di ", dr("patentino") + " di ") & dr("cognome") + " " & dr("nome")
            descr &= If(dr("ente_di_rilascio") <> "" OrElse IsDate(dr("data_rilascio_patentino")), " rilasciato" & If(dr("ente_di_rilascio") <> "", " da " & dr("ente_di_rilascio"), "") + If(IsDate(dr("data_rilascio_patentino")), " il " & CDate(dr("data_rilascio_patentino")).ToShortDateString(), ""), "")
            'descr &= " con scadenza al " & CDate(dr("data_scadenza_patentino")).ToShortDateString()

            Dim note As String = ""
            Dim dataScadenza As DateTime = dr("data_scadenza_patentino")
            Dim id_tipologia As Integer = enum_ID_Area_Tipologia.Patentino_trattamenti

            Dim objE As New AgronicaCoreScadenziario.Alert_Entita With {
                .analisi_campione_cod = 0,
                .Appezza = 0,
                .Campo_Cod = 0,
                .COM = "",
                .FOGLIO = 0,
                .ID_Agenda = 0,
                .Id_Imp = 0,
                .NUMERO = 0,
                .Piva = dr("piva"),
                .PivaSuperUser = objParametri.PivaSuperUser,
                .Programmazione_Entita_Cod = 0,
                .PROV = "",
                .Ricetta_Operazione_cod = 0,
                .Sa_Cod = 0,
                .SEZIONE = "",
                .SUBALTERNO = "",
                .Cod_Contatto = dr("Cod_Contatto"),
                .TipoEntita_Cod = enum_TipoEntita.Contatto,
                .Analisi_Testata_Cod = 0,
                .PC_Testata_Cod = 0,
                .PUA_Cod = 0,
                .Mac_Cod = 0,
                .Id_ImpresexParticelle = 0
            }

            Dim ret_Id_Elenco As Integer = -1
            Dim strErr As String = alert_W.Scrivi(id_tipologia, objE, dataScadenza, descr, "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri, note, ret_Id_Elenco)

            If strErr = "" Then
                nImportati += 1
            Else
                msgErr &= strErr & vbCrLf
            End If
        Next

        Return nImportati

    End Function

    Private Function Importa_Patentini_Modificati(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef msgErr As String) As Integer

        Dim imp_R As New Alert_Importazioni()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        Dim dtModificati As DataTable = imp_R.Leggi_Patentini_Modificati(objParametri)
        Dim nImportati As Integer = 0

        For Each dr As DataRow In dtModificati.Rows

            Dim descr As String = "Patentino " & If(dr("patentino") = "", "di ", dr("patentino") + " di ") & dr("cognome") + " " & dr("nome")
            descr &= If(dr("ente_di_rilascio") <> "" OrElse IsDate(dr("data_rilascio_patentino")), " rilasciato" & If(dr("ente_di_rilascio") <> "", " da " & dr("ente_di_rilascio"), "") + If(IsDate(dr("data_rilascio_patentino")), " il " & CDate(dr("data_rilascio_patentino")).ToShortDateString(), ""), "")
            'descr &= " con scadenza al " & CDate(dr("data_scadenza_patentino")).ToShortDateString()

            Dim dataScadenza As DateTime = dr("data_scadenza_patentino")
            Dim id_tipologia As Integer = enum_ID_Area_Tipologia.Patentino_trattamenti

            Dim strerr As String = alert_W.ModificaAutomatica(id_tipologia, CInt(dr("id_elenco")), dataScadenza, descr, dr("Note"), dr("ID_Alert_Entita"), objParametri)

            If strerr = "" Then
                nImportati += 1
            Else
                msgErr &= strerr & vbCrLf
            End If
        Next

        Return nImportati

    End Function

    Private Function Importa_Patentini_Cancellati(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef msgErr As String) As Integer

        Dim imp_R As New Alert_Importazioni()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        Dim dtCancellati As DataTable = imp_R.Leggi_Patentini_Cancellati(objParametri)
        Dim nImportati As Integer = 0

        For Each dr As DataRow In dtCancellati.Rows

            Dim esito As Boolean = alert_W.Cancella(CInt(dr("id_elenco")), "", objParametri)
            Dim strerr As String = If(esito = True, "", "Errore durante la cancellazione della scadenza " & dr("id_elenco"))

            If strerr = "" Then
                nImportati += 1
            Else
                msgErr &= strerr & vbCrLf
            End If
        Next

        Return nImportati

    End Function

    Private Function Importa_AnalisiTerreno_Nuovi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef msgErr As String) As Integer

        Dim imp_R As New Alert_Importazioni()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        Dim dtNuovi As DataTable = imp_R.Leggi_AnalisiTerreno_Nuovi(objParametri)
        Dim nImportati As Integer = 0

        For Each dr As DataRow In dtNuovi.Rows

            'Esistono analisi associate a nulla, di queste, non gestisco nulla
            If IsDBNull(dr("piva")) Then
                Continue For
            End If

            Dim entita As String = ""
            Dim sa_cod As Integer = 0
            Dim campo_cod As Integer = 0
            Dim appezza As Integer = 0
            Dim id_reg As Integer = 0
            Dim fabbricato_cod As Integer = 0
            Dim prov As String = ""
            Dim com As String = ""
            Dim sezione As String = ""
            Dim foglio As Integer = 0
            Dim numero As Integer = 0
            Dim subalterno As String = ""

            'Select Case CInt(dr("Analisi_Entita_Cod"))
            '    Case enum_Entita_Analisi.Impresa
            '        entita = "sull'Azienda '" & dr("rag_soc") & "'"
            '    Case enum_Entita_Analisi.Centro
            '        entita = "sul Centro Aziendale '" & dr("sa_nome") & "'"
            '        sa_cod = dr("sa_cod")
            '    Case enum_Entita_Analisi.Campo
            '        entita = "sul Campo '" & dr("campo_des") & "' del centro '" & dr("sa_nome") & "'"
            '        sa_cod = dr("sa_cod")
            '        campo_cod = dr("campo_cod")
            '    Case enum_Entita_Analisi.Appezzamento
            '        entita = "sull'Appezzamento '" & dr("app_nome") & "' del centro '" & dr("sa_nome") & "'"
            '        sa_cod = dr("sa_cod")
            '        appezza = dr("appezza")
            '    Case enum_Entita_Analisi.Impianto
            '        entita = "sull'Impianto di '" & dr("Veg_Des") & " - " & dr("Cul_Des") & "' del " & dr("imp_Validita_Inizio") & " nell'Appezzamento '" & dr("app_nome") & "' del centro '" & dr("sa_nome") & "'"
            '        sa_cod = dr("sa_cod")
            '        appezza = dr("appezza")
            '        id_reg = dr("id_imp")
            '    Case enum_Entita_Analisi.Fabbricato
            '        entita = "sul Fabbricato '" & dr("fabbricato_des") & "' del centro '" & dr("sa_nome") & "'"
            '        sa_cod = dr("sa_cod")
            '        fabbricato_cod = dr("fabbricato_cod")
            '    Case enum_Entita_Analisi.Particella
            '        entita = "sulla Particella " & dr("prov") & "_" & dr("com") & "_" & dr("sezione") & "_" & dr("foglio") & "_" & dr("numero") & "_" & dr("subalterno")
            '        prov = dr("prov")
            '        com = dr("com")
            '        sezione = dr("sezione")
            '        foglio = dr("foglio")
            '        numero = dr("numero")
            '        subalterno = dr("subalterno")
            '    Case enum_Entita_Analisi.EntitaGrafica
            '        entita = "sull'Entità Grafica " & dr("ID_Oggetto_Grafico")
            '        sa_cod = dr("sa_cod")
            'End Select

            Dim descr As String = "Analisi del Terreno '" & dr("Analisi_Testata_Des") & "'" '(" & dr("ncert") & ")"
            'descr &= " eseguita " & entita
            ''descr &= " rilasciato dal laboratorio " & dr("lab") & " il " & CDate(dr("validita_inizio")).ToShortDateString()
            'descr &= " in data " & CDate(dr("Analisi_Testata_Data_Inizio")).ToShortDateString()
            ''descr &= " con scadenza al " & CDate(dr("Analisi_Testata_Data_Fine")).ToShortDateString()

            If dr("Analisi_Certificato_Cod") <> "0" Then
                descr &= " con codice '" & dr("Analisi_Certificato_Cod") & "'"
            End If
            If IsDate(dr("Analisi_Testata_Data_Inizio")) AndAlso dr("Analisi_Testata_Data_Inizio") <> AGRODATAINIZIO Then
                descr &= " eseguita in data " & CDate(dr("Analisi_Testata_Data_Inizio")).ToShortDateString()
            End If

            Dim note As String = ""
            Dim dataScadenza As DateTime = dr("Analisi_Testata_Data_Fine")
            Dim id_tipologia As Integer = enum_ID_Area_Tipologia.Analisi_terreno

            Dim objE As New AgronicaCoreScadenziario.Alert_Entita With {
                .analisi_campione_cod = 0,
                .Appezza = appezza,
                .Campo_Cod = campo_cod,
                .COM = com,
                .FOGLIO = foglio,
                .ID_Agenda = 0,
                .Id_Imp = id_reg,
                .NUMERO = numero,
                .Piva = dr("piva"),
                .PivaSuperUser = objParametri.PivaSuperUser,
                .Programmazione_Entita_Cod = 0,
                .PROV = prov,
                .Ricetta_Operazione_cod = 0,
                .Sa_Cod = sa_cod,
                .SEZIONE = sezione,
                .SUBALTERNO = subalterno,
                .Cod_Contatto = "",
                .TipoEntita_Cod = enum_TipoEntita.AnalisiTerreno,
                .Analisi_Testata_Cod = dr("Analisi_Testata_Cod"),
                .PC_Testata_Cod = 0,
                .PUA_Cod = 0,
                .Mac_Cod = 0,
                .Id_ImpresexParticelle = 0
            }

            Dim ret_Id_Elenco As Integer = -1
            Dim strErr As String = alert_W.Scrivi(id_tipologia, objE, dataScadenza, descr, "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri, note, ret_Id_Elenco)

            If strErr = "" Then
                nImportati += 1
            Else
                msgErr &= strErr & vbCrLf
            End If
        Next

        Return nImportati

    End Function

    Private Function Importa_AnalisiTerreno_Modificati(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef msgErr As String) As Integer

        Dim imp_R As New Alert_Importazioni()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        Dim dtModificati As DataTable = imp_R.Leggi_AnalisiTerreno_Modificati(objParametri)
        Dim nImportati As Integer = 0

        For Each dr As DataRow In dtModificati.Rows

            'Dim entita As String = ""

            'Select Case CInt(dr("Analisi_Entita_Cod"))
            '    Case enum_Entita_Analisi.Impresa
            '        entita = "sull'Azienda '" & dr("rag_soc") & "'"
            '    Case enum_Entita_Analisi.Centro
            '        entita = "sul Centro Aziendale '" & dr("sa_nome") & "'"
            '    Case enum_Entita_Analisi.Campo
            '        entita = "sul Campo '" & dr("campo_des") & "' del centro '" & dr("sa_nome") & "'"
            '    Case enum_Entita_Analisi.Appezzamento
            '        entita = "sull'Appezzamento '" & dr("app_nome") & "' del centro '" & dr("sa_nome") & "'"
            '    Case enum_Entita_Analisi.Impianto
            '        entita = "sull'Impianto di '" & dr("Veg_Des") & " - " & dr("Cul_Des") & "' del " & dr("imp_Validita_Inizio") & " nell'Appezzamento '" & dr("app_nome") & "' del centro '" & dr("sa_nome") & "'"
            '    Case enum_Entita_Analisi.Fabbricato
            '        entita = "sul Fabbricato '" & dr("fabbricato_des") & "' del centro '" & dr("sa_nome") & "'"
            '    Case enum_Entita_Analisi.Particella
            '        entita = "sulla Particella " & dr("prov") & "_" & dr("com") & "_" & dr("sezione") & "_" & dr("foglio") & "_" & dr("numero") & "_" & dr("subalterno")
            '    Case enum_Entita_Analisi.EntitaGrafica
            '        entita = "sull'Entità Grafica " & dr("ID_Oggetto_Grafico")
            'End Select

            Dim descr As String = "Analisi del Terreno '" & dr("Analisi_Testata_Des") & "'" '(" & dr("ncert") & ")"
            'descr &= " eseguita " & entita
            ''descr &= " rilasciato dal laboratorio " & dr("lab") & " il " & CDate(dr("validita_inizio")).ToShortDateString()
            'descr &= " in data " & CDate(dr("Analisi_Testata_Data_Inizio")).ToShortDateString()
            ''descr &= " con scadenza al " & CDate(dr("Analisi_Testata_Data_Fine")).ToShortDateString()

            If dr("Analisi_Certificato_Cod") <> "0" Then
                descr &= " con codice '" & dr("Analisi_Certificato_Cod") & "'"
            End If
            If IsDate(dr("Analisi_Testata_Data_Inizio")) AndAlso dr("Analisi_Testata_Data_Inizio") <> AGRODATAINIZIO Then
                descr &= " eseguita in data " & CDate(dr("Analisi_Testata_Data_Inizio")).ToShortDateString()
            End If


            Dim dataScadenza As DateTime = dr("Analisi_Testata_Data_Fine")
            Dim id_tipologia As Integer = enum_ID_Area_Tipologia.Analisi_terreno

            Dim strerr As String = alert_W.ModificaAutomatica(id_tipologia, CInt(dr("id_elenco")), dataScadenza, descr, dr("Note"), dr("ID_Alert_Entita"), objParametri)

            If strerr = "" Then
                nImportati += 1
            Else
                msgErr &= strerr & vbCrLf
            End If

        Next

        Return nImportati

    End Function

    Private Function Importa_AnalisiTerreno_Cancellati(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef msgErr As String) As Integer

        Dim imp_R As New Alert_Importazioni()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        Dim dtCancellati As DataTable = imp_R.Leggi_AnalisiTerreno_Cancellati(objParametri)
        Dim nImportati As Integer = 0

        For Each dr As DataRow In dtCancellati.Rows

            Dim esito As Boolean = alert_W.Cancella(CInt(dr("id_elenco")), "", objParametri)
            Dim strerr As String = If(esito = True, "", "Errore durante la cancellazione della scadenza " & dr("id_elenco"))

            If strerr = "" Then
                nImportati += 1
            Else
                msgErr &= strerr & vbCrLf
            End If
        Next

        Return nImportati

    End Function

    Private Function Importa_TaratureUgelli_Nuovi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef msgErr As String) As Integer

        Dim imp_R As New Alert_Importazioni()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        Dim dtNuovi As DataTable = imp_R.Leggi_TaratureUgelli_Nuovi(objParametri)
        Dim nImportati As Integer = 0

        For Each dr As DataRow In dtNuovi.Rows

            Dim marcaModello As String = String.Join(" - ", {dr("ditta_des"), dr("modello")}.Where(Function(s) Not String.IsNullOrEmpty(s))) & " "
            Dim descrizioneMac As String = If(String.IsNullOrEmpty(dr("mac_des")), "", "(" & dr("mac_des") & ") ")
            Dim ultimaTaratura As String = If(IsDate(dr("Validita_Taratura_Inizio")) AndAlso dr("Validita_Taratura_Inizio") <> AGRODATAINIZIO AndAlso dr("Validita_Taratura_Inizio") <> AGRODATAFINE, " eseguita il " & CDate(dr("Validita_Taratura_Inizio")).ToShortDateString() & " ", "")

            Dim descr As String = "Taratura Ugelli della macchina " & marcaModello & descrizioneMac & "di tipo '" & dr("class_desc") & "' " & ultimaTaratura
            'descr &= "con scadenza al " & CDate(dr("Validita_Taratura_Fine")).ToShortDateString()

            Dim note As String = ""
            Dim dataScadenza As DateTime = dr("Validita_Taratura_Fine")
            Dim id_tipologia As Integer = enum_ID_Area_Tipologia.Taratura_ugelli

            Dim objE As New AgronicaCoreScadenziario.Alert_Entita With {
                .analisi_campione_cod = 0,
                .Appezza = 0,
                .Campo_Cod = 0,
                .COM = "",
                .FOGLIO = 0,
                .ID_Agenda = 0,
                .Id_Imp = 0,
                .NUMERO = 0,
                .Piva = dr("piva"),
                .PivaSuperUser = objParametri.PivaSuperUser,
                .Programmazione_Entita_Cod = 0,
                .PROV = "",
                .Ricetta_Operazione_cod = 0,
                .Sa_Cod = dr("sa_cod"),
                .SEZIONE = "",
                .SUBALTERNO = "",
                .Cod_Contatto = "",
                .TipoEntita_Cod = enum_TipoEntita.Macchina,
                .Analisi_Testata_Cod = 0,
                .PC_Testata_Cod = 0,
                .PUA_Cod = 0,
                .Mac_Cod = dr("mac_cod"),
                .Id_ImpresexParticelle = 0
            }

            Dim ret_Id_Elenco As Integer = -1
            Dim strErr As String = alert_W.Scrivi(id_tipologia, objE, dataScadenza, descr, "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri, note, ret_Id_Elenco)

            If strErr = "" Then
                nImportati += 1
            Else
                msgErr &= strErr & vbCrLf
            End If
        Next

        Return nImportati

    End Function

    Private Function Importa_TaratureUgelli_Modificati(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef msgErr As String) As Integer

        Dim imp_R As New Alert_Importazioni()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        Dim dtModificati As DataTable = imp_R.Leggi_TaratureUgelli_Modificati(objParametri)
        Dim nImportati As Integer = 0

        For Each dr As DataRow In dtModificati.Rows

            Dim marcaModello As String = String.Join(" - ", {dr("ditta_des"), dr("modello")}.Where(Function(s) Not String.IsNullOrEmpty(s))) & " "
            Dim descrizioneMac As String = If(String.IsNullOrEmpty(dr("mac_des")), "", "(" & dr("mac_des") & ") ")
            Dim ultimaTaratura As String = If(IsDate(dr("Validita_Taratura_Inizio")) AndAlso dr("Validita_Taratura_Inizio") <> AGRODATAINIZIO AndAlso dr("Validita_Taratura_Inizio") <> AGRODATAFINE, " eseguita il " & CDate(dr("Validita_Taratura_Inizio")).ToShortDateString() & " ", "")

            Dim descr As String = "Taratura Ugelli della macchina " & marcaModello & descrizioneMac & "di tipo '" & dr("class_desc") & "' " & ultimaTaratura
            'descr &= "con scadenza al " & CDate(dr("Validita_Taratura_Fine")).ToShortDateString()

            Dim dataScadenza As DateTime = dr("Validita_Taratura_Fine")
            Dim id_tipologia As Integer = enum_ID_Area_Tipologia.Taratura_ugelli

            Dim strerr As String = alert_W.ModificaAutomatica(id_tipologia, CInt(dr("id_elenco")), dataScadenza, descr, dr("Note"), dr("ID_Alert_Entita"), objParametri)

            If strerr = "" Then
                nImportati += 1
            Else
                msgErr &= strerr & vbCrLf
            End If
        Next

        Return nImportati

    End Function

    Private Function Importa_TaratureUgelli_Cancellati(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef msgErr As String) As Integer

        Dim imp_R As New Alert_Importazioni()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        Dim dtCancellati As DataTable = imp_R.Leggi_TaratureUgelli_Cancellati(objParametri)
        Dim nImportati As Integer = 0

        For Each dr As DataRow In dtCancellati.Rows

            Dim esito As Boolean = alert_W.Cancella(CInt(dr("id_elenco")), "", objParametri)
            Dim strerr As String = If(esito = True, "", "Errore durante la cancellazione della scadenza " & dr("id_elenco"))

            If strerr = "" Then
                nImportati += 1
            Else
                msgErr &= strerr & vbCrLf
            End If
        Next

        Return nImportati

    End Function

    Private Function Importa_ContrattiAffitto_Nuovi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef msgErr As String) As Integer

        Dim imp_R As New Alert_Importazioni()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        'legge i nuovi record
        Dim dtNuovi As DataTable = imp_R.Leggi_ContrattiAffitto_Nuovi(objParametri)
        Dim nImportati As Integer = 0

        For Each dr As DataRow In dtNuovi.Rows

            Dim entita As String = ""
            Dim sa_cod As Integer = 0
            Dim campo_cod As Integer = 0
            Dim appezza As Integer = 0
            Dim id_reg As Integer = 0
            Dim fabbricato_cod As Integer = 0
            Dim prov As String = ""
            Dim com As String = ""
            Dim sezione As String = ""
            Dim foglio As Integer = 0
            Dim numero As Integer = 0
            Dim subalterno As String = ""

            Dim descr As String = dr("des_lib")

            If IsDate(dr("data_reg")) Then
                descr &= " del " & CDate(dr("data_reg")).ToShortDateString()
            End If

            Dim note As String = ""
            Dim dataScadenza As DateTime = dr("validitaP_al")
            Dim id_tipologia As Integer = enum_ID_Area_Tipologia.Contratti_Affitto

            'istanzia l'oggetto che diventerà il nuovo record
            Dim objE As New AgronicaCoreScadenziario.Alert_Entita With {
                .analisi_campione_cod = 0,
                .Appezza = appezza,
                .Campo_Cod = campo_cod,
                .COM = com,
                .FOGLIO = foglio,
                .ID_Agenda = dr("id_agenda"),
                .Id_Imp = id_reg,
                .NUMERO = numero,
                .Piva = dr("piva"),
                .PivaSuperUser = objParametri.PivaSuperUser,
                .Programmazione_Entita_Cod = 0,
                .PROV = prov,
                .Ricetta_Operazione_cod = 0,
                .Sa_Cod = sa_cod,
                .SEZIONE = sezione,
                .SUBALTERNO = subalterno,
                .Cod_Contatto = "",
                .TipoEntita_Cod = enum_TipoEntita.OperazioneDiAgenda,
                .Analisi_Testata_Cod = 0,
                .PC_Testata_Cod = 0,
                .PUA_Cod = 0,
                .Mac_Cod = 0,
                .Id_ImpresexParticelle = 0
            }

            Dim ret_Id_Elenco As Integer = -1
            Dim strErr As String = alert_W.Scrivi(id_tipologia,
                                                  objE,
                                                  dataScadenza,
                                                  descr,
                                                  "",
                                                  "",
                                                  AGRODATAINIZIO,
                                                  AGRODATAFINE,
                                                  objParametri,
                                                  "",
                                                  ret_Id_Elenco)

            If strErr = "" Then
                nImportati += 1
            Else
                msgErr &= strErr & vbCrLf
            End If
        Next

        Return nImportati

    End Function

    Private Function Importa_ContrattiAffitto_Cancellati(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                         ByRef msgErr As String) As Integer

        Dim imp_R As New Alert_Importazioni()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        'legge i record da cancellare
        Dim dtCancellati As DataTable = imp_R.Leggi_ContrattiAffitto_Cancellati(objParametri)
        Dim nImportati As Integer = 0

        For Each dr As DataRow In dtCancellati.Rows

            'se il record non contiene un allegato viene cancellato, altrimenti imposta la scadenza con AGRODATAFINE
            If dr("allegati_documenti_cod") = 0 Then

                Dim esito As Boolean = alert_W.Cancella(CInt(dr("id_elenco")),
                                                        "",
                                                        objParametri)
                Dim strerr As String = If(esito = True, "", "Errore durante la cancellazione della scadenza " & dr("id_elenco"))

                If strerr = "" Then
                    nImportati += 1
                Else
                    msgErr &= strerr & vbCrLf
                End If

            Else

                Dim descr As String = ""

                Dim dataScadenza As DateTime = AGRODATAFINE
                Dim id_tipologia As Integer = enum_ID_Area_Tipologia.Contratti_Affitto

                Dim strerr As String = alert_W.ModificaAutomatica(id_tipologia,
                                                                  CInt(dr("id_elenco")),
                                                                  dataScadenza,
                                                                  descr,
                                                                  dr("Note"),
                                                                  dr("ID_Alert_Entita"),
                                                                  objParametri,
                                                                  ChkDocumento:=1)

                If strerr = "" Then
                    nImportati += 1
                Else
                    msgErr &= strerr & vbCrLf
                End If

            End If

        Next

        Return nImportati

    End Function

    Private Function Importa_ParticelleImprese_Nuovi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                     ByRef msgErr As String) As Integer

        Dim imp_R As New Alert_Importazioni()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        'legge i nuovi record
        Dim dtNuovi As DataTable = imp_R.Leggi_ParticelleImprese_Nuovi(objParametri)
        Dim nImportati As Integer = 0

        For Each dr As DataRow In dtNuovi.Rows

            Dim entita As String = ""
            Dim sa_cod As Integer = 0
            Dim campo_cod As Integer = 0
            Dim appezza As Integer = 0
            Dim id_reg As Integer = 0
            Dim fabbricato_cod As Integer = 0
            Dim prov As String = dr("PROV")
            Dim com As String = dr("COM")
            Dim sezione As String = dr("sezione")
            Dim foglio As Integer = dr("foglio")
            Dim numero As Integer = dr("numero")
            Dim subalterno As String = dr("SUBALTERNO")

            Dim descr = componiDescrizioneParticella(dr("LOCALITA"),
                                                     dr("COMUNI_PROV"),
                                                     sezione,
                                                     foglio,
                                                     numero,
                                                     subalterno,
                                                     dr("TitoloPossesso"),
                                                     dr("Validita_Inizio"))

            Dim note As String = ""
            Dim dataScadenza As DateTime = dr("Validita_Fine")
            Dim id_tipologia As Integer = enum_ID_Area_Tipologia.Possesso_Particelle

            'istanzia l'oggetto che diventerà il nuovo record
            Dim objE As New AgronicaCoreScadenziario.Alert_Entita With {
                .analisi_campione_cod = 0,
                .Appezza = appezza,
                .Campo_Cod = campo_cod,
                .PROV = "",
                .COM = "",
                .SEZIONE = "",
                .FOGLIO = 0,
                .NUMERO = 0,
                .SUBALTERNO = "",
                .ID_Agenda = 0,
                .Id_Imp = id_reg,
                .Piva = dr("piva"),
                .PivaSuperUser = objParametri.PivaSuperUser,
                .Programmazione_Entita_Cod = 0,
                .Ricetta_Operazione_cod = 0,
                .Sa_Cod = sa_cod,
                .Cod_Contatto = "",
                .TipoEntita_Cod = enum_TipoEntita.Particella_Impresa,
                .Analisi_Testata_Cod = 0,
                .PC_Testata_Cod = 0,
                .PUA_Cod = 0,
                .Mac_Cod = 0,
                .Id_ImpresexParticelle = dr("id")
            }

            Dim ret_Id_Elenco As Integer = -1
            Dim strErr As String = alert_W.Scrivi(id_tipologia,
                                                  objE,
                                                  dataScadenza,
                                                  descr,
                                                  "",
                                                  "",
                                                  AGRODATAINIZIO,
                                                  AGRODATAFINE,
                                                  objParametri,
                                                  "",
                                                  ret_Id_Elenco)

            If strErr = "" Then
                nImportati += 1
            Else
                msgErr &= strErr & vbCrLf
            End If
        Next

        Return nImportati

    End Function

    Private Function Importa_ParticelleImprese_Modificati(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                          ByRef msgErr As String) As Integer

        Dim imp_R As New Alert_Importazioni()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        'legge i record da modificare
        Dim dtModificati As DataTable = imp_R.Leggi_ParticelleImprese_Modificati(objParametri)
        Dim nImportati As Integer = 0

        For Each dr As DataRow In dtModificati.Rows

            Dim descr = componiDescrizioneParticella(dr("LOCALITA"),
                                                     dr("COMUNI_PROV"),
                                                     dr("sezione"),
                                                     dr("foglio"),
                                                     dr("numero"),
                                                     dr("SUBALTERNO"),
                                                     dr("TitoloPossesso"),
                                                     dr("Validita_Inizio"))

            Dim dataScadenza As DateTime = dr("Validita_Fine")
            Dim id_tipologia As Integer = enum_ID_Area_Tipologia.Possesso_Particelle

            Dim strerr As String = alert_W.ModificaAutomatica(id_tipologia,
                                                              CInt(dr("id_elenco")),
                                                              dataScadenza,
                                                              descr,
                                                              dr("Note"),
                                                              dr("ID_Alert_Entita"),
                                                              objParametri)

            If strerr = "" Then
                nImportati += 1
            Else
                msgErr &= strerr & vbCrLf
            End If

        Next

        Return nImportati

    End Function

    Private Function componiDescrizioneParticella(ByVal localita As String,
                                                  ByVal provincia As String,
                                                  ByVal sezione As String,
                                                  ByVal foglio As Integer,
                                                  ByVal numero As Integer,
                                                  ByVal subalterno As String,
                                                  ByVal titoloPossesso As enum_TitoloPossesso,
                                                  ByVal validitaInizio As Object) As String

        Dim descrParticella = ""

        descrParticella = String.Format("Particella {0} ({1}) Sez.{2} Fgl.{3} Num.{4} S.{5} - {6}",
                                        localita,
                                        provincia,
                                        sezione,
                                        foglio,
                                        numero,
                                        subalterno,
                                        titoloPossesso.ToString("G"))

        If IsDate(validitaInizio) AndAlso validitaInizio <> AGRODATAINIZIO Then
            descrParticella += " dal " & CDate(validitaInizio).ToShortDateString()
        End If

        Return descrParticella

    End Function

    Private Function Importa_ParticelleImprese_Cancellati(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                          ByRef msgErr As String) As Integer

        Dim imp_R As New Alert_Importazioni()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        'legge i record da cancellare
        Dim dtCancellati As DataTable = imp_R.Leggi_ParticelleImprese_Cancellati(objParametri)
        Dim nImportati As Integer = 0

        For Each dr As DataRow In dtCancellati.Rows

            'se il record non contiene un allegato viene cancellato, altrimenti imposta la scadenza con AGRODATAFINE
            If dr("allegati_documenti_cod") = 0 Then

                Dim esito As Boolean = alert_W.Cancella(CInt(dr("id_elenco")),
                                                    "",
                                                    objParametri)
                Dim strerr As String = If(esito = True, "", "Errore durante la cancellazione della scadenza " & dr("id_elenco"))

                If strerr = "" Then
                    nImportati += 1
                Else
                    msgErr &= strerr & vbCrLf
                End If

            Else

                Dim descr As String = ""

                Dim dataScadenza As DateTime = AGRODATAFINE
                Dim id_tipologia As Integer = enum_ID_Area_Tipologia.Possesso_Particelle

                Dim strerr As String = alert_W.ModificaAutomatica(id_tipologia,
                                                                  CInt(dr("id_elenco")),
                                                                  dataScadenza,
                                                                  descr,
                                                                  dr("Note"),
                                                                  dr("ID_Alert_Entita"),
                                                                  objParametri,
                                                                  ChkDocumento:=1)

                If strerr = "" Then
                    nImportati += 1
                Else
                    msgErr &= strerr & vbCrLf
                End If

            End If

        Next

        Return nImportati

    End Function

    'Public Function AggiornaTipologiaxIndice(ByVal TipoOperazione As enum_TipoOperazioneDB,
    '                               ByVal piva As String,
    '                                ByVal id_area As Integer,
    '                                ByVal elenco_tipologie As String,
    '                                ByVal id_indice As Integer,
    '                                ByVal titoloindice As String,
    '                                ByVal chkriservato As Integer,
    '                                ByVal chkobbligatorio As Integer,
    '                                ByVal tipocampo As Integer,
    '                                ByVal tipodato As String,
    '                                ByVal elenco_cod As Integer,
    '                                ByVal elenco_val As Integer,
    '                                ByVal validita_inizio As Date,
    '                                ByVal validita_fine As Date,
    '                                ByVal righeInseriteGrid_Dettagli As String,
    '                                ByVal righeModificateGrid_Dettagli As String,
    '                                ByVal righeCancellateGrid_Dettagli As String,
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    '                                ) As Integer


    '    Dim NomeRoutine As String = "AgronicaCoreScadenziario_BIZ.ALERT.AggiornaIndice()"
    '    Dim strErr As String = ""
    '    Dim id_indice_det As Integer = 0
    '    Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
    '    Try

    '        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

    '        Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
    '        Dim objIndice As New AgronicaCoreScadenziario.Alert_Indice_W
    '        Dim objIndiceDettaglio As New AgronicaCoreScadenziario.Alert_Indice_Dettagli_W
    '        Dim ArrayTipologie() As String
    '        Dim bSkip As Boolean = True
    '        Dim bModificaChiavi As Boolean = False

    '        ' forzati
    '        Dim Base, Top As Integer
    '        Dim id_tipologia As Integer = 0
    '        Base = 0
    '        Top = 2000000000

    '        Select Case TipoOperazione

    '            Case enum_TipoOperazioneDB.Scrittura

    '                'Nuovo Indice
    '                id_indice = objSeq.NuovoId_Tabella("Alert_Indice", Base, Top, objParametri)

    '                If chkriservato = 1 Then
    '                    'In caso di indice riservato gestisco il codice negativo
    '                    id_indice = -id_indice
    '                End If

    '                bSkip = False

    '            Case enum_TipoOperazioneDB.Modifica

    '                If CInt(tipocampo) = 0 Then
    '                    objIndiceDettaglio.Cancella(piva, id_indice, 0, objParametri)
    '                End If


    '                objIndice.Cancella(piva, id_indice, objParametri)
    '                bSkip = False

    '                If (chkriservato = 1 And id_indice > 0) Or (chkriservato = 0 And id_indice < 0) Then
    '                    'In caso di indice riservato gestisco il codice negativo
    '                    id_indice = -id_indice

    '                    bModificaChiavi = True

    '                End If



    '            Case enum_TipoOperazioneDB.Cancellazione

    '                'Cancellazione Preventiva
    '                objIndiceDettaglio.Cancella(piva, id_indice, 0, objParametri)
    '                objIndice.Cancella(piva, id_indice, objParametri)
    '                bSkip = True


    '        End Select


    '        If Not bSkip Then


    '            Select Case Trim(elenco_tipologie)
    '                Case ""
    '                    elenco_tipologie = "0" 'Tutte
    '                Case Else

    '            End Select

    '            ArrayTipologie = Split(elenco_tipologie & ",", ",")

    '            For i = 0 To UBound(ArrayTipologie)

    '                If IsNumeric(ArrayTipologie(i)) Then

    '                    id_tipologia = ArrayTipologie(i)

    '                    'Salvataggio Indice
    '                    objIndice.Scrivi(piva, id_area, id_tipologia, id_indice, titoloindice, chkobbligatorio, tipocampo, tipodato, elenco_cod, elenco_val, validita_inizio, validita_fine, objParametri)

    '                End If

    '            Next i

    '            If righeInseriteGrid_Dettagli <> "" And righeInseriteGrid_Dettagli <> "[]" Then

    '                For Each obj As JObject In JArray.Parse(righeInseriteGrid_Dettagli)

    '                    id_indice_det = objSeq.NuovoId_Tabella("Alert_Indice_Dettagli", Base, Top, objParametri)

    '                    objIndiceDettaglio.Scrivi(piva, id_indice, id_indice_det, CStr(obj("Valore")), CDate(obj("Validita_Inizio")), CDate(obj("Validita_Fine")), objParametri)

    '                Next

    '            End If


    '            If righeModificateGrid_Dettagli <> "" And righeModificateGrid_Dettagli <> "[]" Then

    '                For Each obj As JObject In JArray.Parse(righeModificateGrid_Dettagli)

    '                    objIndiceDettaglio.Modifica(piva, CInt(obj("ID_Indice")), CInt(obj("ID_Indice_Det")), CStr(obj("Valore")), CDate(obj("Validita_Inizio")), CDate(obj("Validita_Fine")), objParametri)

    '                Next

    '            End If


    '            If righeCancellateGrid_Dettagli <> "" And righeCancellateGrid_Dettagli <> "[]" Then

    '                For Each obj As JObject In JArray.Parse(righeCancellateGrid_Dettagli)

    '                    objIndiceDettaglio.Cancella(piva, CInt(obj("ID_Indice")), CInt(obj("ID_Indice_Det")), objParametri)

    '                Next

    '            End If



    '        End If

    '        If bModificaChiavi Then

    '            'Aggiornamento chiavi tabelle collegate:
    '            '- Alert_Indice_Dettagli
    '            '- Alert_ExtitaxIndici
    '            objIndice.ModificaChiavi("", id_indice, -id_indice, objParametri)

    '        End If



    '        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

    '    Catch ex As Exception
    '        If Not objParametri.objTransazione Is Nothing Then
    '            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
    '        End If

    '        strErr = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, strErr)

    '    Finally
    '        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

    '    End Try

    '    Return id_indice

    'End Function

    Public Function AggiornaIndice(ByVal TipoOperazione As enum_TipoOperazioneDB,
                                   ByVal piva As String,
                                   ByVal id_indice As Integer,
                                   ByVal titoloindice As String,
                                   ByVal chkriservato As Integer,
                                   ByVal chkobbligatorio As Integer,
                                   ByVal chkindice_speciale As Integer,
                                   ByVal tipocampo As Integer,
                                   ByVal tipodato As String,
                                   ByVal elenco_tipo As Integer,
                                   ByVal elenco_cod As Integer,
                                   ByVal elenco_val As String,
                                   ByVal validita_inizio As Date,
                                   ByVal validita_fine As Date,
                                   ByVal righeInseriteGrid_Dettagli As String,
                                   ByVal righeModificateGrid_Dettagli As String,
                                   ByVal righeCancellateGrid_Dettagli As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   Optional elenco_cod_string As String = ""
                                   ) As Integer


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_BIZ.ALERT.AggiornaIndice()"
        Dim strErr As String = ""
        Dim id_indice_det As Integer = 0
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim objIndice As New AgronicaCoreScadenziario.Alert_Indice_W
            Dim objIndiceDettaglio As New AgronicaCoreScadenziario.Alert_Indice_Dettagli_W
            Dim PivaSuperUser = objParametri.PivaSuperUser
            Dim Validita_Inizio_Dettaglio As Date = AGRODATAINIZIO
            Dim Validita_Fine_Dettaglio As Date = AGRODATAFINE


            ' forzati
            Dim Base, Top As Integer
            Dim id_tipologia As Integer = 0
            Dim ChkObbligatorio_Modificato As Boolean = False
            Base = 0
            Top = 2000000000

            piva = "" 'Non Gestito

            Select Case TipoOperazione

                Case enum_TipoOperazioneDB.Scrittura

                    'Nuovo Indice
                    id_indice = objSeq.NuovoId_Tabella("Alert_Indice", Base, Top, objParametri)

                    If chkriservato = 1 Then
                        'In caso di indice riservato gestisco il codice negativo
                        id_indice = -id_indice
                    End If

                    'Salvataggio Indice
                    objIndice.Scrivi(piva, id_indice, titoloindice, chkobbligatorio, chkindice_speciale, tipocampo, tipodato, elenco_tipo, elenco_cod, elenco_val, validita_inizio, validita_fine, objParametri, elenco_cod_string:=elenco_cod_string)


                Case enum_TipoOperazioneDB.Modifica

                    If CInt(tipocampo) = 0 Then
                        objIndiceDettaglio.Cancella(piva, id_indice, 0, objParametri)
                    End If


                    If chkobbligatorio = 1 Then

                        'Lettura dell'indice per verificare se prima non era obbligatorio
                        Dim objIndice_R As New AgronicaCoreScadenziario.Alert_Indice_R
                        Dim DT As New DataTable

                        DT = objIndice_R.Leggi("", id_indice, objParametri, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta)

                        If DT(0).Item("ChkObbligatorio") = 0 Then

                            'Forzo la relazione di obbligatorietà
                            objIndice.ModificaChkObbligatorio("", id_indice, objParametri)

                        End If

                    End If



                    'Modifica Indice
                    objIndice.Modifica("", id_indice, titoloindice, chkobbligatorio, chkindice_speciale, tipocampo, tipodato, elenco_tipo, elenco_cod, elenco_val, validita_inizio, validita_fine, objParametri, elenco_cod_string:=elenco_cod_string)


                    If (chkriservato = 1 And id_indice > 0) Or (chkriservato = 0 And id_indice < 0) Then
                        'In caso di indice riservato gestisco il codice negativo
                        id_indice = -id_indice

                        'Aggiornamento chiavi tabelle collegate:
                        '- Alert_Indice
                        '- Alert_Indice_Dettagli
                        '- Alert_ExtitaxIndici
                        objIndice.ModificaChiavi("", id_indice, -id_indice, objParametri)

                    End If





                Case enum_TipoOperazioneDB.Cancellazione

                    'Cancellazione
                    objIndiceDettaglio.Cancella("", id_indice, 0, objParametri)
                    objIndice.Cancella("", id_indice, objParametri)



            End Select



            If righeInseriteGrid_Dettagli <> "" And righeInseriteGrid_Dettagli <> "[]" Then

                For Each obj As JObject In JArray.Parse(righeInseriteGrid_Dettagli)

                    id_indice_det = objSeq.NuovoId_Tabella("Alert_Indice_Dettagli", Base, Top, objParametri)

                    If IsDate(obj("Validita_Inizio").ToString) Then
                        Validita_Inizio_Dettaglio = CDate(obj("Validita_Inizio").ToString)
                    Else
                        Validita_Inizio_Dettaglio = AGRODATAINIZIO
                    End If

                    If IsDate(obj("Validita_Fine").ToString) Then
                        Validita_Fine_Dettaglio = CDate(obj("Validita_Fine").ToString)
                    Else
                        Validita_Fine_Dettaglio = AGRODATAFINE
                    End If

                    objIndiceDettaglio.Scrivi(piva, id_indice, id_indice_det, CStr(obj("Valore")), Validita_Inizio_Dettaglio, Validita_Fine_Dettaglio, objParametri)

                Next

            End If


            If righeModificateGrid_Dettagli <> "" And righeModificateGrid_Dettagli <> "[]" Then

                For Each obj As JObject In JArray.Parse(righeModificateGrid_Dettagli)

                    If IsDate(obj("Validita_Inizio").ToString) Then
                        Validita_Inizio_Dettaglio = CDate(obj("Validita_Inizio").ToString)
                    Else
                        Validita_Inizio_Dettaglio = AGRODATAINIZIO
                    End If

                    If IsDate(obj("Validita_Fine").ToString) Then
                        Validita_Fine_Dettaglio = CDate(obj("Validita_Fine").ToString)
                    Else
                        Validita_Fine_Dettaglio = AGRODATAFINE
                    End If

                    objIndiceDettaglio.Modifica("", CInt(obj("ID_Indice")), CInt(obj("ID_Indice_Det")), CStr(obj("Valore")), Validita_Inizio_Dettaglio, Validita_Fine_Dettaglio, objParametri)

                Next

            End If


            If righeCancellateGrid_Dettagli <> "" And righeCancellateGrid_Dettagli <> "[]" Then

                For Each obj As JObject In JArray.Parse(righeCancellateGrid_Dettagli)

                    objIndiceDettaglio.Cancella("", CInt(obj("ID_Indice")), CInt(obj("ID_Indice_Det")), objParametri)

                Next

            End If





            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        Catch ex As Exception
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            strErr = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, strErr)

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return id_indice

    End Function

    Public Function Scrivi_QDC(ByVal objParametri_Server As AgronicaCoreParametri,
                               ByVal objParametri_Utenti As AgronicaCoreParametri,
                               ByRef Piva As String,
                               ByRef Veg_Cod As Integer,
                               ByRef Id_Area As Integer,
                               ByRef Id_Tipologia As Integer,
                               ByRef Anno As Integer,
                               ByRef Percorso As String,
                               ByRef Nome_File As String,
                               ByVal Id_Indice_Veg_Cod As Integer,
                               ByVal Id_Indice_Anno As Integer) As String


        Dim NomeRoutine As String = "AgronicaCoreWS.ALERT.Scrivi_QDC()"

        Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim objScriviElenco As New AgronicaCoreScadenziario.Alert_Elenco_W
        Dim objScriviEntita As New AgronicaCoreScadenziario.Alert_Entita_W
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        Try


            'Definizione Indici
            Dim EntitaxIndici As JArray
            Dim strEntitaxIndici As String = ""
            Dim dtIndici As New DataTable


            dtIndici.Columns.Add(New DataColumn("ID_Indice", GetType(Integer)))
            dtIndici.Columns.Add(New DataColumn("ID_Indice_Det", GetType(Integer)))
            dtIndici.Columns.Add(New DataColumn("Elenco_Val", GetType(String)))
            dtIndici.Columns.Add(New DataColumn("Valore_Des", GetType(String)))
            dtIndici.Columns.Add(New DataColumn("TipoCampo", GetType(String)))

            Dim dSpecie As DataRow

            dSpecie = dtIndici.NewRow
            dSpecie("ID_Indice") = Id_Indice_Veg_Cod
            dSpecie("ID_Indice_Det") = 0
            dSpecie("Elenco_Val") = Veg_Cod
            dSpecie("Valore_Des") = ""
            dSpecie("TipoCampo") = 2


            dtIndici.Rows.Add(dSpecie)

            Dim dAnno As DataRow

            dAnno = dtIndici.NewRow
            dAnno("ID_Indice") = Id_Indice_Anno
            dAnno("ID_Indice_Det") = 0
            dAnno("Elenco_Val") = 0
            dAnno("Valore_Des") = Anno
            dAnno("TipoCampo") = 0


            dtIndici.Rows.Add(dAnno)


            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            strEntitaxIndici = JsonConvert.SerializeObject(dtIndici, Formatting.None, serializerSettings)

            EntitaxIndici = JArray.Parse(strEntitaxIndici)



            Dim Analisi_Campione_Cod As Integer = 0
            Dim Campo_Cod As Integer = 0
            Dim COM As Integer = 0
            Dim FOGLIO As Integer = 0
            Dim ID_Agenda As Integer = 0
            Dim Id_Imp As Integer = 0
            Dim NUMERO As Integer = 0
            Dim Programmazione_Entita_Cod As Integer = 0
            Dim PROV As Integer = 0
            Dim Ricetta_Operazione_cod As Integer = 0
            Dim SEZIONE As Integer = 0
            Dim SUBALTERNO As Integer = 0
            Dim TipoEntita_Cod As Integer = 0

            Dim Sa_Cod As Integer = 0
            Dim Appezza As Integer = 0
            Dim Analisi_Testata_Cod As Integer = 0
            Dim Pua_Cod As Integer = 0
            Dim PC_Testata_Cod As Integer = 0
            Dim Mac_Cod As Integer = 0
            Dim Cod_Contatto As String = ""
            Dim Richiesta_Cod As Integer = 0
            Dim Id_Schema_Template As Integer = 0
            Dim SalvaAllegato As Integer = 1
            Dim chkdocumento As Integer = 0

            Dim Id_Alert_Entita As Integer = 0
            Dim Id_Elenco As Integer = 0
            Dim Allegati_Documenti_Cod As Integer = 0

            Dim strerr As String = ""

            Dim elenco_des As String = ""



            chkdocumento = 1 'Documento
            'elenco_des = Gias.Documento


            '========================================================================================================================
            'Campi non più usati
            '------------------------------------------------------------------------------------------------------------------------
            Dim data_rilascio As Date = AGRODATAINIZIO
            Dim ente_rilascio As String = ""
            '========================================================================================================================
            Dim Data_scadenza As Date = AGRODATAFINE



            Dim Descrizione_scadenza As String = ""
            Dim Note As String = ""
            Dim Allegati_Documenti_numero As String = ""


            Dim validazione_flag As String = 0
            Dim username_upload As String = objParametri_Utenti.UtenteUsername
            Dim data_upload As DateTime = Now


            If Veg_Cod <> 0 Then
                Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                Descrizione_scadenza = objSpecie.VegDes_from_VegCod(Veg_Cod, objParametri_Server)
            End If

            'identifico il tipoentita_cod
            Dim objxTipoInput As New AgronicaCoreScadenziario.Tipo_Entita_Chiavi_R
            TipoEntita_Cod = objxTipoInput.GetTipoEntitaCod(Id_Tipologia, objParametri_Server,
                                                            Piva, Sa_Cod, Appezza, Analisi_Testata_Cod,
                                                            PC_Testata_Cod, Pua_Cod,
                                                            Richiesta_Cod, ID_Agenda, Ricetta_Operazione_cod)


            'Preparazione Oggetto Entità
            Dim objE As New AgronicaCoreScadenziario.Alert_Entita With {
                .ChkDocumento = chkdocumento,
                .analisi_campione_cod = Analisi_Campione_Cod,
                .Appezza = Appezza,
                .Campo_Cod = Campo_Cod,
                .COM = COM,
                .FOGLIO = FOGLIO,
                .ID_Agenda = ID_Agenda,
                .Id_Imp = Id_Imp,
                .NUMERO = NUMERO,
                .Piva = Piva,
                .PivaSuperUser = objParametri_Server.PivaSuperUser,
                .Programmazione_Entita_Cod = Programmazione_Entita_Cod,
                .PROV = PROV,
                .Ricetta_Operazione_cod = Ricetta_Operazione_cod,
                .Sa_Cod = Sa_Cod,
                .SEZIONE = SEZIONE,
                .SUBALTERNO = SUBALTERNO,
                .Cod_Contatto = Cod_Contatto,
                .TipoEntita_Cod = TipoEntita_Cod,
                .Analisi_Testata_Cod = Analisi_Testata_Cod,
                .PC_Testata_Cod = PC_Testata_Cod,
                .PUA_Cod = Pua_Cod,
                .Mac_Cod = Mac_Cod,
                .Richiesta_Cod = Richiesta_Cod,
                .Id_Schema_Template = Id_Schema_Template
            }


            'Determinazione Id_Alert_Entita
            Dim objElenco As New AgronicaCoreScadenziario.Alert_Elenco_R
            Dim dt As DataTable = objElenco.Leggi_x_QDC(Piva, Id_Tipologia, Id_Indice_Veg_Cod, Veg_Cod, Id_Indice_Anno, Anno, objParametri_Server)


            Dim ElencoFileDaEliminareDaFS As New List(Of String)

            If dt.Rows.Count > 0 Then

                Id_Alert_Entita = dt(0)("Id_Alert_Entita")
                Id_Elenco = dt(0)("Id_Elenco")
                Allegati_Documenti_Cod = dt(0)("Allegati_Documenti_Cod")

            Else

                Id_Alert_Entita = 0
                Id_Elenco = 0
                Allegati_Documenti_Cod = 0

            End If



            Dim fileByteArray As Byte() = Nothing
            Try
                'Salvataggio su FS
                fileByteArray = My.Computer.FileSystem.ReadAllBytes(Percorso & Nome_File)
            Catch ex As Exception
                ' FileNonTrovati &= "[" & ex.Message & "]. <br>"
            End Try


            'SE NE DEVO CREARE UNA NUOVA
            If Id_Alert_Entita <= 0 Then


                'Dim path As String = "" 'ViewState("PATH")
                Dim ret_Id_Elenco As Integer = -1


                'scrivo 
                strerr = alert_W.Scrivi(Id_Tipologia, objE, Data_scadenza, Descrizione_scadenza,
                                        Nome_File, Percorso,
                                        AGRODATAINIZIO, AGRODATAFINE,
                                        objParametri_Server, Note, ret_Id_Elenco, EntitaxIndici,
                                        ente_rilascio, validazione_flag,
                                        username_upload, data_upload,
                                        Nothing, Allegati_Documenti_numero, SalvaAllegato, 0, fileByteArray)

                If strerr <> "" Then
                    Return strerr
                End If

                Id_Elenco = ret_Id_Elenco

            Else

                strerr = alert_W.Modifica(Id_Tipologia, objE, Id_Elenco, Id_Alert_Entita, Allegati_Documenti_Cod, Data_scadenza, Descrizione_scadenza, Nome_File, Percorso, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server, Note, EntitaxIndici, validazione_flag,
                                                  username_upload, data_upload, Nothing, True, Allegati_Documenti_numero, SalvaAllegato, fileByteArray)

                'If strerr <> "" Then
                '    r.Errore = strerr
                '    Return r
                'End If

            End If

            'ELiminazione PDF
            My.Computer.FileSystem.DeleteFile(Percorso & Nome_File)

            'r.RispostaStringa = String.Format(Gias.SalvataggioElemXCorretto, elenco_des)
            'r.RispostaOK = True

        Catch ex As Exception
            'r.Errore = ex.Message
            Return ex.Message
        End Try

        Return ""

    End Function



    Public Function Scrivi_GlobalGap(ByVal objParametri_Server As AgronicaCoreParametri,
                                     ByVal objParametri_Utenti As AgronicaCoreParametri,
                                     ByVal Piva As String,
                                     ByVal Id_Tipologia As Integer,
                                     ByVal Codice As String,
                                     ByVal Percorso As String,
                                     ByVal Nome_File As String,
                                     ByVal fileByteArray As Byte(),
                                     ByVal Id_Indice_Codice As Integer,
                                     ByVal Data_Upload As DateTime,
                                     ByVal Descrizione_scadenza As String,
                                     ByVal Id_Indice_Anno As Integer,
                                     ByVal Anno As String) As String


        Dim NomeRoutine As String = "AgronicaCoreWS.ALERT.Scrivi_GlobalGap()"

        Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim objScriviElenco As New AgronicaCoreScadenziario.Alert_Elenco_W
        Dim objScriviEntita As New AgronicaCoreScadenziario.Alert_Entita_W
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        Try


            'Definizione Indici
            Dim EntitaxIndici As JArray
            Dim strEntitaxIndici As String = ""
            Dim dtIndici As New DataTable


            dtIndici.Columns.Add(New DataColumn("ID_Indice", GetType(Integer)))
            dtIndici.Columns.Add(New DataColumn("ID_Indice_Det", GetType(Integer)))
            dtIndici.Columns.Add(New DataColumn("Elenco_Val", GetType(String)))
            dtIndici.Columns.Add(New DataColumn("Valore_Des", GetType(String)))
            dtIndici.Columns.Add(New DataColumn("TipoCampo", GetType(String)))

            Dim dCodice As DataRow

            dCodice = dtIndici.NewRow
            dCodice("ID_Indice") = Id_Indice_Codice
            dCodice("ID_Indice_Det") = 0
            dCodice("Elenco_Val") = 0
            dCodice("Valore_Des") = Codice
            dCodice("TipoCampo") = 0
            dtIndici.Rows.Add(dCodice)


            Dim dAnno As DataRow

            dAnno = dtIndici.NewRow
            dAnno("ID_Indice") = Id_Indice_Anno
            dAnno("ID_Indice_Det") = 0
            dAnno("Elenco_Val") = 0
            dAnno("Valore_Des") = Anno
            dAnno("TipoCampo") = 0
            dtIndici.Rows.Add(dAnno)

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            strEntitaxIndici = JsonConvert.SerializeObject(dtIndici, Formatting.None, serializerSettings)

            EntitaxIndici = JArray.Parse(strEntitaxIndici)



            Dim Analisi_Campione_Cod As Integer = 0
            Dim Campo_Cod As Integer = 0
            Dim COM As Integer = 0
            Dim FOGLIO As Integer = 0
            Dim ID_Agenda As Integer = 0
            Dim Id_Imp As Integer = 0
            Dim NUMERO As Integer = 0
            Dim Programmazione_Entita_Cod As Integer = 0
            Dim PROV As Integer = 0
            Dim Ricetta_Operazione_cod As Integer = 0
            Dim SEZIONE As Integer = 0
            Dim SUBALTERNO As Integer = 0
            Dim TipoEntita_Cod As Integer = 0

            Dim Sa_Cod As Integer = 0
            Dim Appezza As Integer = 0
            Dim Analisi_Testata_Cod As Integer = 0
            Dim Pua_Cod As Integer = 0
            Dim PC_Testata_Cod As Integer = 0
            Dim Mac_Cod As Integer = 0
            Dim Cod_Contatto As String = ""
            Dim Richiesta_Cod As Integer = 0
            Dim Id_Schema_Template As Integer = 0
            Dim chkdocumento As Integer = 0

            Dim Id_Alert_Entita As Integer = 0
            Dim Id_Elenco As Integer = 0
            Dim Allegati_Documenti_Cod As Integer = 0
            Dim SalvaAllegato As Integer = 0 'File System Default
            Dim strerr As String = ""
            Dim elenco_des As String = ""

            chkdocumento = 1 'Documento
            'elenco_des = Gias.Documento


            '========================================================================================================================
            'Campi non più usati
            '------------------------------------------------------------------------------------------------------------------------
            Dim data_rilascio As Date = AGRODATAINIZIO
            Dim ente_rilascio As String = ""
            '========================================================================================================================
            Dim Data_scadenza As Date = AGRODATAFINE
            Dim Note As String = ""
            Dim Allegati_Documenti_numero As String = ""
            Dim Allegati_Documenti_NomeFile As String = ""

            Dim validazione_flag As String = 0
            Dim username_upload As String = objParametri_Utenti.UtenteUsername


            'identifico il tipoentita_cod
            Dim objxTipoInput As New AgronicaCoreScadenziario.Tipo_Entita_Chiavi_R
            TipoEntita_Cod = objxTipoInput.GetTipoEntitaCod(Id_Tipologia, objParametri_Server,
                                                            Piva, Sa_Cod, Appezza, Analisi_Testata_Cod,
                                                            PC_Testata_Cod, Pua_Cod,
                                                            Richiesta_Cod, ID_Agenda, Ricetta_Operazione_cod)


            'Preparazione Oggetto Entità
            Dim objE As New AgronicaCoreScadenziario.Alert_Entita With {
                .ChkDocumento = chkdocumento,
                .analisi_campione_cod = Analisi_Campione_Cod,
                .Appezza = Appezza,
                .Campo_Cod = Campo_Cod,
                .COM = COM,
                .FOGLIO = FOGLIO,
                .ID_Agenda = ID_Agenda,
                .Id_Imp = Id_Imp,
                .NUMERO = NUMERO,
                .Piva = Piva,
                .PivaSuperUser = objParametri_Server.PivaSuperUser,
                .Programmazione_Entita_Cod = Programmazione_Entita_Cod,
                .PROV = PROV,
                .Ricetta_Operazione_cod = Ricetta_Operazione_cod,
                .Sa_Cod = Sa_Cod,
                .SEZIONE = SEZIONE,
                .SUBALTERNO = SUBALTERNO,
                .Cod_Contatto = Cod_Contatto,
                .TipoEntita_Cod = TipoEntita_Cod,
                .Analisi_Testata_Cod = Analisi_Testata_Cod,
                .PC_Testata_Cod = PC_Testata_Cod,
                .PUA_Cod = Pua_Cod,
                .Mac_Cod = Mac_Cod,
                .Richiesta_Cod = Richiesta_Cod,
                .Id_Schema_Template = Id_Schema_Template
            }


            'Determinazione Id_Alert_Entita
            Dim objElenco As New AgronicaCoreScadenziario.Alert_Elenco_R
            Dim dt As DataTable = objElenco.Leggi_x_GlobalGAP(Piva, Id_Tipologia, Id_Indice_Codice, Codice, objParametri_Server)


            Dim ElencoFileDaEliminareDaFS As New List(Of String)

            If dt.Rows.Count > 0 Then

                Id_Alert_Entita = dt(0)("Id_Alert_Entita")
                Id_Elenco = dt(0)("Id_Elenco")
                Allegati_Documenti_Cod = dt(0)("Allegati_Documenti_Cod")
                Allegati_Documenti_NomeFile = dt(0)("Allegati_Documenti_NomeFile")

            Else

                Id_Alert_Entita = 0
                Id_Elenco = 0
                Allegati_Documenti_Cod = 0

            End If


            Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim DTSalvaAllegato As New DataTable


            DTSalvaAllegato = objImpostazioni.Leggi2(2, objParametri_Server.SuperUserUsername, enum_Impostazioni_Utenti.SUPERUSER_DOCUMENTALE_SALVA_ALLEGATO_SU_DB, "", "", objParametri_Utenti)

            If Not IsNothing(DTSalvaAllegato) AndAlso DTSalvaAllegato.Rows.Count > 0 Then
                SalvaAllegato = CInt(DTSalvaAllegato.Rows(0).Item("Impostazione_Valore_1"))
            End If


            'SE NE DEVO CREARE UNA NUOVA
            If Id_Alert_Entita <= 0 Then


                'Dim path As String = "" 'ViewState("PATH")
                Dim ret_Id_Elenco As Integer = -1

                'scrivo 
                strerr = alert_W.Scrivi(Id_Tipologia, objE, Data_scadenza, Descrizione_scadenza,
                                        Nome_File, Percorso,
                                        AGRODATAINIZIO, AGRODATAFINE,
                                        objParametri_Server, Note, ret_Id_Elenco, EntitaxIndici,
                                        ente_rilascio, validazione_flag,
                                        username_upload, Data_Upload,
                                        Nothing, Allegati_Documenti_numero, SalvaAllegato, 0, fileByteArray)

                If strerr <> "" Then
                    Return strerr
                End If

                Id_Elenco = ret_Id_Elenco

            Else

                strerr = alert_W.Modifica(Id_Tipologia, objE, Id_Elenco, Id_Alert_Entita, Allegati_Documenti_Cod, Data_scadenza, Descrizione_scadenza, Nome_File, Percorso, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server, Note, EntitaxIndici, validazione_flag,
                                                  username_upload, Data_Upload, Nothing, True, Allegati_Documenti_numero, SalvaAllegato, fileByteArray)


                If SalvaAllegato = 0 And Trim(Allegati_Documenti_NomeFile) <> "" Then

                    ' il file esportato viene eliminato dal disco

                    'Lettura Categoria Documento
                    Dim objTipo As New AgronicaCoreScadenziario.Alert_Tipologia_R
                    Dim dtTipologia As DataTable = objTipo.Leggi_CatCod(Id_Tipologia, objParametri_Server)

                    'Controllo Esistenza Sottocartella
                    If dtTipologia.Rows.Count > 0 Then

                        If Not IsDBNull(dtTipologia.Rows(0).Item("Sottocartella")) Then
                            If dtTipologia.Rows(0).Item("Sottocartella") <> "" Then
                                Percorso = FileSystemHelper.AggiungiSlashSeNonEsiste(Percorso) & FileSystemHelper.AggiungiSlashSeNonEsiste(dtTipologia.Rows(0).Item("Sottocartella"))
                            End If
                        End If
                    End If

                    Dim outputFilePath As String = Path.Combine(Percorso, Allegati_Documenti_NomeFile)
                    System.IO.File.Delete(outputFilePath)

                End If


            End If


        Catch ex As Exception
            'r.Errore = ex.Message
            Return ex.Message
        End Try

        Return ""

    End Function


    Public Function Salvataggio_Allegato_Documentale(ByVal Piva As String,
                                                    ByVal Id_Area As Integer,
                                                    ByVal Id_Tipologia As Integer,
                                                    ByVal Nome_File As String,
                                                    ByVal File_Allegato As String,
                                                    ByVal Username_Upload As String,
                                                    ByVal Data_scadenza As Date,
                                                    ByVal Descrizione_Scadenza As String,
                                                    ByVal EntitaxIndici As JArray,
                                                    ByVal Richiesta_Cod As Integer,
                                                    ByVal Id_Agenda As Integer,
                                                    ByVal objParametri_Server As AgronicaCoreParametri,
                                                    ByVal objParametri_Utenti As AgronicaCoreParametri) As String

        'Questa funzione gestisce il salvataggio di un allegato sul documentale senza passare dall'interfaccia

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_BIZ.ALERT.Salvataggio_Allegato_Documentale()"

        Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim objScriviElenco As New AgronicaCoreScadenziario.Alert_Elenco_W
        Dim objScriviEntita As New AgronicaCoreScadenziario.Alert_Entita_W
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W
        Dim strerr As String = String.Empty

        Try

            Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim DTSalvaAllegato As New DataTable
            Dim SalvaAllegato As Integer = 0 'File System Default

            DTSalvaAllegato = objImpostazioni.Leggi2(2, objParametri_Server.SuperUserUsername, enum_Impostazioni_Utenti.SUPERUSER_DOCUMENTALE_SALVA_ALLEGATO_SU_DB, "", "", objParametri_Utenti)


            If Not IsNothing(DTSalvaAllegato) AndAlso DTSalvaAllegato.Rows.Count > 0 Then
                SalvaAllegato = CInt(DTSalvaAllegato.Rows(0).Item("Impostazione_Valore_1"))
            End If

            Dim objE As New AgronicaCoreScadenziario.Alert_Entita

            Dim Sa_Cod As Integer = 0
            Dim appezza As Integer = 0
            Dim Cod_Contatto As String = ""
            Dim Analisi_Testata_Cod As Integer = 0
            Dim PC_Testata_Cod As Integer = 0
            Dim Pua_Cod As Integer = 0
            Dim Id_Schema_Template As Integer = 0
            Dim ID_App As String = ""
            Dim ChkStorico As Integer = 0


            Dim Mac_Cod As Integer = 0



            '========================================================================================================================
            'Campi non più usati
            '------------------------------------------------------------------------------------------------------------------------
            Dim data_rilascio As Date = AGRODATAINIZIO
            Dim ente_rilascio As String = ""
            '========================================================================================================================

            If String.IsNullOrEmpty(Descrizione_Scadenza) Then
                Dim ObjTipologia = New Alert_Tipologia_R
                Dim dtTipologia As DataTable = ObjTipologia.Leggi(Id_Area, Id_Tipologia, "", False, objParametri_Server)

                If Not IsNothing(dtTipologia) AndAlso dtTipologia.Rows.Count = 1 Then
                    Descrizione_Scadenza = dtTipologia.Rows(0)("Nome")
                End If

            End If

            Dim Allegati_Documenti_numero As String = ""

            Dim validazione_flag As String = "0"

            Dim Note As String = ""

            Dim data_upload As DateTime = Now

            'Determino se è una scadenza o un documento
            Dim chkdocumento As Integer = 0

            If Not IsNothing(Nome_File) AndAlso Nome_File <> "" Then
                chkdocumento = 1 'Documento
                If Data_scadenza <> AGRODATAFINE Then
                    chkdocumento = 2 'Hybrid
                End If
            Else
                chkdocumento = 0 'Scadenza
            End If


            Dim Analisi_Campione_Cod As Integer = 0
            Dim Campo_Cod As Integer = 0
            Dim COM As Integer = 0
            Dim FOGLIO As Integer = 0
            Dim Id_Imp As Integer = 0
            Dim NUMERO As Integer = 0
            Dim Programmazione_Entita_Cod As Integer = 0
            Dim PROV As Integer = 0
            Dim Ricetta_Operazione_cod As Integer = 0
            Dim SEZIONE As Integer = 0
            Dim SUBALTERNO As Integer = 0
            Dim TipoEntita_Cod As Integer = 0


            Dim LeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
            Dim Percorso As String = dt_Conf.Rows(0).Item("Valore")


            'identifico il tipoentita_cod
            Dim objxTipoInput As New AgronicaCoreScadenziario.Tipo_Entita_Chiavi_R
            TipoEntita_Cod = objxTipoInput.GetTipoEntitaCod(Id_Tipologia, objParametri_Server,
                                                            Piva, Sa_Cod, appezza, Analisi_Testata_Cod,
                                                            PC_Testata_Cod, Pua_Cod,
                                                            Richiesta_Cod, Id_Agenda, Ricetta_Operazione_cod)


            'Preparazione Oggetto Entità
            objE = New AgronicaCoreScadenziario.Alert_Entita With {
                    .ChkDocumento = chkdocumento,
                    .ChkStorico = ChkStorico,
                    .analisi_campione_cod = Analisi_Campione_Cod,
                    .Appezza = appezza,
                    .Campo_Cod = Campo_Cod,
                    .COM = COM,
                    .FOGLIO = FOGLIO,
                    .ID_Agenda = Id_Agenda,
                    .Id_Imp = Id_Imp,
                    .NUMERO = NUMERO,
                    .Piva = Piva,
                    .PivaSuperUser = objParametri_Server.PivaSuperUser,
                    .Programmazione_Entita_Cod = Programmazione_Entita_Cod,
                    .PROV = PROV,
                    .Ricetta_Operazione_cod = Ricetta_Operazione_cod,
                    .Sa_Cod = Sa_Cod,
                    .SEZIONE = SEZIONE,
                    .SUBALTERNO = SUBALTERNO,
                    .Cod_Contatto = Cod_Contatto,
                    .TipoEntita_Cod = TipoEntita_Cod,
                    .Analisi_Testata_Cod = Analisi_Testata_Cod,
                    .PC_Testata_Cod = PC_Testata_Cod,
                    .PUA_Cod = Pua_Cod,
                    .Mac_Cod = Mac_Cod,
                    .Richiesta_Cod = Richiesta_Cod,
                    .Id_Schema_Template = Id_Schema_Template
                    }


            Dim ret_Id_Elenco As Integer = -1

            'scrivo 
            strerr = alert_W.Scrivi(Id_Tipologia, objE, Data_scadenza, Descrizione_Scadenza,
                                                      Nome_File, Percorso,
                                                      AGRODATAINIZIO, AGRODATAFINE,
                                                      objParametri_Server, Note, ret_Id_Elenco, EntitaxIndici,
                                                      ente_rilascio, validazione_flag,
                                                      Username_Upload, data_upload,
                                                      File_Allegato, Allegati_Documenti_numero, SalvaAllegato, ID_App)


        Catch ex As Exception
            strerr = ex.Message
        End Try

        Return strerr

    End Function

    Private Shared Sub Chiama_Scrivi_Log_Contatti(piva As String, cod_contatto As String, Note As String, objParametri_Server As AgronicaCoreParametri, Optional ByVal origine As enum_SistemiEsterni = enum_SistemiEsterni.gias)
        Dim objContatti As New Contatti_R
        Dim objLogContatti As New Agronica_Log_Contatti_W

        Dim contatto = objContatti.LeggiContattoSpecifico(piva, cod_contatto, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server).Rows(0)
        Dim Sa_Cod = contatto.Item("Sa_Cod")
        Dim Id_CF = contatto.Item("Id_CF")
        Dim Contatto_Des = If(Id_CF = 0, contatto.Item("Cognome") & " " & contatto.Item("Nome"), contatto.Item("Rag_Soc"))

        objLogContatti.Scrivi(enum_TipoOperazioneDB.Modifica, piva, cod_contatto, Sa_Cod, Id_CF, Contatto_Des, Note, enum_Id_Servizio.GiasOnline, objParametri_Server, "", origine)
    End Sub

    Public Function Salva_Patentino(Piva As String,
                                    Cod_Contatto As String,
                                    ID_Tipologia As enum_ID_Area_Tipologia,
                                    Num_Documento As String,
                                    Ente_Rilascio As String,
                                    Data_Rilascio As Date,
                                    Descrizione As String,
                                    Data_Scadenza As Date,
                                    Nome_File As String,
                                    File_Allegato As String,
                                    ID_Elenco As Integer,
                                    ID_Alert_Entita As Integer,
                                    Allegati_Documenti_Cod As Integer,
                                    objParametri_Server As AgronicaCoreParametri,
                                    objParametri_Utenti As AgronicaCoreParametri,
                                        Optional ByRef New_ID_Elenco As Integer = -1,
                                        Optional ByVal Note_Log As String = "Edit Contatti",
                                        Optional ByVal Origine As enum_SistemiEsterni = enum_SistemiEsterni.gias
                                    ) As String

        Dim res As String = ""

        Try

            'Determino se è una scadenza o un documento
            Dim ChkDocumento As Integer = 0
            If Not IsNothing(Nome_File) AndAlso Nome_File <> "" Then
                ChkDocumento = 1 'Documento
                If Data_Scadenza <> AGRODATAFINE Then
                    ChkDocumento = 2 'Hybrid
                End If
            Else
                ChkDocumento = 0 'Scadenza
            End If

            '----------------------------
            '    OGGETTO ALERT ENTITA
            '----------------------------
            Dim objE As New Alert_Entita With {
                .Piva = Piva,
                .PivaSuperUser = objParametri_Server.PivaSuperUser,
                .Cod_Contatto = Cod_Contatto,
                .TipoEntita_Cod = enum_TipoEntita.Contatto,
                .ChkDocumento = ChkDocumento
            }

            '----------------------------
            '    GESTIONE DESCRIZIONE
            '----------------------------
            If String.IsNullOrEmpty(Descrizione) Then
                If ID_Tipologia = enum_ID_Area_Tipologia.Patentino_trattamenti Then
                    Descrizione = "Patentino " & If(Num_Documento = "", "di ", Num_Documento & " di ")
                    Dim objContatti As New Contatti_R
                    Dim dtContatti = objContatti.LeggiContattoSpecifico(Piva, Cod_Contatto, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                    If dtContatti.Rows.Count > 0 Then
                        Descrizione &= dtContatti.Rows(0).Item("Rag_Soc") & dtContatti.Rows(0).Item("cognome") & " " & dtContatti.Rows(0).Item("nome")
                    End If
                    Descrizione &= If(Ente_Rilascio <> "" OrElse IsDate(Data_Rilascio), " rilasciato" & If(Ente_Rilascio <> "", " da " & Ente_Rilascio, "") & If(IsDate(Data_Rilascio), " il " & CDate(Data_Rilascio).ToShortDateString(), ""), "")
                End If
            End If

            '----------------------------
            '    GESTIONE DATE
            '----------------------------
            allineaValiditaPatentino(ID_Elenco, Piva, Cod_Contatto, Data_Rilascio, Data_Scadenza, objParametri_Server)

            '----------------------------
            '    TIPO SALVATAGGIO ALLEGATO (FS/DB)
            '----------------------------
            Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim DTSalvaAllegato As New DataTable
            Dim SalvaAllegato As Integer = 0 'File System Default
            DTSalvaAllegato = objImpostazioni.Leggi2(2, objParametri_Server.SuperUserUsername, enum_Impostazioni_Utenti.SUPERUSER_DOCUMENTALE_SALVA_ALLEGATO_SU_DB, "", "", objParametri_Utenti)
            If Not IsNothing(DTSalvaAllegato) AndAlso DTSalvaAllegato.Rows.Count > 0 Then
                SalvaAllegato = CInt(DTSalvaAllegato.Rows(0).Item("Impostazione_Valore_1"))
            End If

            Dim LeggiConfSiti As New Configurazione_Siti_R
            Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
            Dim Percorso As String = dt_Conf.Rows(0).Item("Valore")
            Dim MessaggioErrore As String = ""
            Percorso = FileSystemHelper.AggiungiSlashSeNonEsiste(Percorso)

            '----------------------------
            '    GESTIONE WORKFLOW
            '----------------------------
            Dim WorkFlow As String = ""

            Dim ObjAudit_Impostazione As New AgronicaCoreAuditDAL.Audit_Impostazioni_R
            Dim DtImpostazioni As DataTable = ObjAudit_Impostazione.LeggiImpostazione(Enum_Audit_impostazione.Documentale_GestioneWorkFlow, "", "", MessaggioErrore, objParametri_Server)

            If DtImpostazioni.Rows.Count > 0 Then
                WorkFlow = CStr(DtImpostazioni.Rows(0).Item("Valore1"))
            End If

            'Per i patentini, anche se non è stato effetivamente caricato un file dobbiamo comunque creare una riga di allegati_documenti per salvare scadenze, descrizioni, etc...
            'Questa variabile mi serve per far capire al biz di scrittura/modifica se deve saltare i controlli sulla validità del file stesso
            Dim Gestisci_FileAllegato As Boolean = File_Allegato <> "" OrElse (SalvaAllegato = 0 And Nome_File <> "")

            If ID_Elenco <= 0 Then
                '----------------------------
                '    NUOVO PATENTINO          
                '----------------------------
                res = Scrivi(ID_Tipologia,
                             objE,
                             Data_Scadenza,
                             Descrizione,
                             Nome_File,
                             Percorso,
                             AGRODATAINIZIO,
                             AGRODATAFINE,
                             objParametri_Server,
                             "",
                             New_ID_Elenco,
                             Ente_Des:=Ente_Rilascio,
                             Username_Upload:=objParametri_Server.UtenteUsername,
                             Data_Upload:=Now,
                             File_Allegato:=File_Allegato,
                             Allegati_Documenti_Numero:=Num_Documento,
                             SalvaAllegato:=SalvaAllegato,
                             ID_Area:=enum_ID_Area_Alert.Contatti,
                             Piva:=Piva,
                             WorkFlow_Documentale:=WorkFlow,
                             Data_Rilascio:=Data_Rilascio,
                             Gestisci_FileAllegato:=Gestisci_FileAllegato,
                             Note_Log:=Note_Log,
                             Origine:=Origine)

            Else
                '----------------------------
                '    MODIFICA PATENTINO          
                '----------------------------
                res = Modifica(ID_Tipologia,
                               objE,
                               ID_Elenco,
                               ID_Alert_Entita, Allegati_Documenti_Cod,
                               Data_Scadenza,
                               Descrizione,
                               Nome_File,
                               Percorso,
                               AGRODATAINIZIO,
                               AGRODATAFINE,
                               objParametri_Server,
                               "",
                               Nothing,
                               0,
                               objParametri_Server.UtenteUsername,
                               Now,
                               File_Allegato,
                               True,
                               Num_Documento,
                               SalvaAllegato,
                               ID_Area:=enum_ID_Area_Alert.Contatti,
                               Data_Rilascio:=Data_Rilascio,
                               Ente_Rilascio:=Ente_Rilascio,
                               Gestisci_FileAllegato:=Gestisci_FileAllegato,
                               Note_Log:=Note_Log,
                               Origine:=Origine)
            End If

        Catch ex As Exception
            res = ex.Message
        End Try

        Return res

    End Function

    Public Sub Scrivi_Patentino_APP(ByRef risorsa As AgronicaCoreModelsSTD.anagrafiche.RisorseUmane,
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim patentino = risorsa.contatto.documenti.OrderByDescending(Function(o) o.Data_Scadenza).ToList().FirstOrDefault()

        If Not IsNothing(patentino) AndAlso patentino.Data_Scadenza > DateTime.MinValue AndAlso patentino.Data_Scadenza < CostantiPersonalizzate.AGRODATAFINE Then

            Dim allegatiLeggiPerEntita As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
            Dim xFiltroAggiuntivo = " Alert_Elenco.ID_Tipologia = " & enum_ID_Area_Tipologia.Patentino_trattamenti & ""
            Dim dtPatentini = allegatiLeggiPerEntita.Leggi_Allegati_Entita(risorsa.contatto.primaryKey.partitaIva, risorsa.contatto.primaryKey.codice, 0, 0, 0, 0, 0, 0, xFiltroAggiuntivo, "Alert_Elenco.Data_Scadenza DESC", objParametri_Server)
            Dim patentinoEsistente = dtPatentini IsNot Nothing AndAlso dtPatentini.Rows.Count > 0
            patentino.ID_Tipologia = enum_ID_Area_Tipologia.Patentino_trattamenti

            If patentinoEsistente Then
                If patentino.Numero Is Nothing Then
                    patentino.Numero = dtPatentini.Rows(0).Item("Allegati_Documenti_Numero")
                End If
                If patentino.Data_Rilascio = DateTime.MinValue Then
                    patentino.Data_Rilascio = dtPatentini.Rows(0).Item("Validazione_Data")
                End If
                If patentino.Data_Scadenza = DateTime.MinValue Then
                    patentino.Data_Scadenza = dtPatentini.Rows(0).Item("Data_Scadenza")
                End If
                If patentino.Ente_Rilascio Is Nothing Then
                    patentino.Ente_Rilascio = New AgronicaCoreModelsSTD.documenti.EnteRilascio
                    patentino.Ente_Rilascio.codice = dtPatentini.Rows(0).Item("Allegati_Documenti_Ente_Cod")
                    patentino.Ente_Rilascio.descrizione = dtPatentini.Rows(0).Item("Allegati_Documenti_Ente_Des")
                End If
            End If

            Salva_Patentino(
                risorsa.contatto.primaryKey.partitaIva,
                risorsa.contatto.primaryKey.codice,
                patentino.ID_Tipologia,
                If(patentino.Numero Is Nothing, "N/D", patentino.Numero),
                If(patentino.Ente_Rilascio Is Nothing, "", patentino.Ente_Rilascio.descrizione),
                If(patentino.Data_Rilascio = DateTime.MinValue, CostantiPersonalizzate.AGRODATAINIZIO, patentino.Data_Rilascio),
                If(patentino.Descrizione Is Nothing, "Patentino Trattamenti", patentino.Descrizione),
                If(patentino.Data_Scadenza = DateTime.MinValue, CostantiPersonalizzate.AGRODATAFINE, patentino.Data_Scadenza),
                "",
                "",
                If(patentinoEsistente, dtPatentini.Rows(0).Item("ID_Elenco"), 0),
                If(patentinoEsistente, dtPatentini.Rows(0).Item("ID_Alert_Entita"), 0),
                If(patentinoEsistente, dtPatentini.Rows(0).Item("Allegati_Documenti_Cod"), 0),
                objParametri_Server,
                objParametri_Utenti,
                Note_Log:="Operazione registrata da SincroDatiApp",
                Origine:=enum_SistemiEsterni.GiasAPP)
        End If

    End Sub

    Private Shared Sub allineaValiditaPatentino(ByVal ID_Elenco As Integer, ByVal Piva As String, ByVal Cod_Contatto As String, ByVal Data_Rilascio As Date, ByVal Data_Scadenza As Date, ByVal objParametri_Server As AgronicaCoreParametri)

        '-----------------------
        '   ALLINEO SCADENZA
        '-----------------------
        Dim xFiltroAggiuntivoScadenza = "Allegati_Documenti.Validazione_Data <= '" + Data_Rilascio + "' AND Alert_Elenco.Data_Scadenza >= '" + Data_Rilascio + "' AND  Alert_Elenco.ID_Tipologia = " & enum_ID_Area_Tipologia.Patentino_trattamenti & " AND Alert_Elenco.ID_Elenco <> " & ID_Elenco

        'Cerco i patentini per cui è necessario aggiornare la scadenza (spostandola indietro di un giorno rispetto al nuovo patentino)
        Dim objDocumenti As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
        Dim DT_Scadenza As DataTable = objDocumenti.Leggi_Allegati_Entita(Piva, Cod_Contatto, 0, 0, 0, enum_TipoEntita.Contatto, 0, 0, xFiltroAggiuntivoScadenza, "", objParametri_Server)
        If DT_Scadenza.Rows.Count > 0 Then
            Dim IDElenco = DT_Scadenza(0).Item("ID_Elenco")
            Dim Descrizione_Scadenza = DT_Scadenza(0).Item("Descrizione_Scadenza")
            Dim NewDataScadenza = Data_Rilascio.AddDays(-1)

            Dim objAlertElenco As New Alert_Elenco_W
            objAlertElenco.ModificaAutomatica(IDElenco, NewDataScadenza, Descrizione_Scadenza, objParametri_Server)
        End If

        '-----------------------
        '   ALLINEO RILASCIO
        '-----------------------
        Dim xFiltroAggiuntivoRilascio = "Allegati_Documenti.Validazione_Data <= '" + Data_Scadenza + "' AND Alert_Elenco.Data_Scadenza >= '" + Data_Scadenza + "' AND  Alert_Elenco.ID_Tipologia = " & enum_ID_Area_Tipologia.Patentino_trattamenti & " AND Alert_Elenco.ID_Elenco <> " & ID_Elenco
        Dim DT_Rilascio As DataTable = objDocumenti.Leggi_Allegati_Entita(Piva, Cod_Contatto, 0, 0, 0, enum_TipoEntita.Contatto, 0, 0, xFiltroAggiuntivoRilascio, "Alert_Elenco.Data_Scadenza DESC", objParametri_Server)
        If DT_Rilascio.Rows.Count > 0 Then
            Dim Allegati_Documenti_Cod = DT_Rilascio(0).Item("Allegati_Documenti_Cod")
            Dim NewDataRilascio = Data_Scadenza.AddDays(1)

            Dim objAllegatiDocumenti As New Allegati_Documenti_W

            objAllegatiDocumenti.ModificaAutomatica(Allegati_Documenti_Cod, NewDataRilascio, objParametri_Server)
        End If

    End Sub

    Private Class ScritturaLog

        Inherits LogProvider

        Dim customLOGParams As CustomLOGParams
        Dim _objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        Dim _nomeFileLog As String
        Dim _nomeCartellaLog As String
        Dim _nomeUtente As String

        Sub New(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, nomeFileLog As String)
            _objParametri = objParametri

            customLOGParams = New CustomLOGParams With {
                .LogDescrizioneUtente = objParametri.LogDescrizioneUtente,
                .LogDirectory = objParametri.LogDirectory,
                .LogFileName = nomeFileLog
            }

        End Sub

        Public Sub ScriviLog(ByVal messaggio As String)
            Scrivi_LOG(_objParametri,
                       "AgronicaCoreScadenziario_BIZ.ScritturaLog.ScriviLog",
                       messaggio,
                       CustomLOGParams:=customLOGParams)
        End Sub

    End Class


End Class

