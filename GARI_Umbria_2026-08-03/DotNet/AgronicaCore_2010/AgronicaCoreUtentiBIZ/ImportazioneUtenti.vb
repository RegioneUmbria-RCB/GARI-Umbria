Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDTOStd.InData.NewAgri
Imports AgronicaCoreDTOStd.OutData.NewAgri
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreVarieDAL
Imports InData
Imports Microsoft.VisualBasic.ApplicationServices
Imports System.Net.Http
Imports AgronicaCoreModelsSTD.exceptions


Public Class ImportazioneUtenti

    Public Class ImportazioneUtentiResponse
        Public UsersOk As IEnumerable(Of String) = New List(Of String)
        Public UsersErrors As IEnumerable(Of String) = New List(Of String)
    End Class

    Public Function ImportUsersFromExcel(xlsFile As AgronicaCoreModelsSTD.Utility.FileWrapperAlt, params As ObjParams) As ImportazioneUtentiResponse
        Dim result As ImportazioneUtentiResponse
        Dim logger = CreateLogFile(params)
        Dim xlsResPath = UploadFile(xlsFile, params)
        CheckFileUploaded(xlsResPath)

        Try
            Dim userDT = LoadUserTable(xlsResPath)
            Dim userBusinessDT = LoadUserBusinessTable(xlsResPath)
            If IsNothing(userDT) Then
                Throw New GiasException("Impossibile leggere gli utenti dal file caricato!")
            ElseIf userDT.Rows.Count = 0 Then
                Throw New GiasException("Non è stato rilevato alcun utente nel file caricato!")
            Else
                result = ImportUsers(userDT, userBusinessDT, logger, params)
            End If
            WriteLogImportazioneUtenti(logger, "Importazione Conclusa", params)
        Catch ex As Exception
            Throw
        Finally
            logger.Close()
        End Try

        Dim gestoreCache As New AgronicaCoreVarieBIZ.GestoreCache
        Dim client As New HttpClient()

        gestoreCache.PulisciCachePermessi(client, params.ObjParametri_Server)
        gestoreCache.PulisciCacheImpostazioni(client, params.ObjParametri_Server)

        DeleteFileNoWarning(xlsResPath)
        Return result
    End Function

    Public Function ImportUserNewAgri(utente As AgronicaCoreDTOStd.InData.NewAgri.RequestUtente, params As ObjParams) As AgronicaCoreDTOStd.OutData.NewAgri.ResponseUtente
        Dim response As New AgronicaCoreDTOStd.OutData.NewAgri.ResponseUtente

        response.result = True
        response.error = ""

        Try
            Dim objUtentiR As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim dtUtenti = objUtentiR.Leggi2(utente.username, "", "", params.ObjParametri_Utenti)

            If utente.bloccato = True Then

                If dtUtenti.Rows.Count > 0 Then
                    Dim objUtentiPermessiR As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                    Dim strFiltroData = "Validita_Fine > " & UtilityProvider.Agro_SQL_SaveDate(Date.Now)
                    Dim dtUtentiPermessi = objUtentiPermessiR.Leggi(utente.username, enum_Id_Servizio.GiasOnline, 0, 9999, 0, strFiltroData, "", params.ObjParametri_Utenti)

                    If dtUtentiPermessi.Rows.Count > 0 Then
                        Dim objUtentiPermessiW As New AgronicaCoreUtentiDAL.Utenti_Permessi_W
                        If Not objUtentiPermessiW.Modifica_Validita(utente.username, enum_Id_Servizio.GiasOnline, CostantiPersonalizzate.AGRODATAINIZIO, Date.Now, strFiltroData, params.ObjParametri_Utenti) Then
                            response.result = False
                            response.error = "Errore durante l'importazione: utente non bloccato."
                        End If
                    Else
                        response.result = False
                        response.error = "Utente già bloccato."
                    End If
                Else
                    response.result = False
                    response.error = "Utente da bloccare non esistente a sistema."
                End If

            ElseIf dtUtenti.Rows.Count > 0 Then

                Dim objUtentiW As New AgronicaCoreUtentiDAL.Utenti_Write
                Dim objUtentiDettagliW As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W
                If Not (objUtentiDettagliW.Modifica_Parametrizzata(utente.username, "Cognome", If(utente.cognome, ""), "", params.ObjParametri_Utenti, True) And
                        objUtentiDettagliW.Modifica_Parametrizzata(utente.username, "Nome", If(utente.nome, ""), "", params.ObjParametri_Utenti, True) And
                        objUtentiDettagliW.Modifica_Parametrizzata(utente.username, "Email", If(utente.mail, ""), "", params.ObjParametri_Utenti, True) And
                        objUtentiW.ModificaFlagSPID(utente.username, utente.spid, params.ObjParametri_Utenti)) Then
                    response.result = False
                    response.error = "Errore durante l'importazione: utente non aggiornato."
                End If

                Dim objUtentiPermessiR As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                'Dim strFiltroData = "Validita_Fine <= " & UtilityProvider.Agro_SQL_SaveDate(Date.Now)
                Dim dtUtentiPermessi = objUtentiPermessiR.Leggi(utente.username, enum_Id_Servizio.GiasOnline, 0, 9999, 0, "", "", params.ObjParametri_Utenti)

                Dim objUtentiPermessiW As New AgronicaCoreUtentiDAL.Utenti_Permessi_W

                Dim Tipologia_Cod As Integer = FindTipologiaCodNewAgri(utente, params.ObjParametri_Server, params.ObjParametri_Utenti)
                'Dim objUtentW As New AgronicaCoreUtentiDAL.UtentiGias_Write

                objUtentiW.ModificaTipologia(utente.username, Tipologia_Cod, params.ObjParametri_Utenti)

                If dtUtentiPermessi.Rows.Count > 0 Then
                    If Not (objUtentiPermessiW.Cancella(utente.username, 0, 0, 0, 0, "", params.ObjParametri_Utenti) And
                            objUtentiPermessiW.ScriviDaTipologia(utente.username, "", params.ObjParametri_Utenti)) Then
                        response.result = False
                        response.error = "Errore durante l'importazione: permessi utenti non aggiornati."
                    End If
                Else
                    objUtentiPermessiW.ScriviDaTipologia(utente.username, "", params.ObjParametri_Utenti)
                End If

            Else

                Dim Sistema_Esterno_NewAgri As Integer = TipiEnumerativi.enum_SistemiEsterni.NewAgri
                Dim Tipologia_Cod As Integer = FindTipologiaCodNewAgri(utente, params.ObjParametri_Server, params.ObjParametri_Utenti)

                If Tipologia_Cod > 0 Then
                    Dim objGruppiUtenteR As New AgronicaCoreUtentiDAL.Gruppi_Utente_R
                    Dim strFiltroGruppo As String = "Gruppi_Utente_Identificativo = '" & UtilityProvider.Agro_SQL_SaveText(params.ObjParametri_Utenti.PivaSuperUser) + "' "
                    Dim dtGruppiUtente = objGruppiUtenteR.Leggi(0, strFiltroGruppo, "", params.ObjParametri_Utenti)

                    If dtGruppiUtente.Rows.Count > 0 Then
                        Dim gruppoUtentiCod As Integer = dtGruppiUtente.Rows(0)("Gruppi_Utente_cod")
                        Dim objUserWriter As New AgronicaCoreUtentiDAL.UserWriter
                        If Not objUserWriter.CreateFromImportNewAgriData(utente.username, utente.nome, utente.cognome, utente.codice_Fiscale, utente.mail, Tipologia_Cod, params, setFlagAccessoSpid:=utente.spid, gruppoUtenti:=gruppoUtentiCod) Then
                            response.result = False
                            response.error = "Errore durante l'importazione: nuovo utente non creato."
                        End If
                    Else
                        response.result = False
                        response.error = "Gruppo utenti da assegnare non trovato."
                    End If
                Else
                    response.result = False
                    response.error = "Nessuna tipologia di utente trovata per i ruoli specificati."
                End If
            End If

            Dim gestoreCache As New AgronicaCoreVarieBIZ.GestoreCache
            Dim client As New HttpClient()

            gestoreCache.PulisciCachePermessi(client, params.ObjParametri_Server)
            gestoreCache.PulisciCacheImpostazioni(client, params.ObjParametri_Server)

        Catch ex As Exception
            response.result = False
            response.error = ex.Message
        End Try

        Return response
    End Function

    Private Function FindTipologiaCodNewAgri(utente As AgronicaCoreDTOStd.InData.NewAgri.RequestUtente, ObjParametri_Server As AgronicaCoreParametri, ObjParametri_Utenti As AgronicaCoreParametri) As Integer
        Dim Tipologia_Cod As Integer = 0
        Dim Sistema_Esterno_NewAgri As Integer = TipiEnumerativi.enum_SistemiEsterni.NewAgri
        If utente.codice_Fiscale Is Nothing OrElse String.IsNullOrWhiteSpace(utente.codice_Fiscale) Then
            utente.codice_Fiscale = GeneraCodFiscaleFittizio(ObjParametri_Server, ObjParametri_Utenti)
        End If

        Dim objUtentiTipologiexRuoliSistemiEsterni As New AgronicaCoreUtentiDAL.Utenti_TipologiexRuoli_SistemiEsterni_R
        Dim strRuoli As String = String.Join(",", utente.ruoli)
        Dim dtUtentiTipologiexSistemiEsterni = objUtentiTipologiexRuoliSistemiEsterni.Leggi(Sistema_Esterno_NewAgri, strRuoli, "", "", ObjParametri_Utenti)

        If dtUtentiTipologiexSistemiEsterni.Rows.Count > 0 Then
            Tipologia_Cod = dtUtentiTipologiexSistemiEsterni.Rows(0)("Tipologia_Cod")

        Else
            dtUtentiTipologiexSistemiEsterni = objUtentiTipologiexRuoliSistemiEsterni.Leggi(Sistema_Esterno_NewAgri, "", "", "", ObjParametri_Utenti)
            If dtUtentiTipologiexSistemiEsterni.Rows.Count > 0 Then
                For Each ruolo In utente.ruoli
                    For Each row In dtUtentiTipologiexSistemiEsterni.Rows
                        If ruolo = row("Ruolo_SistemaEsterno") Then
                            Tipologia_Cod = row("Tipologia_Cod")
                            Exit For
                        End If
                    Next
                    If Tipologia_Cod > 0 Then
                        Exit For
                    End If
                Next
            End If
        End If

        Return Tipologia_Cod
    End Function


    Public Function GeneraCodFiscaleFittizio(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.GeneraPivaFittizia()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim Codice_Fiscale As String = ""

        Try

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

            Dim ok = False


            Dim objAgroSe As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim objutenti As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R

            Dim str_r As String = ""
            While Not ok

                str_r = objAgroSe.NuovoId_Tabella("utentiCodFiscale", 0, 0, objParametri_Server).ToString("D16")
                'controllo se è già usato 
                If Not objutenti.EsisteCodice_Fiscale(str_r, objParametri_Utenti) Then
                    ok = True
                End If

            End While

            objParametri_Server.ResettaFinestra()

            Codice_Fiscale = str_r

        Catch ex As Exception

            MessaggioErrore = ex.Message
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return Codice_Fiscale

    End Function

#Region "File management"

    Private Function CreateLogFile(params As ObjParams) As System.IO.StreamWriter
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        objWebConfig.impostaDaDB(params.ObjParametri_SuperServer, params.ObjParametri_Server)
        Dim logDir = objWebConfig.PathDirectoryLOG
        Dim SW As System.IO.StreamWriter
        Dim importName As String = String.Empty
        'Verifico l'esistenza della Directory passata dalle stampe (indicata nel web config)....
        'Altrimenti metto di default la directory C:\GIASLAN
        If logDir = "" Then
            logDir = "C:\GIASLAN"
        End If
        logDir = logDir & "\Importazione_" + importName + "_Log"

        'Verifico esistenza Cartella Importazione_Anagrafiche_Log
        If System.IO.Directory.Exists(logDir) = False Then
            System.IO.Directory.CreateDirectory(logDir)
        End If

        Dim NomeFile = "Importazione_Utenti" + importName + "__" & Now.ToString("yyyy-MM-dd__(hh.mm.ss)") & ".txt"
        Dim Path_File_Log = logDir & "\Importazione_Utenti" + importName + "__" & Now.ToString("yyyy-MM-dd__(hh.mm.ss)") & ".txt"

        SW = System.IO.File.CreateText(Path_File_Log)
        Return SW
    End Function

    Private Sub WriteLogImportazioneUtenti(logger As System.IO.StreamWriter, msg As String, params As ObjParams)
        If logger IsNot Nothing Then
            Dim Testo = Date.Now.ToShortDateString & " " &
                   Date.Now.ToLongTimeString & " {" &
                   params.ObjParametri_Server.LogDescrizioneUtente &
                   "} : [Importazione_Utenti] : " & msg
            logger.WriteLine(Testo)
        End If
    End Sub

    Private Sub DeleteFileNoWarning(resourcePath As String)
        Try
            System.IO.File.Delete(resourcePath)
        Catch ex As Exception
        End Try
    End Sub

    ''' <param name="fileName">The name of the .xls file with the users' data</param>
    Private Function UploadFile(xlsFile As AgronicaCoreModelsSTD.Utility.FileWrapperAlt, params As ObjParams) As String
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        objWebConfig.impostaDaDB(params.ObjParametri_SuperServer, params.ObjParametri_Server)
        Dim resPath As String
        Try
            If String.IsNullOrWhiteSpace(xlsFile.FileName.Trim()) Then
                Throw New InvalidOperationException("Selezionare il file da importare")
            End If

            Dim uploaded As String = System.IO.Path.GetFileName(xlsFile.FileName)
            Dim cpyFileName = "XLSX_" + params.ObjParametri_Server.SuperUserUsername + "_" + uploaded
            Dim pathUpload As String = objWebConfig.GestioneImportazioni_Repository

            If Not pathUpload.EndsWith("\") Then
                pathUpload = pathUpload + "\"
            End If

            resPath = pathUpload & cpyFileName
            If Not System.IO.File.Exists(resPath) = True Then
                Dim fileBytes As Byte() = Convert.FromBase64String(xlsFile.Base64FileStr)
                System.IO.File.WriteAllBytes(resPath, fileBytes)
            End If
        Catch ex As InvalidOperationException
            Throw New GiasException("Errore: UPLOAD FALLITO.<br>" & ex.Message)
        End Try

        Return resPath
    End Function

    Private Sub CheckFileUploaded(resourcePath As String)
        If String.IsNullOrEmpty(resourcePath) Then
            Throw New GiasException("File non trovato.")
        Else
            If Not System.IO.File.Exists(resourcePath) Then
                Throw New GiasException("Il File selezionato NON ESISTE!")
            End If
        End If
    End Sub

#End Region

#Region "Data load"

    Private Function GetConnectionString(xlsResPath As String) As String
        Dim extension As String = xlsResPath.Split(".")(1)
        Dim provider As String
        If Environment.Is64BitProcess Then
            provider = "PROVIDER=Microsoft.ACE.OLEDB.12.0"
        Else
            provider = "PROVIDER=Microsoft.Jet.OLEDB.4.0"
        End If
        Dim StringaConnessione As String = provider
        Select Case extension
            Case "xls"
                StringaConnessione &= ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1';data source="
                StringaConnessione &= "'" & xlsResPath & "'"
            Case "xlsx"
                StringaConnessione &= ";Extended Properties='Excel 12.0;HDR=YES;';data source="
                StringaConnessione &= "'" & xlsResPath & "'"
            Case Else
                'provider = "PROVIDER=Microsoft.Jet.OLEDB.4.0"
                StringaConnessione &= ";Extended Properties='text;HDR=Yes;FMT=Delimited(;)';data source="
                StringaConnessione &= "'" & xlsResPath & "'"
        End Select
        Return StringaConnessione
    End Function

    Private Function LoadUserTable(xlsResPath As String) As DataTable
        Dim connStr = GetConnectionString(xlsResPath)
        Dim userDT As DataTable = Nothing
        Try
            Dim ds_utenti As New DataSet
            Dim MyConnection As New System.Data.OleDb.OleDbConnection(connStr)
            MyConnection.Open()

            Dim dtSheet = MyConnection.GetSchema("Tables")
            Dim foglioUtenti As String = dtSheet.Select.Select(Function(sheet) CStr(sheet("TABLE_NAME")).ToLower).
                Where(Function(sheetName) sheetName.Contains("utenti")).
                DefaultIfEmpty("").First

            Dim da_utenti As New System.Data.OleDb.OleDbDataAdapter("select * from [" & foglioUtenti & "]", MyConnection)
            da_utenti.Fill(ds_utenti, "fileXls")

            MyConnection.Close()
            userDT = ds_utenti.Tables(0)
        Catch ex As Exception
            Throw New Exception("Errore all'apertura del file excel: " & ex.Message)
        End Try
        Return userDT
    End Function

    Private Function LoadUserBusinessTable(xlsResPath As String) As DataTable
        Dim connStr = GetConnectionString(xlsResPath)
        Dim userDT As DataTable = Nothing
        Try
            Dim ds_UtentiAziende As New DataSet
            Dim MyConnection As New System.Data.OleDb.OleDbConnection(connStr)
            MyConnection.Open()

            Dim dtSheet = MyConnection.GetSchema("Tables")
            Dim foglioAziende As String = dtSheet.Select.Select(Function(sheet) CStr(sheet("TABLE_NAME")).ToLower).
                Where(Function(sheetName) sheetName.Contains("aziende")).
                DefaultIfEmpty("").First

            Dim da_utentiAziende As New System.Data.OleDb.OleDbDataAdapter("select * from [" & foglioAziende & "]", MyConnection)
            da_utentiAziende.Fill(ds_UtentiAziende, "fileXls")

            MyConnection.Close()
            userDT = ds_UtentiAziende.Tables(0)
        Catch ex As Exception
            Throw New Exception("Errore all'apertura del file excel: " & ex.Message)
        End Try
        Return userDT
    End Function

    Private Function LoadVisibility(
        Dt_Utenti As DataTable, Dt_UtentiAziende As DataTable,
        userRow As DataRow,
        ByRef ListaVisibilitaPiva As List(Of String),
        ByRef ListaPiva As List(Of String),
        logger As System.IO.StreamWriter,
        params As ObjParams
    )
        Dim nome = GetStringOrDefault(userRow, "Nome")
        Dim cognome = GetStringOrDefault(userRow, "Cognome")
        Dim username = GetUsernameFromRow(userRow)
        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim ListaCuaa As New List(Of String)
        Dim ListaVisibilita As List(Of String) = Dt_Utenti.Select.
            Where(Function(dr) GetStringOrDefault(dr, "nome") = nome AndAlso GetStringOrDefault(dr, "cognome") = cognome).
            Select(Function(dr) GetStringOrDefault(dr, "visibilita")).
            Distinct.ToList
        Dim isVisibilityEmpty = ListaVisibilita.Count = 0 OrElse (ListaVisibilita.Count = 1 AndAlso ListaVisibilita.First = "")

        If isVisibilityEmpty Then
            ListaCuaa = Dt_UtentiAziende.Select.
                Where(Function(dr) GetStringOrDefault(dr, "username") = username).
                Select(Function(dr) GetStringOrDefault(dr, "cuaa")).
                Distinct.ToList
        End If

        For Each el In ListaVisibilita
            If el <> "" Then
                Dim piva = objImprese.Piva_from_CUAA(el, params.ObjParametri_Server)
                If piva <> "" Then
                    ListaVisibilitaPiva.Add(piva)
                End If
            Else
                ListaVisibilitaPiva.Add(el)
            End If
        Next
        For Each el In ListaCuaa
            If el <> "" Then
                Dim piva = objImprese.Piva_from_CUAA(el, params.ObjParametri_Server)
                If piva <> "" Then
                    ListaPiva.Add(piva)
                End If
            End If
        Next

        If (ListaVisibilita.Count = 0 OrElse (ListaVisibilita.Count = 1 AndAlso ListaVisibilita(0) <> "")) AndAlso ListaVisibilitaPiva.Count = 0 Then
            WriteLogImportazioneUtenti(logger, nome & "." & cognome & " (" & username & ") --> Impresa Padre non trovata. ", params)
            Return "Impresa Padre non trovata"
        End If
        If isVisibilityEmpty AndAlso ListaPiva.Count = 0 AndAlso ListaCuaa.Count > 0 AndAlso ListaCuaa(0) <> "" Then
            WriteLogImportazioneUtenti(logger, nome & "." & cognome & " (" & username & ") --> Impresa non trovata. ", params)
            Return "Impresa non trovata"
        End If
        Return String.Empty
    End Function

#End Region

#Region "User checks"

    Private Function hasRequiredFields(user As DataRow) As Boolean
        Dim Nome = GetStringOrDefault(user, "nome")
        Dim Cognome = GetStringOrDefault(user, "cognome")
        Dim CodiceFiscale = GetStringOrDefault(user, "codice_fiscale")
        Dim Email = GetStringOrDefault(user, "email")
        Dim Profilo = GetIntegerOr(user, "profilo", 0)
        Return Profilo <> 0 AndAlso {Nome, Cognome, CodiceFiscale, Email}.All(Function(str) Not String.IsNullOrWhiteSpace(str))
    End Function

    Private Function IsUsernameTaken(ByRef username As String, params As ObjParams) As Boolean
        username = Pulisci_Username(username)
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim DtUtenti As DataTable
        DtUtenti = objUtenti.Leggi(
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            "", String.Empty, params.ObjParametri_Utenti, username
        )
        Return (Not DtUtenti Is Nothing AndAlso DtUtenti.Rows.Count > 0)
    End Function

    Private Function IsCodiceFiscaleTaken(ByRef CF As String, params As ObjParams) As Boolean
        Dim objUtentiDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim dt = objUtentiDettagli.Utenti_Dettagli_from_CF(
            CF, enumSelezioneVariabile.Selezione_TabellaCompleta,
            String.Empty, String.Empty, params.ObjParametri_Utenti
        )
        Return (dt IsNot Nothing AndAlso dt.Rows.Count > 0)
    End Function

    Private Function isPasswordHashEnabled(params As ObjParams)
        Dim handleConfigSiti As New Configurazione_Siti_R
        Dim dtConfigSiti As DataTable = handleConfigSiti.Leggi(0, "AbilitaHashPassword", "", "", params.ObjParametri_Server)
        Dim hashPasswordAbilitato As Boolean = False
        If dtConfigSiti.Rows.Count > 0 Then
            hashPasswordAbilitato = dtConfigSiti.Rows(0)("Valore")
        End If
        Return hashPasswordAbilitato
    End Function

    Private Function Pulisci_Username(ByVal Username As String) As String
        Username = Username.Replace(",", "")
        Username = Username.Replace("'", "")
        Username = Username.Replace(" ", "")
        Username = Username.Replace("à", "a")
        Username = Username.Replace("è", "e")
        Username = Username.Replace("é", "e")
        Username = Username.Replace("ù", "u")
        Username = Username.Replace("ò", "o")
        Username = Username.Replace("ì", "i")
        Return Username
    End Function

#End Region

    Private Function ImportUsers(userDt As DataTable, userBusinessDT As DataTable, logger As System.IO.StreamWriter, params As ObjParams) As ImportazioneUtentiResponse
        Dim result As New ImportazioneUtentiResponse
        Dim HashUtenti As New Hashtable
        Dim Username, descUtente As String
        Dim InseritoUtente As Boolean

        Dim createdUsers As List(Of String) = New List(Of String)
        Dim notCreatedUsers As List(Of String) = New List(Of String)

        For Each dr_utente As DataRow In userDt.Rows

            If IsEmptyRow(dr_utente) Then
                Continue For 'Skip to next row
            End If

            Dim messaggioErroreUtente As String = ""
            Username = GetUsernameFromRow(dr_utente)
            descUtente = GetUserDescription(dr_utente)
            InseritoUtente = False

            If hasRequiredFields(dr_utente) Then
                If Not HashUtenti.ContainsKey(Username) Then
                    Dim strErr_InsertUtente = ""
                    Dim ListaVisibilitaPiva As New List(Of String)
                    Dim ListaPiva As New List(Of String)
                    LoadVisibility(userDt, userBusinessDT, dr_utente, ListaVisibilitaPiva, ListaPiva, logger, params)

                    If messaggioErroreUtente = "" Then
                        InseritoUtente = createUser(dr_utente, ListaVisibilitaPiva, ListaPiva, strErr_InsertUtente, params)
                    End If

                    'Finalize user creation
                    If InseritoUtente Then
                        createdUsers.Add(Username)
                        WriteLogImportazioneUtenti(logger, Username & " --> Utente Attivato", params)
                    Else
                        notCreatedUsers.Add(descUtente & " (" & strErr_InsertUtente & ")")
                        WriteLogImportazioneUtenti(logger, descUtente & " --> Utente NON Attivato --> " & strErr_InsertUtente, params)
                    End If
                    HashUtenti.Add(Username, "")
                End If
            Else
                Dim errMsg = "Per generare l'Utente è necessario avere Nome, Cognome, Codice Fiscale, Email e Profilo"
                notCreatedUsers.Add(descUtente & " (" & errMsg & ")")
                WriteLogImportazioneUtenti(logger, descUtente & " --> Utente NON Attivato --> " & errMsg, params)
            End If
        Next

        result.UsersOk = createdUsers
        result.UsersErrors = notCreatedUsers
        Return result
    End Function

#Region "User creation"

    Private Function createUser(
        userRow As DataRow,
        ListaVisibilita As List(Of String),
        ListaPive As List(Of String),
        ByRef strErr As String,
        params As ObjParams
    ) As Boolean
        Dim isUserCreated As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            Dim Username = GetUsernameFromRow(userRow)
            Dim _Password = GetStringOrDefault(userRow, "password")
            Dim Nome = GetStringOrDefault(userRow, "nome")
            Dim Cognome = GetStringOrDefault(userRow, "cognome")
            Dim CodiceFiscale = GetStringOrDefault(userRow, "codice_fiscale")
            Dim Email = GetStringOrDefault(userRow, "email")
            Dim Tel = GetStringOrDefault(userRow, "telefono")
            Dim Fax = GetStringOrDefault(userRow, "fax")
            Dim Qualifica = GetStringOrDefault(userRow, "qualifica")
            Dim Gruppo_Utente = GetIntegerOr(userRow, "gruppo_utente", 99_999_999)
            Dim Profilo = GetIntegerOr(userRow, "profilo", 0)

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, params.ObjParametri_Utenti)

            Dim hasRequiredFields = {Nome, Cognome, CodiceFiscale}.All(Function(field) Not String.IsNullOrWhiteSpace(field))
            If Not hasRequiredFields Then
                strErr = "Per generare l'Utente è necessario avere Nome, Cognome e Codice Fiscale!"
            End If

            'controllo esistenza utente
            Dim esisteUtente As Boolean = IsUsernameTaken(Username, params)
            If esisteUtente Then
                strErr = "Username già presente!"
            End If
            If Not esisteUtente AndAlso IsCodiceFiscaleTaken(CodiceFiscale, params) Then
                strErr = "Codice Fiscale già presente!"
            End If

            Dim hashPasswordAbilitato As Boolean = isPasswordHashEnabled(params)
            Dim Password = Pulisci_Username(If(String.IsNullOrWhiteSpace(_Password), Cognome, _Password))
            If Not Utenti.ValidaComplessitaPassword(Password, gestioneHashAbilitata:=hashPasswordAbilitato) Then
                strErr = Utenti.MessaggioRequisitiPassword(gestioneHashAbilitata:=hashPasswordAbilitato)
            End If

            'creazione utente
            Dim success As Boolean = False
            If Not esisteUtente Then
                '----- Record UTENTE
                Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Write
                success = objUtente.Scrivi(
                    Username, Password, enum_AgroLingue.Italiano_it, hashPasswordAbilitato, False,
                    params.ObjParametri_Utenti, Profilo
                )
            End If

            If success Then
                If Not esisteUtente Then
                    '----- Record DETTAGLI (PERSONA)
                    Dim objDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W
                    objDettagli.Scrivi(
                        Username, Cognome, Nome,
                        String.Empty, String.Empty, String.Empty, String.Empty, String.Empty,
                        Tel, Fax, Email, String.Empty, CodiceFiscale, String.Empty, 2,
                        0, Qualifica, params.ObjParametri_Utenti
                    )
                End If

                SetUserGroup(Username, Gruppo_Utente, esisteUtente, params)
                AssignVisibility(Username, ListaVisibilita, ListaPive, params)
                ApplyProfile(Username, Profilo, params)

                isUserCreated = True
            End If

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, params.ObjParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, params.ObjParametri_Utenti)
        Catch ex As Exception
            strErr = ex.Message
            'rollback della transazione
            If params.ObjParametri_Utenti.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, params.ObjParametri_Utenti)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, params.ObjParametri_Utenti)
            End If
            isUserCreated = False
        End Try

        Return isUserCreated
    End Function

    Private Sub AssignVisibility(username As String, ListaVisibilita As List(Of String), ListaPive As List(Of String), params As ObjParams)
        Dim objVis As New AgronicaCoreUtentiBIZ.Utenti_Visibilita
        Dim user As New AgronicaCoreModelsSTD.profilazione.UtenteDTO With {.UserName = username}
        Dim business As New List(Of AgronicaCoreModelsSTD.anagrafiche.ImpresaDto)
        If ListaVisibilita IsNot Nothing AndAlso ListaVisibilita.Any(Function(piva) Not String.IsNullOrEmpty(piva)) Then
            business = ListaVisibilita.Where(Function(piva) Not String.IsNullOrEmpty(piva)).
                Select(Function(piva) New AgronicaCoreModelsSTD.anagrafiche.ImpresaDto With {.piva = piva}).
                ToList
        ElseIf ListaPive IsNot Nothing AndAlso ListaPive.Any(Function(piva) Not String.IsNullOrEmpty(piva)) Then
            business = ListaPive.Where(Function(piva) Not String.IsNullOrEmpty(piva)).
               Select(Function(piva) New AgronicaCoreModelsSTD.anagrafiche.ImpresaDto With {.piva = piva}).
               ToList
        End If
        objVis.ImpostaVisibilitaAzienda(
            user, business, False, False,
            params.ObjParametri_Server, params.ObjParametri_Utenti,
            False
        )
    End Sub

    ''' <summary>
    ''' Applica i permessi e le impostazioni del profilo specificato all'utente.
    ''' I permessi sono validi all'interno dell'anno corrente
    ''' nel periodo 01/01/~31/12.
    ''' </summary>
    Private Sub ApplyProfile(username As String, profilo As Integer, params As ObjParams)
        Dim objUt As New AgronicaCoreUtentiBIZ.Utenti
        Dim userList As New List(Of UtentePermessi) From {New UtentePermessi With {
            .UserName = username,
            .ValiditaInizioPermessi = CDate("01/01/" & Today.Year),
            .ValiditaFinePermessi = CDate("31/12/" & Today.Year)
        }}
        Dim profile As New AgronicaCoreModelsSTD.profilazione.TipologiaUtente(profilo, String.Empty)
        objUt.AssociaProfilo(userList, profile, True, params, False, True)
    End Sub

    Private Sub SetUserGroup(username As String, group As Integer, esisteUtente As Boolean, params As ObjParams)
        If group > 0 Then
            Dim objGruppi As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_W
            If esisteUtente Then
                objGruppi.Cancella(0, username, "", params.ObjParametri_Utenti)
            End If
            objGruppi.Scrivi(
                group, username,
                CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAFINE,
                params.ObjParametri_Utenti
            )
        End If
    End Sub

#End Region

#Region "Utility"

    Private Function GetUsernameFromRow(dr_utente As DataRow) As String
        Dim Username = GetStringOrDefault(dr_utente, "username")
        Dim Nome = GetStringOrDefault(dr_utente, "nome")
        Dim Cognome = GetStringOrDefault(dr_utente, "cognome")
        Username = If(String.IsNullOrWhiteSpace(Username), Nome & "." & Cognome, Username)
        Return Pulisci_Username(Username.ToLower)
    End Function

    Private Function GetUserDescription(dr_utente As DataRow) As String
        Dim Nome = GetStringOrDefault(dr_utente, "nome")
        Dim Cognome = GetStringOrDefault(dr_utente, "cognome")
        Dim CodiceFiscale = GetStringOrDefault(dr_utente, "codice_fiscale")
        Return "Username: " & GetUsernameFromRow(dr_utente) & " - Nome: " & Nome & " - Cognome: " & Cognome & " - Codice Fiscale: " & CodiceFiscale
    End Function

    Private Function IsEmptyRow(row As DataRow) As Boolean
        Return row.ItemArray.All(Function(value) IsDBNull(value) OrElse String.IsNullOrWhiteSpace(CStr(value)))
    End Function

    Private Function GetStringOrDefault(row As DataRow, field As String) As String
        Return If(Not row.Table.Columns.Contains(field) OrElse IsDBNull(row(field)),
            String.Empty, CStr(row(field))
        ).Trim
    End Function

    Private Function GetIntegerOr(row As DataRow, field As String, defaultValue As Integer) As Integer
        Return If(Not row.Table.Columns.Contains(field) OrElse IsDBNull(row(field)) OrElse Not IsNumeric(row(field)),
                defaultValue, CInt(row(field))
            )
    End Function

#End Region

End Class
