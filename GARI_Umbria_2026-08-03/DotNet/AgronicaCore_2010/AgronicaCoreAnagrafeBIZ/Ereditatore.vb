Imports System.Data.Entity
Imports System.Linq
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCorePianoConcimazioneBIZ
Imports AgronicaCoreVarieBIZ
Imports DocumentFormat.OpenXml.InkML
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Ereditatore
    Inherits AgronicaCoreDataProvider.DataProvider

    Public parametriAppezzamento As New List(Of Integer) From {Enum_ParametriModificaMultiplaPianoColturale.APP_MetodoProduzione,
                                                               Enum_ParametriModificaMultiplaPianoColturale.APP_DataFineAppezzamento,
                                                               Enum_ParametriModificaMultiplaPianoColturale.APP_NrAppBio}

    Dim parametriImpianto As New List(Of Integer) From {Enum_ParametriModificaMultiplaPianoColturale.IMP_Finalita,
                                                        Enum_ParametriModificaMultiplaPianoColturale.IMP_Varieta,
                                                        Enum_ParametriModificaMultiplaPianoColturale.IMP_GruppoVarietale,
                                                        Enum_ParametriModificaMultiplaPianoColturale.IMP_Copertura,
                                                        Enum_ParametriModificaMultiplaPianoColturale.IMP_ImpIrrigazione,
                                                        Enum_ParametriModificaMultiplaPianoColturale.IMP_SuFila,
                                                        Enum_ParametriModificaMultiplaPianoColturale.IMP_TraFila,
                                                        Enum_ParametriModificaMultiplaPianoColturale.IMP_FormaAllevamento,
                                                        Enum_ParametriModificaMultiplaPianoColturale.IMP_Portinnesto,
                                                        Enum_ParametriModificaMultiplaPianoColturale.IMP_DataInizioPortinnesto,
                                                        Enum_ParametriModificaMultiplaPianoColturale.IMP_DataFineImpianto,
                                                        Enum_ParametriModificaMultiplaPianoColturale.IMP_DataInizioImpianto}

    Dim parametriEsercizio = New List(Of Integer) From {Enum_ParametriModificaMultiplaPianoColturale.ESE_DisciplinareMassimaleNPK,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_Resa,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_DataSemina,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_DataRaccolta,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_DataFioritura,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_CapitolatoPrivato,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_OrganismoReferente,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_MagazzinoConferimento,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_Certificazione,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_LimiteN,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_LimiteP,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_LimiteK,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_Regolamento,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_DPI,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_FlagSecondoRaccolto,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_CertificazioneAziendale,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_Contributi,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_CertificazioneProdotto,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_Residuo,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_LicenzaColtivazione,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_RiferimentoTrasferimentoDati,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_Tecnico,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_PianoSemina,
                                                        Enum_ParametriModificaMultiplaPianoColturale.ESE_Prodotto}

    Dim dicAppezzamenti As New Dictionary(Of keyAppezzamento, DatiAnagrafica)
    Dim dicImpianti As New Dictionary(Of keyImpianto, DatiAnagrafica)
    Dim dicEsercizi As New Dictionary(Of keyEsercizio, DatiAnagrafica)

    'Se false, non ricarico la griglia 
    Dim modificatoAlmenoUnElemento As Boolean = False

    <Obsolete("Implementata nuova Modifica Multipla Massiva")>
    Public Function ModificaMultipla_PianoColturale(ByVal parametri As String, ByVal dati As String, ByRef objParametriServer As AgronicaCoreParametri) As RispostaStandard

        Dim nomeRoutine As String = "AnagrafeBIZ.Ereditatore.ModificaMultipla_PianoColturale"
        Dim messaggioErrore As String = ""

        Dim r As New RispostaStandard

        Dim objImpiantoCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)

        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim NoteLog As String = "Operazione effettuata da Modifica Multipla Piano Colturale"
        'Using scope As New TransactionScope()

        Try

            Dim objParametri As JObject = JObject.Parse(parametri)
            Dim objDati As JArray = JArray.Parse(dati)

            Dim anagrafica = objParametri("anagrafica")

            Dim parametriAnagrafica = objParametri("parametri")
            Dim mod_esercizi = objParametri("mod_esercizi")
            Dim data_esercizi = objParametri("data_esercizi")

            Dim parametriAppezzamento = New List(Of String) From {"5", "27", "29"}
            Dim parametriImpianto = New List(Of String) From {"1", "3", "4", "6", "19", "22", "23", "24", "25", "26", "28", "40"}
            Dim parametriEsercizio = New List(Of String) From {"2", "7", "8", "9", "10", "12", "13", "14", "15", "16", "17", "18", "20", "21", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"}

            For Each obj As JObject In objDati
                Using scope As New TransactionScope()
                    Dim Piva = CStr(obj("PIVA"))
                    Dim Sa_Cod = CInt(obj("sa_cod"))
                    Dim Appezza = CInt(obj("Appezza"))
                    Dim Id_Reg = If(obj("id_Reg") Is Nothing, 0, CInt(obj("id_Reg")))
                    Dim Progetto_Cod = If(obj("Progetto_Cod") Is Nothing, 0, CInt(obj("Progetto_Cod")))


                    For Each selezionati As String In anagrafica
                        For Each parametro In parametriAnagrafica

                            Select Case selezionati

                                Case "1" ' modifica appezzamenti

                                    If parametriAppezzamento.Contains(parametro) Then
                                        ModificaAppezzamento(Piva, Sa_Cod, Appezza, parametro, obj, objParametriServer, GiasContext, NoteLog)
                                    End If

                                Case "2" ' modifica impianti/esercizi

                                    If parametriImpianto.Contains(parametro) Then

                                        ' modifica impianti
                                        ModificaImpianto(Piva, Sa_Cod, Appezza, Id_Reg, parametro, obj, objParametriServer, GiasContext)

                                    ElseIf parametriEsercizio.Contains(parametro) AndAlso mod_esercizi IsNot Nothing Then

                                        ' filtra esercizi per data validita
                                        Dim Data_Validita = Date.Now.Date
                                        If mod_esercizi = "2" Then
                                            Data_Validita = AGRODATAINIZIO
                                        ElseIf mod_esercizi = "3" AndAlso data_esercizi IsNot Nothing Then
                                            Data_Validita = CDate(data_esercizi.ToString)
                                        End If

                                        ' modifica esercizi
                                        ModificaEsercizio(Piva, Sa_Cod, Appezza, Id_Reg, 0, Data_Validita, parametro, obj, objParametriServer, GiasContext, NoteLog)

                                    End If

                                Case "3" ' modifica esercizi

                                    If parametriEsercizio.Contains(parametro) Then
                                        ModificaEsercizio(Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, AGRODATAINIZIO, parametro, obj, objParametriServer, GiasContext, NoteLog)
                                    End If

                            End Select

                        Next
                    Next
                    'GiasContext.SaveChanges()

                    scope.Complete()
                    scope.Dispose()
                End Using
            Next

            'GiasContext.SaveChanges()

            'scope.Complete()
            'scope.Dispose()

            r.RispostaOK = True
            r.RispostaStringa = "Salvataggio effettuato correttamente"

        Catch ex As GiasException
            'Errore gestito
            Throw ex
        Catch ex As Exception

            r.RispostaOK = False
            messaggioErrore = ex.Message
            r.Errore = "[" & nomeRoutine & "] : " & messaggioErrore
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

            'r.RispostaOK = False

            'messaggioErrore = ex.Message
            'Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)

            'r.Errore = "[" & nomeRoutine & "] : " & messaggioErrore
            'scope.Dispose()

        Finally

            GiasContext.Dispose()

        End Try

        '

        Return r

    End Function

    <Obsolete("Utilizzato dall'anagrafica vecchia")>
    Public Function ModificaMultipla(ByVal parametri As String, ByVal dati As String, ByRef objParametriServer As AgronicaCoreParametri) As RispostaStandard

        Dim nomeRoutine As String = "AnagrafeBIZ.Ereditatore.ModificaMultipla"
        Dim messaggioErrore As String = ""

        Dim r As New RispostaStandard

        Dim objImpiantoCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)

        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim NoteLog As String = "Operazione effettuata da Modifica Multipla Singola Azienda "

        Using scope As New TransactionScope()

            Try

                Dim objParametri As JObject = JObject.Parse(parametri)
                Dim objDati As JArray = JArray.Parse(dati)

                Dim anagrafica = CStr(objParametri("anagrafica"))
                Dim parametriAnagrafica = objParametri("parametri")
                Dim mod_esercizi = objParametri("mod_esercizi")
                Dim data_esercizi = objParametri("data_esercizi")

                Dim parametriAppezzamento = New List(Of String) From {"5", "27"}
                Dim parametriImpianto = New List(Of String) From {"1", "3", "4", "6", "19", "22", "23", "24", "25", "26", "28"}
                Dim parametriEsercizio = New List(Of String) From {"2", "7", "8", "9", "10", "12", "13", "14", "15", "16", "17", "18", "20", "21", "30"}

                For Each obj As JObject In objDati

                    Dim Piva = CStr(obj("Piva"))
                    Dim Sa_Cod = CInt(obj("Sa_Cod"))
                    Dim Appezza = CInt(obj("Appezza"))
                    Dim Id_Reg = If(obj("Id_Reg") Is Nothing, 0, CInt(obj("Id_Reg")))
                    Dim Progetto_Cod = If(obj("Progetto_Cod") Is Nothing, 0, CInt(obj("Progetto_Cod")))

                    For Each parametro In parametriAnagrafica

                        Select Case anagrafica

                            Case "1" ' modifica appezzamenti

                                If parametriAppezzamento.Contains(parametro) Then
                                    ModificaAppezzamento(Piva, Sa_Cod, Appezza, parametro, obj, objParametriServer, GiasContext, NoteLog)
                                End If

                            Case "2" ' modifica impianti/esercizi

                                If parametriImpianto.Contains(parametro) Then

                                    ' modifica impianti
                                    ModificaImpianto(Piva, Sa_Cod, Appezza, Id_Reg, parametro, obj, objParametriServer, GiasContext, NoteLog)

                                ElseIf parametriEsercizio.Contains(parametro) AndAlso mod_esercizi IsNot Nothing Then

                                    ' filtra esercizi per data validita
                                    Dim Data_Validita = Date.Now.Date
                                    If mod_esercizi = "2" Then
                                        Data_Validita = AGRODATAINIZIO
                                    ElseIf mod_esercizi = "3" AndAlso data_esercizi IsNot Nothing Then
                                        Data_Validita = CDate(data_esercizi.ToString)
                                    End If

                                    ' modifica esercizi
                                    ModificaEsercizio(Piva, Sa_Cod, Appezza, Id_Reg, 0, Data_Validita, parametro, obj, objParametriServer, GiasContext, NoteLog)

                                End If

                            Case "3" ' modifica esercizi

                                If parametriEsercizio.Contains(parametro) Then
                                    ModificaEsercizio(Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, AGRODATAINIZIO, parametro, obj, objParametriServer, GiasContext, NoteLog)
                                End If

                        End Select

                    Next

                Next

                'GiasContext.SaveChanges()

                scope.Complete()
                scope.Dispose()

                r.RispostaOK = True
                r.RispostaStringa = "Salvataggio effettuato correttamente"

            Catch ex As GiasException
                Throw ex
            Catch ex As Exception

                r.RispostaOK = False

                messaggioErrore = ex.Message
                Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)

                r.Errore = "[" & nomeRoutine & "] : " & messaggioErrore

            Finally
                scope.Dispose()
                GiasContext.Dispose()

            End Try

        End Using

        Return r

    End Function

    <Obsolete("Implementata nuova Modifica Multipla Massiva")>
    Public Sub ModificaAppezzamento(ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Appezza As Integer,
                                ByVal parametro As String,
                                ByRef obj As JObject,
                                ByRef objParametri_Server As AgronicaCoreParametri,
                                ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    Optional NoteLog As String = "")

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Ereditatore.ModificaAppezzamento()"
        Dim messaggioErrore As String = ""

        Dim campoModificato As String = ""

        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
        Dim log As Agronica_Log_Anagrafe

        Try

            Dim appezzamento = (From a In GiasContext.Appezzamento Where a.PIVA = Piva AndAlso a.SA_COD = Sa_Cod AndAlso a.APPEZZA = Appezza Select a).FirstOrDefault()

            If appezzamento IsNot Nothing Then

                If parametro = "27" AndAlso obj("Data_Fine_Appezzamento") IsNot Nothing Then
                    Dim validita_fine_old As Date = CDate(appezzamento.Validita_Fine)

                    verificaMovimentieCdG(Piva, Sa_Cod, Appezza, 0, 0, AGRODATAINIZIO, CDate(obj("Data_Fine_Appezzamento").ToString), appezzamento.APP_NOME, objParametri_Server)
                    verifica_ValiditaFineAppezzamento(appezzamento, CDate(obj("Data_Fine_Appezzamento").ToString), objParametri_Server, GiasContext)

                    appezzamento.Validita_Fine = CDate(obj("Data_Fine_Appezzamento").ToString)
                    campoModificato = "Data_Fine_Appezzamento newValue = [" & CDate(obj("Data_Fine_Appezzamento").ToString) & "]"

                    'Modifico le validità di Impianto ed Esercizio solo se la nuova data è più stringente 
                    If validita_fine_old > CDate(obj("Data_Fine_Appezzamento").ToString) Then
                        Dim impianto = (From a In GiasContext.Reg_Impianti Where a.PIVA = Piva AndAlso a.SA_COD = Sa_Cod AndAlso a.APPEZZA = Appezza Select a).FirstOrDefault()

                        verifica_ValiditaFineImpianto(impianto, CDate(obj("Data_Fine_Appezzamento").ToString), objParametri_Server, GiasContext)

                        impianto.Validita_Fine = CDate(obj("Data_Fine_Appezzamento").ToString)
                        impianto.Username_Modifica = objParametri_Server.UsernameOperazione
                        impianto.Data_Modifica = Date.Now

                        GiasContext.Entry(impianto).State = EntityState.Modified
                        GiasContext.SaveChanges()

                        'Scrittura tabella Agronica_Log_Anagrafe
                        log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Impianti,
                                                            CStr(Piva), CStr(Sa_Cod),
                                                            CStr(Appezza), CStr(impianto.ID_REG),
                                                            Nothing, Nothing,
                                                            enum_TipoOperazioneDB.Modifica,
                                                            objParametri_Server,
                                                            enum_Id_Servizio.GiasOnline,
                                                            NoteLog & " (" & campoModificato & ")",
                                                            "")
                        GiasContext.Agronica_Log_Anagrafe.Add(log)
                        GiasContext.SaveChanges()


                        Dim esercizio = (From a In GiasContext.Imprese_Progetti Where a.Piva = Piva AndAlso a.Sa_Cod = Sa_Cod AndAlso a.Appezza = Appezza AndAlso a.Id_Reg = impianto.ID_REG Select a).FirstOrDefault()
                        verifica_ValiditaFineEsercizio(esercizio, CDate(obj("Data_Fine_Appezzamento").ToString), objParametri_Server, GiasContext)

                        esercizio.Validita_Fine = CDate(obj("Data_Fine_Appezzamento").ToString)
                        esercizio.Username_Modifica = objParametri_Server.UsernameOperazione
                        esercizio.Data_Modifica = Date.Now

                        GiasContext.Entry(esercizio).State = EntityState.Modified
                        GiasContext.SaveChanges()

                        log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Progetti,
                                                            CStr(Piva), CStr(esercizio.Progetto_Cod), CStr(Sa_Cod),
                                                            CStr(Appezza), CStr(impianto.ID_REG), Nothing,
                                                            enum_TipoOperazioneDB.Modifica,
                                                            objParametri_Server,
                                                            enum_Id_Servizio.GiasOnline,
                                                            NoteLog & " (" & campoModificato & ")",
                                                            "")

                        GiasContext.Agronica_Log_Anagrafe.Add(log)
                        GiasContext.SaveChanges()
                    End If

                    'AGGIORNO LE PARTICELLE COLLEGATE 
                    Dim particelle = (From a In GiasContext.AppezzamentiXParticelle Where a.PIVA = Piva AndAlso a.SA_COD = Sa_Cod AndAlso a.APPEZZA = Appezza Select a).ToList()
                    For Each particella In particelle
                        particella.Validita_Fine = CDate(obj("Data_Fine_Appezzamento").ToString)

                        GiasContext.Entry(particella).State = EntityState.Modified
                        GiasContext.SaveChanges()
                    Next
                End If


                If parametro = "5" AndAlso obj("MetodoProduzione_Cod") IsNot Nothing Then
                    ModificaAppezzamentoCodice(Piva, Sa_Cod, Appezza,
                    enum_CodiciAnagrafe.MetodoDiProduzione, obj("MetodoProduzione_Cod"),
                    objParametri_Server, GiasContext)
                    campoModificato = "MetodoProduzione_Cod newValue = [id_cod:" & enum_CodiciAnagrafe.MetodoDiProduzione & ", val_cod:" & obj("MetodoProduzione_Cod").ToString() & "]"
                End If

                If parametro = "29" AndAlso obj("nrAppBio") IsNot Nothing Then
                    ModificaAppezzamentoCodice(Piva, Sa_Cod, Appezza,
                    enum_CodiciAnagrafe.Codice_Appezza_Biologico, obj("nrAppBio"),
                    objParametri_Server, GiasContext)
                    campoModificato = "nrAppBio newValue = [id_cod:" & enum_CodiciAnagrafe.Codice_Appezza_Biologico & ", val_cod:" & obj("nrAppBio").ToString() & "]"
                End If

                appezzamento.Username_Modifica = objParametri_Server.UsernameOperazione
                appezzamento.Data_Modifica = Date.Now

                GiasContext.Entry(appezzamento).State = EntityState.Modified
                GiasContext.SaveChanges()

                If campoModificato <> "" Then
                    'Scrittura tabella Agronica_Log_Anagrafe
                    log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Appezza,
                                                        CStr(Piva), CStr(Sa_Cod),
                                                        CStr(Appezza), Nothing,
                                                        Nothing, Nothing,
                                                        enum_TipoOperazioneDB.Modifica,
                                                        objParametri_Server,
                                                        enum_Id_Servizio.GiasOnline,
                                                        NoteLog & " (" & campoModificato & ")",
                                                        "")
                    GiasContext.Agronica_Log_Anagrafe.Add(log)
                    GiasContext.SaveChanges()
                End If
            End If

        Catch ex As GiasException
            'Errore gestito
            Throw ex
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    <Obsolete("Implementata nuova Modifica Multipla Massiva")>
    Public Sub ModificaAppezzamentoCodice(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Appezza As Integer,
                                        ByVal id_cod As Integer,
                                        ByVal val_cod As String,
                                        ByRef objParametriServer As AgronicaCoreParametri,
                                        ByRef GiasContext As Gias_DeveloperServer_Entities)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Ereditatore.ModificaAppezzamentoCodice()"
        Dim messaggioErrore As String = ""

        Try

            If id_cod <> 0 Then

                Dim codici = From c In GiasContext.Appezzamento_Codici
                             Where c.PIVA = Piva AndAlso
                               c.sa_cod = Sa_Cod AndAlso
                               c.appezza = Appezza AndAlso
                               c.id_cod = id_cod
                             Select c

                Dim operazione As enum_TipoOperazioneDB

                If codici.Count > 0 AndAlso (val_cod <> "" AndAlso val_cod IsNot Nothing) Then
                    operazione = enum_TipoOperazioneDB.Modifica
                ElseIf codici.Count > 0 AndAlso (val_cod = "" OrElse val_cod Is Nothing) Then
                    operazione = enum_TipoOperazioneDB.Cancellazione
                ElseIf codici.Count = 0 AndAlso (val_cod = "" OrElse val_cod Is Nothing) Then
                    operazione = enum_TipoOperazioneDB.Lettura
                ElseIf codici.Count = 0 AndAlso (val_cod <> "" AndAlso val_cod IsNot Nothing) Then
                    operazione = enum_TipoOperazioneDB.Scrittura
                End If

                Select Case operazione
                    Case enum_TipoOperazioneDB.Scrittura

                        Dim codice As New Appezzamento_Codici With {
                        .PIVA = Piva,
                        .sa_cod = Sa_Cod,
                        .appezza = Appezza,
                        .id_cod = id_cod,
                        .val_cod = val_cod,
                        .inviato = 0,
                        .Data_Creazione = DateTime.Now,
                        .Data_Modifica = DateTime.Now,
                        .Validita_Inizio = AGRODATAINIZIO,
                        .Validita_Fine = AGRODATAFINE,
                        .Username_Creazione = objParametriServer.UsernameOperazione,
                        .Username_Modifica = objParametriServer.UsernameOperazione
                    }

                        GiasContext.Appezzamento_Codici.Add(codice)

                    Case enum_TipoOperazioneDB.Modifica

                        Dim codice = codici.FirstOrDefault

                        If codice IsNot Nothing Then
                            codice.val_cod = val_cod
                            codice.Data_Modifica = DateTime.Now
                            codice.Username_Modifica = objParametriServer.UsernameOperazione

                            GiasContext.Entry(codice).State = EntityState.Modified
                        End If

                    Case enum_TipoOperazioneDB.Cancellazione

                        Dim codice = codici.FirstOrDefault
                        GiasContext.Appezzamento_Codici.Remove(codice)

                End Select

                GiasContext.SaveChanges()

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    <Obsolete("Implementata nuova Modifica Multipla Massiva")>
    Public Sub ModificaImpianto(ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Appezza As Integer,
                            ByVal Id_Reg As Integer,
                            ByVal parametro As String,
                            ByRef obj As JObject,
                            ByRef objParametri_Server As AgronicaCoreParametri,
                            ByRef GiasContext As Gias_DeveloperServer_Entities,
                                Optional NoteLog As String = "")

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Ereditatore.ModificaImpianto()"
        Dim messaggioErrore As String = ""

        Dim campoModificato As String = ""

        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
        Dim log As Agronica_Log_Anagrafe

        Try

            Dim impianto = (From i In GiasContext.Reg_Impianti Where i.PIVA = Piva AndAlso i.SA_COD = Sa_Cod AndAlso i.APPEZZA = Appezza AndAlso i.ID_REG = Id_Reg Select i).FirstOrDefault()

            If impianto IsNot Nothing Then

                'Dim Resa_Prevista = obj("Resa")
                Dim Finalita = obj("grfi_cod")
                Dim Specie = obj("veg_cod")
                Dim Varieta = obj("cul_cod")
                Dim Gruppo_Varietale = obj("grva_cod")
                Dim Copertura = obj("Cop_Cod")
                Dim ImpIrrigazione = obj("ImpIrrigazione")
                Dim FormaAllevamento = obj("Foral_Cod")
                Dim Portinnesto = obj("Port_Cod")
                Dim Data_Inizio_Impianto = obj("Data_Inizio_Impianto")

                'If parametro = "7" AndAlso Resa_Prevista IsNot Nothing Then
                '    impianto.Resa_Prevista = Resa_Prevista
                'End If

                If parametro = "1" AndAlso Finalita IsNot Nothing Then
                    impianto.GRFI_COD = Finalita
                    campoModificato = "Finalita newValue = [" & Finalita.ToString() & "]"
                End If

                If parametro = "3" AndAlso Varieta IsNot Nothing Then
                    impianto.CUL_COD = Varieta
                    campoModificato = "Varieta newValue = [" & Varieta.ToString() & "]"
                End If

                If parametro = "4" AndAlso Gruppo_Varietale IsNot Nothing Then
                    impianto.GRVA_Cod_VEG = Gruppo_Varietale
                    campoModificato = "Gruppo_Varietale newValue = [" & Gruppo_Varietale.ToString() & "]"
                End If

                If parametro = "6" AndAlso Copertura IsNot Nothing Then
                    impianto.COP_COD = Copertura
                    campoModificato = "Copertura newValue = [" & Copertura.ToString() & "]"
                End If

                If parametro = "19" AndAlso ImpIrrigazione IsNot Nothing Then
                    impianto.IMP_COD = ImpIrrigazione
                    campoModificato = "ImpIrrigazione newValue = [" & ImpIrrigazione.ToString() & "]"
                End If

                If parametro = "24" AndAlso FormaAllevamento IsNot Nothing Then
                    impianto.FORAL_COD = FormaAllevamento
                    campoModificato = "FormaAllevamento newValue = [" & FormaAllevamento.ToString() & "]"
                End If

                If parametro = "25" AndAlso Portinnesto IsNot Nothing Then
                    impianto.PORT_COD = Portinnesto
                    campoModificato = "Portinnesto newValue = [" & Portinnesto.ToString() & "]"
                End If

                If parametro = "26" AndAlso obj("Data_Inizio_Portinnesto") IsNot Nothing Then
                    impianto.Data_Inizio_Portinnesto = CDate(obj("Data_Inizio_Portinnesto").ToString)
                    campoModificato = "Data_Inizio_Portinnesto newValue = [" & CDate(obj("Data_Inizio_Portinnesto").ToString) & "]"
                End If

                If parametro = "28" AndAlso obj("Data_Fine_Impianto") IsNot Nothing Then
                    Dim validita_fine_old As Date = CDate(impianto.Validita_Fine)
                    verificaMovimentieCdG(Piva, Sa_Cod, Appezza, Id_Reg, 0, AGRODATAINIZIO, CDate(obj("Data_Fine_Impianto").ToString), "", objParametri_Server)
                    verifica_ValiditaFineImpianto(impianto, CDate(obj("Data_Fine_Impianto").ToString), objParametri_Server, GiasContext)

                    impianto.Validita_Fine = CDate(obj("Data_Fine_Impianto").ToString)
                    campoModificato = "Data_Fine_Impianto newValue = [" & CDate(obj("Data_Fine_Impianto").ToString) & "]"

                    If validita_fine_old > CDate(obj("Data_Fine_Impianto").ToString) Then
                        Dim esercizio = (From a In GiasContext.Imprese_Progetti Where a.Piva = Piva AndAlso
                                                                                a.Sa_Cod = Sa_Cod AndAlso
                                                                                a.Appezza = Appezza AndAlso
                                                                                a.Id_Reg = impianto.ID_REG
                                         Select a).FirstOrDefault()

                        verifica_ValiditaFineEsercizio(esercizio, CDate(obj("Data_Fine_Impianto").ToString), objParametri_Server, GiasContext)

                        esercizio.Validita_Fine = CDate(obj("Data_Fine_Impianto").ToString)
                        esercizio.Username_Modifica = objParametri_Server.UsernameOperazione
                        esercizio.Data_Modifica = Date.Now

                        GiasContext.Entry(esercizio).State = EntityState.Modified
                        GiasContext.SaveChanges()

                        log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Progetti,
                                                            CStr(Piva), CStr(Sa_Cod),
                                                            CStr(Appezza), CStr(impianto.ID_REG),
                                                            CStr(esercizio.Progetto_Cod), Nothing,
                                                            enum_TipoOperazioneDB.Modifica,
                                                            objParametri_Server,
                                                            enum_Id_Servizio.GiasOnline,
                                                            NoteLog & " (" & campoModificato & ")",
                                                            "")

                        GiasContext.Agronica_Log_Anagrafe.Add(log)
                        GiasContext.SaveChanges()

                    End If
                End If

                If parametro = "40" AndAlso obj("Data_Inizio_Impianto") IsNot Nothing Then
                    impianto.Data_Inizio_Impianto = CDate(obj("Data_Inizio_Impianto").ToString)
                    campoModificato = "Data_Inizio_Impianto newValue = [" & CDate(obj("Data_Inizio_Impianto").ToString) & "]"
                End If

                impianto.Username_Modifica = objParametri_Server.UsernameOperazione
                impianto.Data_Modifica = Date.Now

                GiasContext.Entry(impianto).State = EntityState.Modified
                GiasContext.SaveChanges()

                Dim objImpiantoCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W

                If parametro = "22" AndAlso obj("Su_Fila") IsNot Nothing Then

                    objImpiantoCodici.ScriviModificaEliminaxImpianto(Piva, Sa_Cod, Appezza, Id_Reg,
                    enum_CodiciAnagrafe.Impianto_SuFila_Maschio, CDbl(obj("Su_Fila")), True, objParametri_Server, GiasContext)
                    campoModificato = "Su_Fila newValue = [id_cod:" & enum_CodiciAnagrafe.Impianto_SuFila_Maschio & ", val_cod:" & CDbl(obj("Su_Fila")).ToString() & "]"

                ElseIf parametro = "23" AndAlso obj("Tra_Fila") IsNot Nothing Then

                    objImpiantoCodici.ScriviModificaEliminaxImpianto(Piva, Sa_Cod, Appezza, Id_Reg,
                    enum_CodiciAnagrafe.Impianto_TraFila_Maschio, CDbl(obj("Tra_Fila")), True, objParametri_Server, GiasContext)
                    campoModificato = "Tra_Fila newValue = [id_cod:" & enum_CodiciAnagrafe.Impianto_TraFila_Maschio & ", val_cod:" & CDbl(obj("Tra_Fila")).ToString() & "]"

                    'ElseIf parametro = "26" AndAlso obj("Data_Inizio_Portinnesto") IsNot Nothing Then
                    '    objImpiantoCodici.ScriviModificaEliminaxImpianto(Piva, Sa_Cod, Appezza, Id_Reg,
                    '        enum_CodiciAnagrafe.Data_Inizio_Portinnesto, obj("Data_Inizio_Portinnesto"), True, objParametriServer, GiasContext)
                End If

                impianto.Username_Modifica = objParametri_Server.UsernameOperazione
                impianto.Data_Modifica = Date.Now

                GiasContext.Entry(impianto).State = EntityState.Modified
                GiasContext.SaveChanges()

                If campoModificato <> "" Then
                    'Scrittura tabella Agronica_Log_Anagrafe
                    log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Impianti,
                                                        CStr(Piva), CStr(Sa_Cod),
                                                        CStr(Appezza), CStr(Id_Reg),
                                                        Nothing, Nothing,
                                                        enum_TipoOperazioneDB.Modifica,
                                                        objParametri_Server,
                                                        enum_Id_Servizio.GiasOnline,
                                                        NoteLog & " (" & campoModificato & ")",
                                                        "")
                    GiasContext.Agronica_Log_Anagrafe.Add(log)
                    GiasContext.SaveChanges()


                End If

            End If

        Catch ex As GiasException
            'Errore gestito
            Throw ex
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    <Obsolete("Implementata nuova Modifica Multipla Massiva")>
    Public Sub ModificaEsercizio(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Appezza As Integer,
                             ByVal Id_Reg As Integer,
                             ByVal Progetto_Cod As Integer,
                             ByVal Data_Validita As Date,
                             ByVal parametro As String,
                             ByRef obj As JObject,
                             ByRef objParametri_Server As AgronicaCoreParametri,
                             ByRef GiasContext As Gias_DeveloperServer_Entities,
                                Optional NoteLog As String = "")

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Ereditatore.ModificaEsercizio()"
        Dim messaggioErrore As String = ""

        Dim campoModificato As String = ""

        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
        Dim log As Agronica_Log_Anagrafe

        Try

            Dim esercizi = (From e In GiasContext.Imprese_Progetti
                            Where e.Piva = Piva And e.Sa_Cod = Sa_Cod And e.Appezza = Appezza And e.Id_Reg = Id_Reg AndAlso
                            (Progetto_Cod = 0 OrElse e.Progetto_Cod = Progetto_Cod) AndAlso
                            (Data_Validita = AGRODATAINIZIO OrElse (e.Validita_Inizio <= Data_Validita AndAlso e.Validita_Fine >= Data_Validita))
                            Select e).ToList()

            For Each esercizio In esercizi

                Dim distinta_chiusa = (From cc In GiasContext.Reg_Impianti_Codici
                                       Where cc.PIVA = esercizio.Piva AndAlso cc.sa_cod = esercizio.Sa_Cod AndAlso
                                         cc.appezza = esercizio.Appezza AndAlso cc.Id_Reg = esercizio.Id_Reg AndAlso
                                         cc.Progetto_Cod = esercizio.Progetto_Cod AndAlso cc.id_cod = enum_CodiciAnagrafe.Distinta_Chiusa
                                       Select cc).FirstOrDefault()

                ' modifico esercizio se non è stato chiuso
                If distinta_chiusa Is Nothing OrElse distinta_chiusa.val_cod <> "1" Then

                    Dim objImpiantoCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W

                    If parametro = "12" AndAlso obj("CapitolatoPrivato") IsNot Nothing Then

                        objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                        enum_CodiciAnagrafe.Capitolato_Privato, obj("CapitolatoPrivato"), True, objParametri_Server, GiasContext)
                        campoModificato = "CapitolatoPrivato newValue = [" & obj("CapitolatoPrivato").ToString() & "]"

                    ElseIf parametro = "13" AndAlso obj("OrganismoReferente") IsNot Nothing Then

                        objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                            enum_CodiciAnagrafe.Organismo_Referente, obj("OrganismoReferente"), True, objParametri_Server, GiasContext)
                        campoModificato = "OrganismoReferente newValue = [" & obj("OrganismoReferente").ToString() & "]"

                    ElseIf parametro = "14" AndAlso obj("MagazzinoConferimento") IsNot Nothing Then

                        objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                            enum_CodiciAnagrafe.Magazzino_Conferimento, obj("MagazzinoConferimento"), True, objParametri_Server, GiasContext)
                        campoModificato = "MagazzinoConferimento newValue = [" & obj("MagazzinoConferimento").ToString() & "]"

                    ElseIf parametro = "15" AndAlso obj("Certificazione") IsNot Nothing Then

                        objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                            enum_CodiciAnagrafe.Certificazione, obj("Certificazione"), True, objParametri_Server, GiasContext)
                        campoModificato = "Certificazione newValue = [" & obj("Certificazione").ToString() & "]"

                    ElseIf parametro = "16" AndAlso obj("N") IsNot Nothing Then

                        Dim N As String = If(IsNumeric((obj("N"))), CDbl(obj("N")), "")
                        objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                            enum_CodiciAnagrafe.Impianto_LimiteN, N, True, objParametri_Server, GiasContext)
                        campoModificato = "N newValue = [" & If(N.ToString() = "", "NULL", N.ToString()) & "]"

                    ElseIf parametro = "17" AndAlso obj("P") IsNot Nothing Then

                        Dim P As String = If(IsNumeric((obj("P"))), CDbl(obj("P")), "")
                        objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                            enum_CodiciAnagrafe.Impianto_LimiteP, P, True, objParametri_Server, GiasContext)
                        campoModificato = "P newValue = [" & If(P.ToString() = "", "NULL", P.ToString()) & "]"

                    ElseIf parametro = "18" AndAlso obj("K") IsNot Nothing Then

                        Dim K As String = If(IsNumeric((obj("K"))), CDbl(obj("K")), "")
                        objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                            enum_CodiciAnagrafe.Impianto_LimiteK, K, True, objParametri_Server, GiasContext)
                        campoModificato = "K newValue = [" & If(K.ToString() = "", "NULL", K.ToString()) & "]"

                    Else

                        If parametro = "7" AndAlso obj("Resa") IsNot Nothing Then
                            esercizio.Produzione_Prevista = CDbl(obj("Resa"))
                            campoModificato = "Resa newValue = [" & obj("Resa").ToString() & "]"
                        End If

                        If parametro = "8" AndAlso obj("Data_Semina") IsNot Nothing Then
                            Dim data = obj("Data_Semina").ToString

                            If data <> "" Then
                                esercizio.Data_Inizio_Prevista = CDate(obj("Data_Semina").ToString)
                                campoModificato = "Data_Semina newValue = [" & CDate(obj("Data_Semina").ToString) & "]"
                            Else
                                esercizio.Data_Inizio_Prevista = AGRODATAINIZIO
                                campoModificato = "Data_Semina newValue = [" & AGRODATAINIZIO.ToString() & "]"
                            End If

                        End If

                        If parametro = "9" AndAlso obj("Data_Raccolta") IsNot Nothing Then
                            Dim data = obj("Data_Raccolta").ToString

                            If data <> "" Then
                                esercizio.Data_Fine_Prevista = CDate(obj("Data_Raccolta").ToString)
                                campoModificato = "Data_Raccolta newValue = [" & CDate(obj("Data_Raccolta").ToString) & "]"
                            Else
                                esercizio.Data_Fine_Prevista = AGRODATAFINE
                                campoModificato = "Data_Raccolta newValue = [" & AGRODATAFINE.ToString() & "]"
                            End If
                        End If

                        If parametro = "10" AndAlso obj("Data_Fioritura") IsNot Nothing Then
                            Dim data = obj("Data_Fioritura").ToString

                            If data <> "" Then
                                esercizio.Data_Fioritura_Prevista = CDate(obj("Data_Fioritura").ToString)
                                campoModificato = "Data_Fioritura newValue = [" & CDate(obj("Data_Fioritura").ToString) & "]"
                            Else
                                esercizio.Data_Fioritura_Prevista = AGRODATAFINE
                                campoModificato = "Data_Fioritura newValue = [" & AGRODATAFINE.ToString() & "]"
                            End If
                        End If

                        If parametro = "20" AndAlso obj("Reg_Cod") IsNot Nothing Then
                            esercizio.Regolamento_Cod = obj("Reg_Cod")
                            campoModificato = "Reg_Cod newValue = [" & obj("Reg_Cod").ToString() & "]"
                        End If

                        If parametro = "21" AndAlso obj("Dpi_Cod") IsNot Nothing AndAlso obj("Flag_PubblicoPrivato") IsNot Nothing Then
                            esercizio.Disciplinare_Cod = obj("Dpi_Cod")
                            esercizio.Disciplinare_PubblicoPrivato = obj("Flag_PubblicoPrivato")
                            campoModificato = "Dpi_Cod e Flag_PubblicoPrivato newValue = [" & obj("Dpi_Cod").ToString() & ", " & obj("Flag_PubblicoPrivato").ToString() & "]"
                        End If

                        If parametro = "30" AndAlso obj("FlagSecondoRaccolto") IsNot Nothing Then
                            esercizio.FlagSecondoRaccolto = obj("FlagSecondoRaccolto")
                            campoModificato = "FlagSecondoRaccolto newValue = [" & obj("FlagSecondoRaccolto").ToString() & "]"
                        End If

                        If parametro = "31" AndAlso obj("CertificazioneAziendale") IsNot Nothing Then
                            objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                            enum_CodiciAnagrafe.Codice_Certificazione, CStr(obj("CertificazioneAziendale")), True, objParametri_Server, GiasContext)
                            campoModificato = "CertificazioneAziendale newValue = [" & obj("CertificazioneAziendale").ToString() & "]"
                        End If

                        If parametro = "32" AndAlso obj("Contributi") IsNot Nothing Then
                            objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                            enum_CodiciAnagrafe.Contributi, CStr(obj("Contributi")), True, objParametri_Server, GiasContext)
                            campoModificato = "Contributi newValue = [" & obj("Contributi").ToString() & "]"
                        End If

                        If parametro = "33" AndAlso obj("CertificazioneProdotto") IsNot Nothing Then
                            objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                            enum_CodiciAnagrafe.Codice_Certificazione_Prodotto, CStr(obj("CertificazioneProdotto")), True, objParametri_Server, GiasContext)
                            campoModificato = "CertificazioneProdotto newValue = [" & obj("CertificazioneProdotto").ToString() & "]"
                        End If

                        If parametro = "34" AndAlso obj("Residuo") IsNot Nothing Then
                            objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                            enum_CodiciAnagrafe.Codice_Residuo, CStr(obj("Residuo")), True, objParametri_Server, GiasContext)
                            campoModificato = "Residuo newValue = [" & obj("Residuo").ToString() & "]"
                        End If

                        If parametro = "35" AndAlso obj("LicenzaColtivazione") IsNot Nothing Then
                            objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                            enum_CodiciAnagrafe.Zespri_Fasi_Fase, CStr(obj("LicenzaColtivazione")), True, objParametri_Server, GiasContext)
                            campoModificato = "LicenzaColtivazione newValue = [" & obj("LicenzaColtivazione").ToString() & "]"
                        End If

                        If parametro = "36" AndAlso obj("RiferimentoTrasferimentoDati") IsNot Nothing Then
                            objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                            enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati, CStr(obj("RiferimentoTrasferimentoDati")), True, objParametri_Server, GiasContext)
                            campoModificato = "RiferimentoTrasferimentoDati newValue = [" & obj("RiferimentoTrasferimentoDati").ToString() & "]"
                        End If

                        If parametro = "37" AndAlso obj("Tecnico") IsNot Nothing Then
                            objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                            enum_CodiciAnagrafe.Tecnico, CStr(obj("Tecnico")), True, objParametri_Server, GiasContext)
                            campoModificato = "Tecnico newValue = [" & obj("Tecnico").ToString() & "]"
                        End If

                        If parametro = "38" AndAlso obj("PianoSemina") IsNot Nothing Then
                            objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                            enum_CodiciAnagrafe.Impianto_PianoSemina, CStr(obj("PianoSemina")), True, objParametri_Server, GiasContext)
                            campoModificato = "PianoSemina newValue = [" & obj("PianoSemina").ToString() & "]"
                        End If

                        If parametro = "39" AndAlso obj("Prodotto") IsNot Nothing Then
                            esercizio.Mat_Cod = obj("Prodotto")
                            campoModificato = "Mat_Cod newValue = [" & obj("Prodotto").ToString() & "]"
                        End If

                        If parametro = "2" Then

                            Dim Metodo_Produzione = obj("MetodoProduzione_Cod")
                            Dim Disciplinare = obj("Disciplinare")
                            Dim Regolamento_Cod = obj("Reg_Cod")
                            Dim Disciplinare_Cod = obj("Dpi_Cod")
                            Dim Regolamento_Concimazione_Cod = obj("Regolamento_Concimazione_Cod")
                            Dim Flag_PubblicoPrivato = obj("Flag_PubblicoPrivato")
                            Dim StatoImpianto_Cod = obj("StatoImpianto_Cod")
                            Dim Finalita_Concimazione_Impianto = obj("Finalita_Concimazione_Impianto")
                            Dim IAF = obj("IAF")
                            Dim N = obj("N")
                            Dim P = obj("P")
                            Dim K = obj("K")

                            ' forza metodo produzione appezzamento
                            'If Metodo_Produzione IsNot Nothing Then
                            '    ModificaAppezzamentoCodice(Piva, Sa_Cod, Appezza, enum_CodiciAnagrafe.MetodoDiProduzione, Metodo_Produzione, objParametriServer, GiasContext)
                            'End If

                            If Regolamento_Cod IsNot Nothing Then
                                esercizio.Regolamento_Cod = Regolamento_Cod
                                campoModificato = "Regolamento_Cod newValue = [" & Regolamento_Cod.ToString() & "]"
                            End If

                            If Disciplinare_Cod IsNot Nothing Then
                                esercizio.Disciplinare_Cod = Disciplinare_Cod
                                campoModificato = "Disciplinare_Cod newValue = [" & Disciplinare_Cod.ToString() & "]"
                            End If

                            If Flag_PubblicoPrivato IsNot Nothing Then
                                esercizio.Disciplinare_PubblicoPrivato = Flag_PubblicoPrivato
                                campoModificato = "Flag_PubblicoPrivato newValue = [" & Flag_PubblicoPrivato.ToString() & "]"
                            End If

                            If Regolamento_Concimazione_Cod IsNot Nothing Then
                                esercizio.Regolamento_Concimazioni_Cod = Regolamento_Concimazione_Cod
                                campoModificato = "Regolamento_Concimazione_Cod newValue = [" & Regolamento_Concimazione_Cod.ToString() & "]"
                            End If

                            If StatoImpianto_Cod IsNot Nothing Then
                                esercizio.Stato_Impianto = StatoImpianto_Cod
                                campoModificato = "StatoImpianto_Cod newValue = [" & StatoImpianto_Cod.ToString() & "]"
                            End If

                            If Finalita_Concimazione_Impianto IsNot Nothing Then
                                objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                                enum_CodiciAnagrafe.Finalita_Concimazione_Impianto, Finalita_Concimazione_Impianto, True, objParametri_Server, GiasContext)
                                campoModificato = "Finalita_Concimazione_Impianto newValue = [id_cod:" & enum_CodiciAnagrafe.Finalita_Concimazione_Impianto & ", val_cod:" & Finalita_Concimazione_Impianto.ToString() & "]"
                            End If

                            If IAF IsNot Nothing Then
                                objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                                enum_CodiciAnagrafe.Impianto_IAF_ImpegniAggiuntiviFacoltativi, IAF, True, objParametri_Server, GiasContext)
                                campoModificato = "IAF newValue = [id_cod:" & enum_CodiciAnagrafe.Impianto_IAF_ImpegniAggiuntiviFacoltativi & ", val_cod:" & IAF.ToString() & "]"
                            End If

                            If N IsNot Nothing Then
                                objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                                enum_CodiciAnagrafe.Impianto_LimiteN, N, True, objParametri_Server, GiasContext)
                                campoModificato = "N newValue = [id_cod:" & enum_CodiciAnagrafe.Impianto_LimiteN & ", val_cod:" & N.ToString() & "]"
                            End If

                            If P IsNot Nothing Then
                                objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                                enum_CodiciAnagrafe.Impianto_LimiteP, P, True, objParametri_Server, GiasContext)
                                campoModificato = "P newValue = [id_cod:" & enum_CodiciAnagrafe.Impianto_LimiteP & ", val_cod:" & P.ToString() & "]"
                            End If

                            If K IsNot Nothing Then
                                objImpiantoCodici.ScriviModificaEliminaxProgetto(Piva, Sa_Cod, Appezza, Id_Reg, esercizio.Progetto_Cod,
                                enum_CodiciAnagrafe.Impianto_LimiteK, K, True, objParametri_Server, GiasContext)
                                campoModificato = "K newValue = [id_cod:" & enum_CodiciAnagrafe.Impianto_LimiteK & ", val_cod:" & K.ToString() & "]"
                            End If

                        End If

                        esercizio.Username_Modifica = objParametri_Server.UsernameOperazione
                        esercizio.Data_Modifica = Date.Now

                        GiasContext.Entry(esercizio).State = EntityState.Modified
                        GiasContext.SaveChanges()

                    End If

                End If


                If campoModificato <> "" Then
                    'Scrittura tabella Agronica_Log_Anagrafe
                    log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Progetti,
                                                        CStr(esercizio.Piva), CStr(esercizio.Progetto_Cod),
                                                        CStr(esercizio.Sa_Cod), CStr(esercizio.Appezza),
                                                        CStr(esercizio.Id_Reg), Nothing,
                                                        enum_TipoOperazioneDB.Scrittura,
                                                        objParametri_Server,
                                                        enum_Id_Servizio.GiasOnline,
                                                        NoteLog & " (" & campoModificato & ")",
                                                        "")
                    GiasContext.Agronica_Log_Anagrafe.Add(log)
                    GiasContext.SaveChanges()

                End If

            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    <Obsolete("Implementata nuova Modifica Multipla Massiva")>
    Private Sub verificaMovimentieCdG(piva As String,
                                 sa_cod As Integer,
                                 appezza As Integer,
                                 id_reg As Integer,
                                 progetto_cod As Integer,
                                 Validita_Inizio As Date,
                                 Validita_Fine As Date,
                                 descrizione As String,
                                 ByRef objParametri_Server As AgronicaCoreParametri)
        Try
            Dim objControlloAgenda As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
            objControlloAgenda.controllo_MovimentiRicettePua(Nothing, piva, sa_cod, appezza, id_reg, Validita_Inizio, Validita_Fine, objParametri_Server)

            'controllo COSTI DI GESTIONE su qualsiasi Esercizio collegato
            Dim objControlloCdG As New AgronicaCoreAnagrafeBIZ.Progetto_W
            Dim controlloCdG = objControlloCdG.controllo_CdG(Nothing, piva, sa_cod, appezza, id_reg, progetto_cod, Validita_Inizio, Validita_Fine, objParametri_Server)
            If controlloCdG.errore Then
                Dim MessaggioErroreCdG As String = ""

                If Not controlloCdG.messaggioSpecifico Then
                    MessaggioErroreCdG &= ("Non è possibile modificare la " & controlloCdG.inizio_fine & " dell'appezzamento " & descrizione & ", perché sono stati associati Costi di Gestione ad un esercizio in data successiva a quella selezionata")
                Else
                    MessaggioErroreCdG &= ("Non è possibile modificare la " & controlloCdG.inizio_fine & " dell'appezzamento " & descrizione & ", perché sono stati associati Costi di Gestione ad un esercizio in data " & controlloCdG.dataCdG)
                End If

                Throw New GiasException(MessaggioErroreCdG)
            End If

        Catch ex As GiasException
            'Errore gestito
            Throw ex
        End Try
    End Sub

    <Obsolete("Implementata nuova Modifica Multipla Massiva")>
    Private Sub verifica_ValiditaFineEsercizio(esercizio As Imprese_Progetti,
                                               validita_fine As Date,
                                               ByRef objParametri_Server As AgronicaCoreParametri,
                                               ByRef GiasContext As Gias_DeveloperServer_Entities)

        Dim nomeRoutine = "verifica_ValiditaFineEsercizio()"

        Try
            Dim impianto = (From a In GiasContext.Reg_Impianti Where a.PIVA = esercizio.Piva AndAlso a.SA_COD = esercizio.Sa_Cod AndAlso a.APPEZZA = esercizio.Appezza AndAlso a.ID_REG = esercizio.Id_Reg Select a).FirstOrDefault()
            Dim nomeImpianto As String = "(" & esercizio.Validita_Inizio & "-" & esercizio.Validita_Fine & ")"

            If validita_fine > CDate(impianto.Validita_Fine) Then
                Throw New GiasException(("La data di fine dell'esercizio non può essere superiore alla data di fine dell'impianto") & " (" & impianto.Validita_Fine & ").")
            End If

            If validita_fine < esercizio.Validita_Inizio Then
                Throw New GiasException("La fine dell'esercizio non può precedere la data di inizio (" & CDate(esercizio.Validita_Inizio).ToShortDateString & ").")
            End If

            'Verifico che non ci siano altri esercizi
            Dim dicEsercizi = (From a In GiasContext.Imprese_Progetti Where a.Piva = esercizio.Piva AndAlso
                                                                       a.Sa_Cod = esercizio.Sa_Cod AndAlso
                                                                       a.Appezza = esercizio.Appezza AndAlso
                                                                       a.Id_Reg = esercizio.Id_Reg AndAlso
                                                                       a.Progetto_Cod <> esercizio.Progetto_Cod AndAlso
                                                                       validita_fine > a.Validita_Inizio Select a).ToList()
            If dicEsercizi.Count > 0 Then
                Throw New GiasException(DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/GestioneEsercizi.aspx",
                    "ImpossibileEseguireOperazioneSuEserciziConDateSovrapposteConAltri"), String))
            End If

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Sub

    <Obsolete("Implementata nuova Modifica Multipla Massiva")>
    Private Sub verifica_ValiditaFineImpianto(impianto As Reg_Impianti,
                                          validita_fine As Date,
                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                          ByRef GiasContext As Gias_DeveloperServer_Entities)

        Const nomeRoutine = "verifica_ValiditaFineImpianto()"

        Try
            Dim appezzamento = (From a In GiasContext.Appezzamento Where a.PIVA = impianto.PIVA AndAlso a.SA_COD = impianto.SA_COD AndAlso a.APPEZZA = impianto.APPEZZA Select a).FirstOrDefault()
            Dim nomeImpianto As String = "(" & impianto.Validita_Inizio & "-" & impianto.Validita_Fine & ")"

            If validita_fine > CDate(appezzamento.Validita_Fine) Then
                Throw New GiasException(("La data di fine dell'impianto '" & nomeImpianto & "' non può essere superiore alla data di fine dell'appezzamento '") & appezzamento.APP_NOME & "'" & " (" & appezzamento.Validita_Fine & ").")
            End If

            If validita_fine < impianto.Validita_Inizio Then
                Throw New GiasException("La fine dell'impianto non può precedere la data di inizio (" & CDate(impianto.Validita_Inizio).ToShortDateString & ").")
            End If

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Sub

    <Obsolete("Implementata nuova Modifica Multipla Massiva")>
    Public Sub verifica_ValiditaFineAppezzamento(appezzamento As Appezzamento,
                                              validita_fine As Date,
                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                              ByRef GiasContext As Gias_DeveloperServer_Entities)

        Const nomeRoutine = "verifica_ValiditaFineAppezzamento()"
        Try
            Dim centro = (From a In GiasContext.Centri_Aziendali Where a.PIVA = appezzamento.PIVA AndAlso a.sa_cod = appezzamento.SA_COD Select a).FirstOrDefault()

            If centro Is Nothing Then
                Throw New GiasException("Centro Aziendale non trovato.")
            End If

            If validita_fine > CDate(centro.Validita_Fine) Then
                Throw New GiasException(("La fine dell'appezzamento '" & appezzamento.APP_NOME & "' non può seguire la cessazione del Centro Aziendale '") & centro.sa_nome & "' (" & CDate(centro.Validita_Fine).ToShortDateString & ").")
            End If

            If validita_fine < appezzamento.Validita_Inizio Then
                Throw New GiasException("La fine dell'appezzamento '" & appezzamento.APP_NOME & "' non può precedere la sua data di inizio" & " (" & CDate(appezzamento.Validita_Inizio).ToShortDateString & ").")
            End If

            Dim campo = (From a In GiasContext.Campi Where a.Piva = appezzamento.PIVA AndAlso a.Sa_Cod = appezzamento.SA_COD AndAlso a.Campo_Cod = appezzamento.Campo_Cod Select a).FirstOrDefault()
            If campo IsNot Nothing Then
                If validita_fine > CDate(campo.Validita_Fine) Then
                    Throw New GiasException(("La fine dell'appezzamento '" & appezzamento.APP_NOME & "' non può seguire la cessazione del Campo '") & campo.Campo_Des & "' (" & CDate(campo.Validita_Fine).ToShortDateString & ").")
                End If
            End If
        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Sub

    Public Function editResaPrevista(
                                ByVal eserciziObj As String,
                                ByVal objParametriServer As AgronicaCoreParametri,
                                ByVal objParametriUtenti As AgronicaCoreParametri,
                                ByVal objParametriSuperServer As AgronicaCoreParametri,
                                Optional NoteLog As String = ""
                                )

        Dim nomeRoutine = "AgronicaCoreAnagrafeBIZ.Ereditatore.editResaPrevista()"

        Dim esercizi = JArray.Parse(eserciziObj)
        Dim datiPrevisionaliObj As New DatiPrevisionaliColtureRead

        Dim listaSpecie As New List(Of String)

        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean

        Try
            Dim listaChiaviEsercizi = PreparaListaEsercizi(esercizi).ToList()

            Dim objImpreseProgetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim dtEsercizi As DataTable = objImpreseProgetti.Leggi_per_EditResaPrevista(listaChiaviEsercizi, objParametriServer)

            Dim resaImpianti As New Dictionary(Of Decimal, List(Of (String, Integer, Integer, Integer, Integer)))
            Dim campoModificato As String = ""
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim currentSpecie As String = ""

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametriServer, System.Data.IsolationLevel.ReadUncommitted)

            For Each ese In dtEsercizi.Rows
                Try
                    Dim piva As String = CStr(ese("PIVA"))
                    Dim sa_cod As Integer = CInt(ese("sa_cod"))
                    Dim appezza As Integer = CInt(ese("Appezza"))
                    Dim id_reg As Integer = CInt(ese("id_Reg"))
                    Dim progettoCod As Integer = CInt(ese("Progetto_Cod"))
                    currentSpecie = ese("veg_des")

                    Dim resa = datiPrevisionaliObj.readDatiPrevisionaliForImpianto_ModificaMultipla(ese, objParametriServer)

                    Dim produzionePrevista As Double = convertUoM(resa.valore, enum_UnitaMisura.KG__HA, resa.udm.codice)

                    If resaImpianti.ContainsKey(produzionePrevista) Then
                        resaImpianti(produzionePrevista).Add((piva, sa_cod, appezza, id_reg, progettoCod))
                    Else
                        resaImpianti.Add(produzionePrevista, New List(Of (String, Integer, Integer, Integer, Integer)) From {(piva, sa_cod, appezza, id_reg, progettoCod)})
                    End If

                Catch ex As DatiPrevisionaliNoDataFoundException
                    listaSpecie.Add(currentSpecie)
                Catch ex As Exception
                    Throw ex
                End Try
            Next

            Dim data = Date.Now
            Dim objEsercizio As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W
            Dim objLogAnagrafe As New AgronicaLogAnagrafe_W
            For Each resa In resaImpianti
                objEsercizio.UpdateColonna_Massivo(resa.Value, "produzione_prevista", resa.Key, "number", data, objParametriServer)

                Dim dummy As String = $"Operazione effettuata da Modifica Multipla Piano Colturale(resa newValue = [{resa.Key}])"
                objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                       Nothing,
                                       listaChiaviEsercizi,
                                       Enum_EntitaModificaMultiplaPianoColturale.Esercizi,
                                       data,
                                       dummy,
                                       objParametriServer)
            Next

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametriServer)

        Catch ex As Exception
            If objParametriServer.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametriServer)
            End If

            Scrivi_LOG(objParametriServer, nomeRoutine, ex.Message)

            Throw New Exception(ex.Message)

        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametriServer)
        End Try

        If listaSpecie.Any() Then
            Dim messaggio = Gias.NessunaResaTrovataPerSpecie + ": " + String.Join(", ", listaSpecie.Distinct())
            Throw New GiasException(messaggio)
        End If

        Return True
    End Function

    Private Function convertUoM(
                           ByVal value As Double,
                           ByVal uomToCod As Integer,
                           ByVal uomFromCod As Integer
                           ) As Double
        Select Case uomToCod
            Case enum_UnitaMisura.KG__HA
                Select Case uomFromCod
                    Case enum_UnitaMisura.QUINTALI__HA
                        Return value * 100
                    Case Else
                        Return value
                End Select
            Case Else
                Return value
        End Select
    End Function

    'Private Function GetSpecieDescriptions(
    '                                      ByVal species As HashSet(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescr),
    '                                      ByRef GiasContext As Gias_DeveloperServer_Entities
    '                                      ) As List(Of String)
    '    Dim descriptions As New List(Of String)
    '    For Each specie In species
    '        Dim specieVegetale = (
    '            From e In GiasContext.SpecieVegetali
    '            Where e.Veg_Cod = specie.codice
    '                ).FirstOrDefault

    '        descriptions.Add(specieVegetale.Veg_Des)
    '    Next
    '    Return descriptions
    'End Function


#Region "NEW - Massivo"
    Public Function ModificaMultipla_PianoColturale_NEW(ByVal parametri As String, ByVal dati As String, ByRef objParametriServer As AgronicaCoreParametri) As RispostaStandard
        Dim nomeRoutine As String = "AnagrafeBIZ.Ereditatore.ModificaMultipla_PianoColturale"
        Dim messaggioErrore As String = ""

        Dim r As New RispostaStandard

        Dim objImpiantoCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W

        Dim NoteLog As String = "Operazione effettuata da Modifica Multipla Piano Colturale"

        Try

            Dim objParametri As JObject = JObject.Parse(parametri)
            Dim objDati As JArray = JArray.Parse(dati)

            Dim entitaModifica = objParametri("anagrafica")

            Dim parametri_modificati = objParametri("parametri_modificati")
            Dim valore_parametri_modificati = objParametri("valore_parametri_modificati")

            Dim mod_esercizi = objParametri("mod_esercizi")
            Dim data_esercizi = objParametri("data_esercizi")

            PopolaDictionaryEntita(objDati)

            If dicEsercizi.Count > 0 Then
                Dim errore As String = PopolaDatiAnagrafici(parametri_modificati, mod_esercizi, data_esercizi, objParametriServer)
                If errore <> "" Then
                    r.ErroriGias.Add(New ErroreGias With {.messaggio = errore})
                End If
            End If

            If dicEsercizi.Count > 0 Then
                For Each parametro In parametri_modificati
                    Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean

                    Try
                        ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametriServer, System.Data.IsolationLevel.ReadUncommitted)

                        If parametriAppezzamento.Contains(parametro) Then
                            Dim errore As String = ModificaAppezzamenti_NEW(parametro, valore_parametri_modificati, objParametriServer, NoteLog)
                            If errore <> "" Then
                                r.ErroriGias.Add(New ErroreGias With {.messaggio = errore})
                            End If
                        End If

                        If parametriImpianto.Contains(parametro) Then
                            ' modifica impianti
                            Dim errore As String = ModificaImpianti_NEW(parametro, valore_parametri_modificati, objParametriServer, NoteLog)
                            If errore <> "" Then
                                r.ErroriGias.Add(New ErroreGias With {.messaggio = errore})
                            End If
                        End If

                        If parametriEsercizio.Contains(parametro) Then
                            Dim errore As String = ModificaEsercizi_NEW(parametro, valore_parametri_modificati, objParametriServer, NoteLog)
                            If errore <> "" Then
                                r.ErroriGias.Add(New ErroreGias With {.messaggio = errore})
                            End If
                        End If

                        ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametriServer)

                    Catch ex As Exception
                        If objParametriServer.objTransazione IsNot Nothing Then
                            ConnessioniTransazioni.ChiudiTransazione(2, objParametriServer)
                        End If
                        Throw New Exception(ex.Message)
                    Finally
                        ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametriServer)
                    End Try
                Next
                r.RispostaOK = True
                r.RispostaStringa = "Salvataggio effettuato correttamente"
            Else
                r.RispostaOK = False
            End If


        Catch ex As GiasException
            r.RispostaOK = False
            messaggioErrore = ex.Message
            r.ErroriGias.Add(New ErroreGias With {.messaggio = messaggioErrore})
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Catch ex As Exception
            r.RispostaOK = False
            messaggioErrore = ex.Message
            r.Errore = "[" & nomeRoutine & "] : " & messaggioErrore
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally
            If objParametriServer.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametriServer)
            End If
        End Try

        Return r

    End Function
    Private Sub PopolaDictionaryEntita(objDati As JArray)

        'Estraggo tutte le chiavi degli esercizi selezionati
        For Each obj As JObject In objDati
            Dim Piva As String = CStr(obj("PIVA"))
            Dim Sa_Cod As Integer = CInt(obj("sa_cod"))
            Dim Appezza As Integer = CInt(obj("Appezza"))
            Dim Id_Reg As Integer = CInt(obj("id_Reg"))
            Dim Progetto_Cod As Integer = CInt(obj("Progetto_Cod"))

            Dim keyAppezzamento As New keyAppezzamento With {.Piva = Piva, .Sa_Cod = Sa_Cod, .Appezza = Appezza}
            Dim keyImpianto As New keyImpianto With {.Piva = Piva, .Sa_Cod = Sa_Cod, .Appezza = Appezza, .Id_Reg = Id_Reg}
            Dim keyEsercizio As New keyEsercizio With {.Piva = Piva, .Sa_Cod = Sa_Cod, .Appezza = Appezza, .Id_Reg = Id_Reg, .Progetto_Cod = Progetto_Cod}
            'Creo i dictionary a scalare per appezzamento, impianto, esercizio
            'Le chiavi mi arrivano ordinate per piva, sa_cod, appezza, id_reg, validita_fine_esercizio (non posso basarmi sul progetto_cod perchè da interfaccia potrei fare cose strane, ad esempio creare un esercizio più vecchio in un secondo momento)
            If Not dicAppezzamenti.ContainsKey(keyAppezzamento) Then
                dicAppezzamenti.Add(keyAppezzamento, Nothing)
            End If
            If Not dicImpianti.ContainsKey(keyImpianto) Then
                dicImpianti.Add(keyImpianto, Nothing)
            End If

            If Not dicEsercizi.ContainsKey(keyEsercizio) Then
                dicEsercizi.Add(keyEsercizio, Nothing)
            End If
        Next
    End Sub
    Private Function PreparaListaEsercizi(objDati As JArray) As List(Of (String, Integer, Integer, Integer, Integer))

        Dim listaChiaviEsercizi = New List(Of (String, Integer, Integer, Integer, Integer))()

        'Estraggo tutte le chiavi degli esercizi selezionati
        For Each obj As JObject In objDati
            Dim Piva As String = CStr(obj("PIVA"))
            Dim Sa_Cod As Integer = CInt(obj("sa_cod"))
            Dim Appezza As Integer = CInt(obj("Appezza"))
            Dim Id_Reg As Integer = CInt(obj("id_Reg"))
            Dim Progetto_Cod As Integer = CInt(obj("Progetto_Cod"))

            Dim keyEsercizio As (String, Integer, Integer, Integer, Integer) = (Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod)

            If Not listaChiaviEsercizi.Contains(keyEsercizio) Then
                listaChiaviEsercizi.Add(keyEsercizio)
            End If

        Next

        Return listaChiaviEsercizi

    End Function
    Private Function PopolaDatiAnagrafici(parametri_modificati As JToken,
                                          mod_esercizi As JToken,
                                          data_esercizi As JToken,
                                          objParametriServer As AgronicaCoreParametri) As String

        Dim msgEserciziNonModificabili As String = ""
        Dim eserciziNonModificabiliChiusi As New List(Of String)

        Dim listaChiaviEsercizi = dicEsercizi.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza), CInt(item.Key.Id_Reg), CInt(item.Key.Progetto_Cod))).ToList()

        'Se stiamo modificando parametri di esercizi, verifico il tipo di filtro data inserito
        Dim Data_Validita = AGRODATAINIZIO
        Dim modEsercizi As Integer = Enum_ModEserciziModificaMultiplaPianoColturale.TuttiGliEsercizi
        Dim almenoUnParametroEsercizio As Boolean = False
        For Each parametro In parametri_modificati
            If parametriEsercizio.Contains(parametro) AndAlso mod_esercizi IsNot Nothing Then
                Select Case mod_esercizi.ToString()
                    Case Enum_ModEserciziModificaMultiplaPianoColturale.AttiviAllaDataOdierna
                        modEsercizi = Enum_ModEserciziModificaMultiplaPianoColturale.AttiviAllaDataOdierna
                        Data_Validita = Date.Now.Date
                    Case Enum_ModEserciziModificaMultiplaPianoColturale.TuttiGliEsercizi
                        modEsercizi = Enum_ModEserciziModificaMultiplaPianoColturale.TuttiGliEsercizi
                        Data_Validita = AGRODATAINIZIO
                    Case Enum_ModEserciziModificaMultiplaPianoColturale.EserciziAttiviAllaDataX
                        If data_esercizi IsNot Nothing Then
                            modEsercizi = Enum_ModEserciziModificaMultiplaPianoColturale.EserciziAttiviAllaDataX
                            Data_Validita = CDate(data_esercizi.ToString)
                        Else
                            Throw New GiasException("Data filtro esercizi non valida")
                        End If
                    Case Else
                        Throw New GiasException("Modalità filtro esercizi non valida")
                End Select

                almenoUnParametroEsercizio = True
                'Esco dal loop perchè mi basta sapere che stiamo modificando almeno un parametro esercizio
                Exit For
            End If
        Next

        Dim objImpreseProgetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim DT = objImpreseProgetti.Leggi_per_ModificaMultipla(listaChiaviEsercizi, modEsercizi, Data_Validita, almenoUnParametroEsercizio, objParametriServer)

        If DT.Rows.Count > 0 Then
            'Ciclo tutte le chiavi estratte dal DT e popolo i dati anagrafici e capire quali devono subire le modifiche e quali no (in caso di modifica esercizi attivi alla data odierna/x)
            For Each esercizio In DT.Rows
                Dim Piva As String = esercizio.Item("Piva")
                Dim Sa_Cod As Integer = esercizio.Item("Sa_Cod")
                Dim Appezza As Integer = esercizio.Item("Appezza")
                Dim Id_Reg As Integer = esercizio.Item("Id_Reg")
                Dim Progetto_Cod As Integer = esercizio.Item("Progetto_Cod")

                Dim keyAppezzamento As New keyAppezzamento With {.Piva = Piva, .Sa_Cod = Sa_Cod, .Appezza = Appezza}
                Dim keyImpianto As New keyImpianto With {.Piva = Piva, .Sa_Cod = Sa_Cod, .Appezza = Appezza, .Id_Reg = Id_Reg}
                Dim keyEsercizio As New keyEsercizio With {.Piva = Piva, .Sa_Cod = Sa_Cod, .Appezza = Appezza, .Id_Reg = Id_Reg, .Progetto_Cod = Progetto_Cod}

                'Se l'esercizio è statato selezionato dal client lo popolo
                Dim Rag_Soc = esercizio.item("Rag_Soc")
                Dim Validita_Inizio_Azienda As Date = CDate(esercizio.item("Validita_Inizio_Azienda"))
                Dim Validita_Fine_Azienda As Date = CDate(esercizio.item("Validita_Fine_Azienda"))

                Dim Sa_Nome = esercizio.item("Sa_Nome")
                Dim Validita_Inizio_Centro As Date = CDate(esercizio.item("Validita_Inizio_Centro"))
                Dim Validita_Fine_Centro As Date = CDate(esercizio.item("Validita_Fine_Centro"))

                Dim hasCampo As Integer = esercizio.item("hasCampo")
                Dim Campo_Des As String = ""
                Dim Validita_Inizio_Campo As Date = AGRODATAINIZIO
                Dim Validita_Fine_Campo As Date = AGRODATAINIZIO
                If hasCampo = 1 Then
                    Campo_Des = esercizio.item("Campo_Des")
                    Validita_Inizio_Campo = esercizio.item("Validita_Inizio_Campo")
                    Validita_Fine_Campo = esercizio.item("Validita_Fine_Campo")
                End If

                Dim App_Nome = esercizio.item("app_nome")
                Dim Validita_Inizio_Appezzamento As Date = CDate(esercizio.item("Validita_Inizio_Appezzamento"))
                Dim Validita_Fine_Appezzamento As Date = CDate(esercizio.item("Validita_Fine_Appezzamento"))
                Dim MetodoProduzione As Integer = CInt(esercizio.item("MetodoProduzione"))
                Dim SoloEserciziBio As Integer = esercizio.item("SoloEserciziBio")

                Dim Validita_Inizio_Impianto As Date = CDate(esercizio.item("Validita_Inizio_Impianto"))
                Dim Validita_Fine_Impianto As Date = CDate(esercizio.item("Validita_Fine_Impianto"))

                Dim Validita_Inizio_Esercizio As Date = CDate(esercizio.item("Validita_Inizio_Esercizio"))
                Dim Validita_Fine_Esercizio As Date = CDate(esercizio.item("Validita_Fine_Esercizio"))
                Dim Regolamento As Integer = CInt(esercizio.item("Regolamento"))

                Dim Utilizzo As String = ""
                If esercizio.item("Cul_Cod") <> 0 Then
                    Utilizzo = $"{esercizio.item("Veg_Des")} - {esercizio.item("Cul_Des")}"
                Else
                    Utilizzo = $"{esercizio.item("destinazioneUso")}"
                End If

                Dim Lotto As String = esercizio.item("Lotto")

                Dim EsercizioChiuso As Boolean = esercizio.item("EsercizioChiuso")
                Dim ModificaEsercizioPostFiltroValidita As Boolean = esercizio.item("ModificaEsercizioPostFiltroValidita")

                Dim objDatiAnagrafica As New DatiAnagrafica With {
                    .Rag_Soc = Rag_Soc.trim(),
                    .Validita_Inizio_Azienda = Validita_Inizio_Azienda,
                    .Validita_Fine_Azienda = Validita_Fine_Azienda,
                    .Sa_Nome = Sa_Nome.trim(),
                    .Validita_Inizio_Centro = CDate(Validita_Inizio_Centro),
                    .Validita_Fine_Centro = CDate(Validita_Fine_Centro),
                    .App_Nome = If(App_Nome.contains("App"), App_Nome.trim(), $"{Gias.AppezzamentoAbbr} {App_Nome.trim()}"),
                    .Validita_Inizio_Appezzamento = CDate(Validita_Inizio_Appezzamento),
                    .Validita_Fine_Appezzamento = CDate(Validita_Fine_Appezzamento),
                    .MetodoProduzione = MetodoProduzione,
                    .SoloEserciziBio = SoloEserciziBio,
                    .hasCampo = hasCampo,
                    .Campo_Des = Campo_Des.Trim(),
                    .Validita_Inizio_Campo = CDate(Validita_Inizio_Campo),
                    .Validita_Fine_Campo = CDate(Validita_Fine_Campo),
                    .Utilizzo = Utilizzo.Trim(),
                    .Id_Reg = Id_Reg,
                    .Validita_Inizio_Impianto = CDate(Validita_Inizio_Impianto),
                    .Validita_Fine_Impianto = CDate(Validita_Fine_Impianto),
                    .Lotto = Lotto.Trim(),
                    .Progetto_Cod = Progetto_Cod,
                    .Validita_Inizio_Esercizio = CDate(Validita_Inizio_Esercizio),
                    .Validita_Fine_Esercizio = CDate(Validita_Fine_Esercizio),
                    .Regolamento = Regolamento,
                    .EsercizioChiuso = EsercizioChiuso,
                    .ModificaEsercizioPostFiltroValidita = ModificaEsercizioPostFiltroValidita
                }

                If (dicEsercizi.ContainsKey(keyEsercizio)) Then
                    'Se l'esercizio è già presente (perchè passato dal client) lo aggiorno
                    If dicEsercizi(keyEsercizio) Is Nothing Then
                        dicEsercizi(keyEsercizio) = objDatiAnagrafica
                    End If
                Else
                    'Altrimenti sisgnifca che è un esercizio comparso dal filtro data, quindi lo aggiungo
                    dicEsercizi.Add(keyEsercizio, objDatiAnagrafica)
                End If

                If dicImpianti.ContainsKey(keyImpianto) Then
                    If dicImpianti(keyImpianto) Is Nothing Then
                        'Il DT è ordinato per validita_fine_esercizio DESC, quindi nell'obj anagrafica ci sarà sempre l'ultimo esercizio valido per impianto
                        dicImpianti(keyImpianto) = objDatiAnagrafica
                    End If
                End If

                If dicAppezzamenti.ContainsKey(keyAppezzamento) Then
                    If dicAppezzamenti(keyAppezzamento) Is Nothing Then
                        'Il DT è ordinato per validita_fine_esercizio DESC, quindi nell'obj anagrafica ci sarà sempre l'ultimo esercizio valido per appezzamento
                        dicAppezzamenti(keyAppezzamento) = objDatiAnagrafica
                    End If
                End If
            Next

            'Rimuovo gli esercizi che non possono essere modificati post filtro data
            Dim listaEserciziPostFiltroData = dicEsercizi.Where(Function(x) x.Value IsNot Nothing AndAlso x.Value.ModificaEsercizioPostFiltroValidita = False).Select(Function(x) x.Key).ToList()
            For Each elem In listaEserciziPostFiltroData
                If dicEsercizi(elem) IsNot Nothing Then
                    dicEsercizi.Remove(elem)
                End If
            Next

            'Se esistono altri esercizi in lista li controllo per EsercizioChiuso
            If dicEsercizi.Count > 0 Then
                Dim listaEserciziChiusiNonModificabili = dicEsercizi.Where(Function(x) x.Value IsNot Nothing AndAlso x.Value.EsercizioChiuso = True).Select(Function(x) x.Key).ToList()
                Dim multiAzienda As Boolean = dicEsercizi.AsEnumerable().Select(Function(x) (x.Key.Piva)).Distinct().ToList().Count > 1
                For Each elem In listaEserciziChiusiNonModificabili
                    If dicEsercizi(elem) IsNot Nothing Then
                        Dim Esercizio As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Esercizi, dicEsercizi(elem), multiAzienda)
                        eserciziNonModificabiliChiusi.Add(Esercizio)
                        dicEsercizi.Remove(elem)
                    End If
                Next
            Else
                'Se non ci sono elementi coerenti, do errore
                msgEserciziNonModificabili = Gias.NessunEsercizioTrovatoDataInseritaAssociatoImpiantiSelezionati
            End If
        End If

        If eserciziNonModificabiliChiusi.Count > 0 Then
            msgEserciziNonModificabili += $"{Gias.ImpossibileModificareEserciziChiusiDataInserita}: <br>- {String.Join($"<br>- ", eserciziNonModificabiliChiusi)} <br>"
        End If

        Return msgEserciziNonModificabili
    End Function
    Private Function verificaMovimentieCdG_NEW(listChiaviImpianto As List(Of (String, Integer, Integer, Integer)),
                                               listChiaviEsercizio As List(Of (String, Integer, Integer, Integer, Integer)),
                                               profonditaJoin As Enum_EntitaModificaMultiplaPianoColturale,
                                               Validita_Inizio As Date,
                                               Validita_Fine As Date,
                                               ByRef objParametri_Server As AgronicaCoreParametri
                                               ) As List(Of (String, Integer, Integer, Integer, Integer))

        Dim listaBloccati As New List(Of (String, Integer, Integer, Integer, Integer))

        Try
            Dim objControlloAgenda As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
            listaBloccati.AddRange(objControlloAgenda.controllo_MovimentiRicettePua_Massivo(listChiaviImpianto, profonditaJoin, Validita_Inizio, Validita_Fine, objParametri_Server))

            'controllo COSTI DI GESTIONE su qualsiasi Esercizio collegato
            Dim objControlloCdG As New AgronicaCoreAnagrafeBIZ.Progetto_W
            listaBloccati.AddRange(objControlloCdG.controllo_CdG_Massivo(listChiaviEsercizio, profonditaJoin, Validita_Inizio, Validita_Fine, objParametri_Server))

            If listaBloccati.Count > 0 Then
                listaBloccati = listaBloccati.Distinct().ToList()
            End If

        Catch ex As GiasException
            'Errore gestito
            Throw ex
        End Try

        Return listaBloccati

    End Function
    Private Sub verifica_ValiditaFineAppezzamenti_NEW(listApp As Dictionary(Of keyAppezzamento, DatiAnagrafica),
                                                      validita_fine As Date,
                                                      ByRef incoerentiAzienda As List(Of keyAppezzamento),
                                                      ByRef incoerentiCentro As List(Of keyAppezzamento),
                                                      ByRef incoerentiCampo As List(Of keyAppezzamento),
                                                      ByRef incoerentiAppezzamento As List(Of keyAppezzamento),
                                                      ByRef incoerentiImpianto As List(Of keyAppezzamento),
                                                      ByRef incoerentiEsercizio As List(Of keyAppezzamento),
                                                      ByRef aggiornaDataImpianto As Dictionary(Of keyAppezzamento, DatiAnagrafica),
                                                      ByRef aggiornaDataEsercizio As Dictionary(Of keyAppezzamento, DatiAnagrafica),
                                                      ByRef objParametri_Server As AgronicaCoreParametri)

        Dim nomeRoutine = "verifica_ValiditaFineAppezzamento_NEW()"

        Try
            For Each elem In listApp
                Dim app = elem.Value
                Dim key = elem.Key

                '1) Verifico che la data fine sia coerente con la data inizio dello stesso
                If validita_fine <= app.Validita_Inizio_Appezzamento Then
                    incoerentiAppezzamento.Add(key)
                    Continue For
                End If


                '2) Verifico che la nuova data fine sia coerente con le entità al di sopra dell'appezzamento
                ' - La validità fine non può essere strettamente maggiore alla validita fine di comparazione (se sono uguali è ok)
                ' - La validità fine non può essere minore o uguale alla validita inizio di comparazione 
                If app.hasCampo Then
                    If validita_fine > app.Validita_Fine_Campo OrElse
                    validita_fine <= app.Validita_Inizio_Campo Then
                        incoerentiCampo.Add(key)
                        Continue For
                    End If
                End If
                If validita_fine > app.Validita_Fine_Centro OrElse
                    validita_fine <= app.Validita_Inizio_Centro Then
                    incoerentiCentro.Add(key)
                    Continue For
                End If
                If validita_fine > app.Validita_Fine_Azienda OrElse
                    validita_fine <= app.Validita_Inizio_Azienda Then
                    incoerentiAzienda.Add(key)
                    Continue For
                End If

                '3) Se la nuova validita_fine è restringente rispetto a quella attuale, verifico le entità sottostanti
                If validita_fine < app.Validita_Fine_Appezzamento Then

                    ' - La validità fine non può essere minore o uguale alla validita inizio di comparazione 
                    If validita_fine <= app.Validita_Inizio_Impianto Then
                        incoerentiImpianto.Add(key)
                        Continue For
                    Else

                        If validita_fine < app.Validita_Fine_Impianto Then
                            'La data è restringente ma coerente con l'entità sottostante, se ci fossero state operazioni registrate sarebbe già stato escluso a priori
                            aggiornaDataImpianto.Add(key, app)
                        End If

                        ' - La validità fine non può essere minore o uguale alla validita inizio di comparazione 
                        If validita_fine <= app.Validita_Inizio_Esercizio Then
                            incoerentiEsercizio.Add(key)
                            Continue For
                        Else
                            If validita_fine < app.Validita_Fine_Esercizio Then
                                'La data è restringente ma coerente con l'esercizio, se ci fosserocosti registrati sarebbe già stato escluso a priori
                                aggiornaDataEsercizio.Add(key, app)
                            End If
                        End If
                    End If
                End If
            Next

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

    End Sub
    Private Sub verifica_ValiditaFineImpianti_NEW(dicImpianti As Dictionary(Of keyImpianto, DatiAnagrafica),
                                                  validita_fine As Date,
                                                  ByRef incoerentiAzienda As List(Of keyImpianto),
                                                  ByRef incoerentiCentro As List(Of keyImpianto),
                                                  ByRef incoerentiCampo As List(Of keyImpianto),
                                                  ByRef incoerentiAppezzamento As List(Of keyImpianto),
                                                  ByRef incoerentiImpianto As List(Of keyImpianto),
                                                  ByRef incoerentiEsercizio As List(Of keyImpianto),
                                                  ByRef aggiornaDataEsercizio As Dictionary(Of keyImpianto, DatiAnagrafica),
                                                  ByRef objParametri_Server As AgronicaCoreParametri)

        Dim nomeRoutine = "verifica_ValiditaFineImpianti_NEW()"

        Try
            For Each elem In dicImpianti
                Dim imp = elem.Value
                Dim key = elem.Key

                '1) Verifico che la data fine sia coerente con la data inizio dello stesso
                If validita_fine <= imp.Validita_Inizio_Impianto Then
                    incoerentiImpianto.Add(key)
                    Continue For
                End If


                '2) Verifico che la nuova data fine sia coerente con le entità al di sopra dell'impianto
                ' - La validità fine non può essere strettamente maggiore alla validita fine di comparazione (se sono uguali è ok)
                ' - La validità fine non può essere minore o uguale alla validita inizio di comparazione 
                If validita_fine > imp.Validita_Fine_Appezzamento OrElse
                    validita_fine <= imp.Validita_Inizio_Appezzamento Then
                    incoerentiAppezzamento.Add(key)
                    Continue For
                End If
                If imp.hasCampo Then
                    If validita_fine > imp.Validita_Fine_Campo OrElse
                    validita_fine <= imp.Validita_Inizio_Campo Then
                        incoerentiCampo.Add(key)
                        Continue For
                    End If
                End If
                If validita_fine > imp.Validita_Fine_Centro OrElse
                    validita_fine <= imp.Validita_Inizio_Centro Then
                    incoerentiCentro.Add(key)
                    Continue For
                End If
                If validita_fine > imp.Validita_Fine_Azienda OrElse
                    validita_fine <= imp.Validita_Inizio_Azienda Then
                    incoerentiAzienda.Add(key)
                    Continue For
                End If


                '3) Se la nuova validita_fine è restringente rispetto a
                'quella attuale, verifico le entità sottostanti
                If validita_fine < imp.Validita_Fine_Impianto Then

                    ' - La validità fine non può essere minore o uguale alla validita inizio di comparazione 
                    If validita_fine <= imp.Validita_Inizio_Esercizio Then
                        incoerentiEsercizio.Add(key)
                        Continue For
                    Else
                        If validita_fine < imp.Validita_Fine_Esercizio Then
                            'La data è restringente ma coerente con l'impianto, se ci fossero state operaizoni registrate sarebbe già stato escluso a priori
                            aggiornaDataEsercizio.Add(key, imp)
                        End If
                    End If
                End If
            Next

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

    End Sub
    Private Sub verifica_ValiditaFineEsercizi_NEW(dicEsercizi As Dictionary(Of keyEsercizio, DatiAnagrafica),
                                                  validita_fine As Date,
                                                  ByRef incoerentiAzienda As List(Of keyImpianto),
                                                  ByRef incoerentiCentro As List(Of keyImpianto),
                                                  ByRef incoerentiCampo As List(Of keyImpianto),
                                                  ByRef incoerentiAppezzamento As List(Of keyImpianto),
                                                  ByRef incoerentiImpianto As List(Of keyImpianto),
                                                  ByRef incoerentiEsercizio As List(Of keyImpianto),
                                                  ByRef objParametri_Server As AgronicaCoreParametri)

        Dim nomeRoutine = "verifica_ValiditaFineImpianti_NEW()"

        Try
            For Each elem In dicEsercizi
                Dim ese = elem.Value
                Dim key = elem.Key

                '1) Verifico che la data fine sia coerente con la data inizio dello stesso
                If validita_fine <= ese.Validita_Inizio_Esercizio Then
                    incoerentiEsercizio.Add(key)
                    Continue For
                End If

                '2) Verifico che la nuova data fine sia coerente con le entità al di sopra dell'impianto
                ' - La validità fine non può essere strettamente maggiore alla validita fine di comparazione (se sono uguali è ok)
                ' - La validità fine non può essere minore o uguale alla validita inizio di comparazione 
                If validita_fine > ese.Validita_Fine_Impianto OrElse
                    validita_fine <= ese.Validita_Inizio_Impianto Then
                    incoerentiImpianto.Add(key)
                    Continue For
                End If
                If validita_fine > ese.Validita_Fine_Appezzamento OrElse
                    validita_fine <= ese.Validita_Inizio_Appezzamento Then
                    incoerentiAppezzamento.Add(key)
                    Continue For
                End If
                If ese.hasCampo Then
                    If validita_fine > ese.Validita_Fine_Campo OrElse
                    validita_fine <= ese.Validita_Inizio_Campo Then
                        incoerentiCampo.Add(key)
                        Continue For
                    End If
                End If
                If validita_fine > ese.Validita_Fine_Centro OrElse
                    validita_fine <= ese.Validita_Inizio_Centro Then
                    incoerentiCentro.Add(key)
                    Continue For
                End If
                If validita_fine > ese.Validita_Fine_Azienda OrElse
                    validita_fine <= ese.Validita_Inizio_Azienda Then
                    incoerentiAzienda.Add(key)
                    Continue For
                End If
            Next

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

    End Sub
    Private Sub verifica_BioAppezzamenti(ByRef incoerentiAppezzamento As List(Of keyAppezzamento),
                                         ByRef objParametri_Server As AgronicaCoreParametri)

        Dim nomeRoutine = "verifica_BioAppezzamenti()"

        Try
            'Per ogni appezzamento verifico se esistono esercizi non bio
            For Each elem In dicAppezzamenti
                Dim app = elem.Value
                Dim key = elem.Key

                If app.SoloEserciziBio = False Then
                    incoerentiAppezzamento.Add(key)
                End If
            Next

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

    End Sub
    Private Sub verifica_NrAppBioAppezzamenti(ByRef incoerentiAppezzamento As List(Of keyAppezzamento),
                                              ByRef objParametri_Server As AgronicaCoreParametri)

        Dim nomeRoutine = "verifica_NrAppBioAppezzamenti()"

        Try
            'Per ogni appezzamento verifico se il metodo produzione è Bio
            For Each elem In dicAppezzamenti
                Dim app = elem.Value
                Dim key = elem.Key

                If app.MetodoProduzione <> CInt(enum_MetodoProduzione.Biologico) Then
                    incoerentiAppezzamento.Add(key)
                End If
            Next

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

    End Sub
    Private Sub verifica_BioEsercizi(ByRef incoerentiEsercizi As List(Of keyEsercizio),
                                     ByRef objParametri_Server As AgronicaCoreParametri)

        Dim nomeRoutine = "verifica_BioEsercizi()"

        Try
            'Se vogliamo impostare un Regolamento Esercizio <> BIO devo verificare il metodo di produzione dell'appezzamento
            'Su app bio, l'unico regolamento possibile è il BIO
            For Each elem In dicEsercizi
                Dim ese = elem.Value
                Dim key = elem.Key

                If ese.MetodoProduzione = CInt(enum_MetodoProduzione.Biologico) Then
                    incoerentiEsercizi.Add(key)
                End If
            Next

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

    End Sub

    Private Function ModificaAppezzamenti_NEW(ByVal parametro As Integer,
                                              ByRef valore_parametri_modificati As JObject,
                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                              NoteLog As String) As String

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Ereditatore.ModificaAppezzamenti_NEW()"
        Dim messaggioErrore As String = ""

        Dim campoModificato As String = ""

        Dim objLogAnagrafe As New AgronicaLogAnagrafe_W
        Dim objAppezzamento As New Appezzamento_Write
        Dim objAppezzamentoCodici As New Appezzamento_Codici_W
        Dim objImpianto As New Reg_Impianti_Write
        Dim objEsercizio As New Impresa_Progetti_W
        Dim objAppezzamentixParticelle As New AppezzamentixParticelle_W

        Dim appNonModificabili As New Dictionary(Of keyAppezzamento, DatiAnagrafica)

        Dim erroreMovimenti As New List(Of String)
        Dim erroreValidita As New List(Of String)
        Dim erroreBIO As New List(Of String)
        Dim erroreNrAppBIO As New List(Of String)
        Dim errore As String = ""

        Try
            Dim timeStamp = Date.Now()

            'Mi serve come copia dell'oggetto orignale. Da questi verranno rimossi gli appezzamenti non modificabili
            Dim dicDistinctAppezzamentiPostControlli As New Dictionary(Of keyAppezzamento, DatiAnagrafica)(dicAppezzamenti)

            'Da passare negli update
            Dim listaChiaviAppezzamenti = dicAppezzamenti.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza))).ToList()
            'Da passare nei controlli
            Dim listaChiaviImpianti = dicImpianti.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza), CInt(item.Key.Id_Reg))).ToList()
            Dim listaChiaviEsercizi = dicEsercizi.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza), CInt(item.Key.Id_Reg), CInt(item.Key.Progetto_Cod))).ToList()

            Dim multiAzienda As Boolean = dicEsercizi.AsEnumerable().Select(Function(x) (x.Key.Piva)).Distinct().ToList().Count > 1

            Select Case parametro
                Case Enum_ParametriModificaMultiplaPianoColturale.APP_DataFineAppezzamento
                    If valore_parametri_modificati("Data_Fine_Appezzamento") IsNot Nothing Then

                        'Passo le chiavi degli impianti e degli esercizi per controllare i Movimenti e i CdG
                        Dim listaBloccatiMovimenti = verificaMovimentieCdG_NEW(listaChiaviImpianti, listaChiaviEsercizi, Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti, AGRODATAINIZIO, CDate(valore_parametri_modificati("Data_Fine_Appezzamento").ToString), objParametri_Server)

                        If listaBloccatiMovimenti.Count > 0 Then
                            For Each elem In listaBloccatiMovimenti
                                Dim keyAppezzamento As New keyAppezzamento With {.Piva = elem.Item1, .Sa_Cod = elem.Item2, .Appezza = elem.Item3}
                                appNonModificabili.Add(keyAppezzamento, dicAppezzamenti(keyAppezzamento))
                            Next

                            For Each nonModificabile In appNonModificabili
                                If dicDistinctAppezzamentiPostControlli.ContainsKey(nonModificabile.Key) Then
                                    dicDistinctAppezzamentiPostControlli.Remove(nonModificabile.Key)

                                    Dim App_Nome As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti, nonModificabile.Value, multiAzienda)
                                    If String.IsNullOrEmpty(App_Nome) Then
                                        App_Nome = nonModificabile.Key.Piva + "-" + CStr(nonModificabile.Key.Sa_Cod) + "-" + CStr(nonModificabile.Key.Appezza)
                                    End If
                                    erroreMovimenti.Add(App_Nome)
                                End If
                            Next
                        End If

                        If dicDistinctAppezzamentiPostControlli.Count > 0 Then
                            appNonModificabili.Clear()

                            Dim incoerentiAzienda As New List(Of keyAppezzamento)
                            Dim incoerentiCentro As New List(Of keyAppezzamento)
                            Dim incoerentiCampo As New List(Of keyAppezzamento)
                            Dim incoerentiAppezzamento As New List(Of keyAppezzamento)
                            Dim incoerentiImpianto As New List(Of keyAppezzamento)
                            Dim incoerentiEsercizio As New List(Of keyAppezzamento)

                            Dim aggiornaDataImpianto As New Dictionary(Of keyAppezzamento, DatiAnagrafica)
                            Dim aggiornaDataEsercizio As New Dictionary(Of keyAppezzamento, DatiAnagrafica)

                            verifica_ValiditaFineAppezzamenti_NEW(dicDistinctAppezzamentiPostControlli, CDate(valore_parametri_modificati("Data_Fine_Appezzamento").ToString), incoerentiAzienda, incoerentiCentro, incoerentiCampo, incoerentiAppezzamento, incoerentiImpianto, incoerentiEsercizio, aggiornaDataImpianto, aggiornaDataEsercizio, objParametri_Server)

                            If incoerentiAppezzamento.Count > 0 Then
                                For Each elem In incoerentiAppezzamento
                                    appNonModificabili.Add(elem, dicAppezzamenti(elem))
                                Next
                                For Each nonModificabile In appNonModificabili
                                    If dicDistinctAppezzamentiPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicDistinctAppezzamentiPostControlli.Remove(nonModificabile.Key)

                                        Dim App_Nome As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti, nonModificabile.Value, multiAzienda) &
                                            $" ({Gias.ValiditaInizioAppAbbr}: {nonModificabile.Value.Validita_Inizio_Appezzamento.ToShortDateString()})"
                                        erroreValidita.Add(App_Nome)
                                    End If
                                Next
                            End If

                            If incoerentiCampo.Count > 0 Then
                                For Each elem In incoerentiCampo
                                    appNonModificabili.Add(elem, dicAppezzamenti(elem))
                                Next
                                For Each nonModificabile In appNonModificabili
                                    If dicDistinctAppezzamentiPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicDistinctAppezzamentiPostControlli.Remove(nonModificabile.Key)

                                        Dim App_Nome As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti, nonModificabile.Value, multiAzienda) &
                                            $" ({Gias.ValiditaCampo} {nonModificabile.Value.Campo_Des}: {nonModificabile.Value.Validita_Inizio_Campo.ToShortDateString()}-{nonModificabile.Value.Validita_Fine_Campo.ToShortDateString()})"
                                        erroreValidita.Add(App_Nome)
                                    End If
                                Next
                            End If

                            If incoerentiCentro.Count > 0 Then
                                For Each elem In incoerentiCentro
                                    appNonModificabili.Add(elem, dicAppezzamenti(elem))
                                Next
                                For Each nonModificabile In appNonModificabili
                                    If dicDistinctAppezzamentiPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicDistinctAppezzamentiPostControlli.Remove(nonModificabile.Key)
                                        Dim App_Nome As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti, nonModificabile.Value, multiAzienda) &
                                            $" ({Gias.ValiditaCentro}: {nonModificabile.Value.Validita_Inizio_Centro.ToShortDateString()}-{nonModificabile.Value.Validita_Fine_Centro.ToShortDateString()})"
                                        erroreValidita.Add(App_Nome)
                                    End If
                                Next
                            End If

                            If incoerentiAzienda.Count > 0 Then
                                For Each elem In incoerentiCentro
                                    appNonModificabili.Add(elem, dicAppezzamenti(elem))
                                Next
                                For Each nonModificabile In appNonModificabili
                                    If dicDistinctAppezzamentiPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicDistinctAppezzamentiPostControlli.Remove(nonModificabile.Key)
                                        Dim App_Nome As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti, nonModificabile.Value, multiAzienda) &
                                            $" ({Gias.ValiditaAzienda}: {nonModificabile.Value.Validita_Inizio_Azienda.ToShortDateString()}-{nonModificabile.Value.Validita_Fine_Azienda.ToShortDateString()})"
                                        erroreValidita.Add(App_Nome)
                                    End If
                                Next
                            End If

                            If incoerentiImpianto.Count > 0 Then
                                For Each elem In incoerentiImpianto
                                    appNonModificabili.Add(elem, dicAppezzamenti(elem))
                                Next
                                For Each nonModificabile In appNonModificabili
                                    If dicDistinctAppezzamentiPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicDistinctAppezzamentiPostControlli.Remove(nonModificabile.Key)

                                        Dim App_Nome As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti, nonModificabile.Value, multiAzienda) &
                                             $" ({Gias.ValiditaImpianto} [{nonModificabile.Value.Utilizzo}]: {nonModificabile.Value.Validita_Inizio_Impianto.ToShortDateString()}-{nonModificabile.Value.Validita_Fine_Impianto.ToShortDateString()})"
                                        erroreValidita.Add(App_Nome)
                                    End If
                                Next
                            End If

                            If incoerentiEsercizio.Count > 0 Then
                                For Each elem In incoerentiEsercizio
                                    appNonModificabili.Add(elem, dicAppezzamenti(elem))
                                Next
                                For Each nonModificabile In appNonModificabili
                                    If dicDistinctAppezzamentiPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicDistinctAppezzamentiPostControlli.Remove(nonModificabile.Key)

                                        Dim App_Nome As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti, nonModificabile.Value, multiAzienda) &
                                            $" ({Gias.ValiditaEsercizio} [{nonModificabile.Value.Utilizzo}{If(nonModificabile.Value.Lotto <> "", $", {Gias.Lotto}: {nonModificabile.Value.Lotto}", "")}]: {nonModificabile.Value.Validita_Inizio_Esercizio.ToShortDateString()}-{nonModificabile.Value.Validita_Fine_Esercizio.ToShortDateString()}) "
                                        erroreValidita.Add(App_Nome)
                                    End If
                                Next
                            End If

                            If dicDistinctAppezzamentiPostControlli.Count > 0 Then
                                campoModificato = $"Validita_Fine_Appezzamento newValue = [{CDate(valore_parametri_modificati("Data_Fine_Appezzamento").ToString)}]"

                                Dim listChiaviAppezzamenti_POSTCLEANUP = dicDistinctAppezzamentiPostControlli.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza))).ToList()
                                objAppezzamento.UpdateColonna_Massivo(listChiaviAppezzamenti_POSTCLEANUP, "Validita_Fine", CDate(valore_parametri_modificati("Data_Fine_Appezzamento").ToString), "date", timeStamp, objParametri_Server)
                                objAppezzamentixParticelle.UpdateColonna_Massivo_DaChiaviAppezzamento(listChiaviAppezzamenti_POSTCLEANUP, "Validita_Fine", CDate(valore_parametri_modificati("Data_Fine_Appezzamento").ToString), "date", timeStamp, objParametri_Server)

                                If aggiornaDataImpianto.Count > 0 Then
                                    Dim listChiaviImpianti_POSTCLEANUP = aggiornaDataImpianto.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza), CInt(item.Value.Id_Reg))).ToList()
                                    objImpianto.UpdateColonna_Massivo(listChiaviImpianti_POSTCLEANUP, "Validita_Fine", CDate(valore_parametri_modificati("Data_Fine_Appezzamento").ToString), "date", timeStamp, objParametri_Server)

                                    'Scrittura tabella Agronica_Log_Anagrafe
                                    objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                                                                   listChiaviImpianti_POSTCLEANUP,
                                                                                   Nothing,
                                                                                   Enum_EntitaModificaMultiplaPianoColturale.Impianti,
                                                                                   timeStamp,
                                                                                   $"{NoteLog} ({campoModificato})",
                                                                                   objParametri_Server)

                                    If aggiornaDataEsercizio.Count > 0 Then
                                        Dim listChiaviEsercizi_POSTCLEANUP = aggiornaDataEsercizio.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza), CInt(item.Value.Id_Reg), CInt(item.Value.Progetto_Cod))).ToList()
                                        objEsercizio.UpdateColonna_Massivo(listChiaviEsercizi_POSTCLEANUP, "Validita_Fine", CDate(valore_parametri_modificati("Data_Fine_Appezzamento").ToString), "date", timeStamp, objParametri_Server)

                                        'Scrittura tabella Agronica_Log_Anagrafe
                                        objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                                                                       Nothing,
                                                                                       listChiaviEsercizi_POSTCLEANUP,
                                                                                       Enum_EntitaModificaMultiplaPianoColturale.Esercizi,
                                                                                       timeStamp,
                                                                                       $"{NoteLog} ({campoModificato})",
                                                                                       objParametri_Server)
                                    End If
                                End If
                            End If
                        End If
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.APP_MetodoProduzione
                    If valore_parametri_modificati("MetodoProduzione_Cod") IsNot Nothing Then
                        Dim val_cod As String = valore_parametri_modificati("MetodoProduzione_Cod").ToString()

                        If val_cod <> "" AndAlso val_cod = CInt(enum_MetodoProduzione.Biologico) Then
                            Dim incoerentiAppezzamento As New List(Of keyAppezzamento)
                            verifica_BioAppezzamenti(incoerentiAppezzamento, objParametri_Server)

                            If incoerentiAppezzamento.Count > 0 Then
                                For Each elem In incoerentiAppezzamento
                                    appNonModificabili.Add(elem, dicAppezzamenti(elem))
                                Next
                                For Each nonModificabile In appNonModificabili
                                    If dicDistinctAppezzamentiPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicDistinctAppezzamentiPostControlli.Remove(nonModificabile.Key)

                                        Dim App_Nome As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti, nonModificabile.Value, multiAzienda)
                                        erroreBIO.Add(App_Nome)
                                    End If
                                Next
                            End If
                        End If

                        If dicDistinctAppezzamentiPostControlli.Count > 0 Then
                            Dim listChiaviAppezzamenti_POSTCLEANUP = dicDistinctAppezzamentiPostControlli.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza))).ToList()
                            objAppezzamentoCodici.InsertUpdateDeleteCodici_Massivo(listChiaviAppezzamenti_POSTCLEANUP,
                                                                                   If(val_cod = "", enum_TipoOperazioneDB.Cancellazione, enum_TipoOperazioneDB.Modifica),
                                                                                   enum_CodiciAnagrafe.MetodoDiProduzione,
                                                                                   val_cod,
                                                                                   timeStamp,
                                                                                   objParametri_Server)
                            campoModificato = $"MetodoProduzione_Cod newValue = [id_cod:{CInt(enum_CodiciAnagrafe.MetodoDiProduzione)}, val_cod:{val_cod}]"
                        End If
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.APP_NrAppBio
                    If valore_parametri_modificati("nrAppBio") IsNot Nothing Then
                        Dim val_cod As String = valore_parametri_modificati("nrAppBio").ToString().Trim()

                        If val_cod <> "" Then
                            Dim incoerentiAppezzamento As New List(Of keyAppezzamento)
                            verifica_NrAppBioAppezzamenti(incoerentiAppezzamento, objParametri_Server)

                            If incoerentiAppezzamento.Count > 0 Then
                                For Each elem In incoerentiAppezzamento
                                    appNonModificabili.Add(elem, dicAppezzamenti(elem))
                                Next
                                For Each nonModificabile In appNonModificabili
                                    If dicDistinctAppezzamentiPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicDistinctAppezzamentiPostControlli.Remove(nonModificabile.Key)

                                        Dim App_Nome As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti, nonModificabile.Value, multiAzienda)
                                        erroreNrAppBIO.Add(App_Nome)
                                    End If
                                Next
                            End If
                        End If

                        If dicDistinctAppezzamentiPostControlli.Count > 0 Then
                            Dim listChiaviAppezzamenti_POSTCLEANUP = dicDistinctAppezzamentiPostControlli.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza))).ToList()

                            objAppezzamentoCodici.InsertUpdateDeleteCodici_Massivo(listaChiaviAppezzamenti,
                                                                               If(val_cod = "", enum_TipoOperazioneDB.Cancellazione, enum_TipoOperazioneDB.Modifica),
                                                                               enum_CodiciAnagrafe.Codice_Appezza_Biologico,
                                                                               val_cod,
                                                                               timeStamp,
                                                                               objParametri_Server)
                            campoModificato = $"nrAppBio newValue = [id_cod:{CInt(enum_CodiciAnagrafe.Codice_Appezza_Biologico)}, val_cod:{If(val_cod = "", "NULL", val_cod)}]"
                        End If
                    End If
            End Select

            If campoModificato <> "" Then
                'Scrittura tabella Agronica_Log_Anagrafe
                objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(listaChiaviAppezzamenti,
                                                                   Nothing,
                                                                   Nothing,
                                                                   Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti,
                                                                   timeStamp,
                                                                   $"{NoteLog} ({campoModificato})",
                                                                   objParametri_Server)
            End If

            'todo : verifica i metodi:
            'verifica_ValiditaFineAppezzamento(appezzamento, CDate(valore_parametri_modificati("Data_Fine_Appezzamento").ToString), objParametri_Server, GiasContext)
            'verificaMovimentieCdG
            'particelle, impianti, esercizi data fine

            If erroreMovimenti.Count > 0 Then
                errore += $"{Gias.ImpossibileModificareDataChiusuraAppezzamentiCostiMovimentiAssociati}: <br>- {String.Join($"<br>- ", erroreMovimenti)} <br>"
            End If
            If erroreValidita.Count > 0 Then
                If errore <> "" Then
                    errore += "<br>"
                End If
                errore += $"{Gias.ImpossibileModificareDataChiusuraAppezzamentiNonCoerenteEntita}: <br>- {String.Join($"<br>- ", erroreValidita)} <br>"
            End If
            If erroreBIO.Count > 0 Then
                If errore <> "" Then
                    errore += "<br>"
                End If
                errore += $"{Gias.ImpossibileModificareMetodoProduzioneBIOAppezzamentiEserciziNonBIO}: <br>- {String.Join($"<br>- ", erroreBIO)} <br>"
            End If
            If erroreNrAppBIO.Count > 0 Then
                If errore <> "" Then
                    errore += "<br>"
                End If
                errore += $"{Gias.ImpossibileModificareNrAppBioAppezzamentiNonBIO}: <br>- {String.Join($"<br>- ", erroreNrAppBIO)} <br>"
            End If

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            messaggioErrore = "Enum_ParametriModificaMultiplaPianoColturale Modificato: " & parametro.ToString() & vbCrLf &
            "Oggetto: " & JsonConvert.SerializeObject(valore_parametri_modificati) & vbCrLf &
            "Numero elementi in modifica: " & dicAppezzamenti.Count().ToString() & vbCrLf &
            ex.Message

            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return errore
    End Function
    Private Function ModificaImpianti_NEW(parametro As Integer,
                                          ByRef valore_parametri_modificati As JObject,
                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                          NoteLog As String) As String

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Ereditatore.ModificaImpianto_NEW()"
        Dim messaggioErrore As String = ""

        Dim campoModificato As String = ""

        Dim objLogAnagrafe As New AgronicaLogAnagrafe_W
        Dim objImpianto As New Reg_Impianti_Write
        Dim objImpiantoCodici As New Reg_Impianti_Codici_W
        Dim objEsercizio As New Impresa_Progetti_W

        Dim impNonModificabili As New Dictionary(Of keyImpianto, DatiAnagrafica)

        Dim nomeColonnaUpdate As String = String.Empty
        Dim valoreUpdate As String = String.Empty
        Dim dataTypeValoreUpdate As String = "string"

        Dim codiceUpdate As Integer = -1
        Dim valoreCodiceUpdate As String = String.Empty

        Dim erroreMovimenti As New List(Of String)
        Dim erroreValidita As New List(Of String)
        Dim errore As String = ""

        Try
            Dim timeStamp = Date.Now()

            'Mi serve come copia dell'oggetto orignale. Da questi verranno rimossi gli impianti non modificabili
            Dim dicImpiantiPostControlli As New Dictionary(Of keyImpianto, DatiAnagrafica)(dicImpianti)

            'Da passare negli update e nei controlli
            Dim listaChiaviImpianti = dicImpianti.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza), CInt(item.Key.Id_Reg))).ToList()
            Dim listaChiaviEsercizi = dicEsercizi.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza), CInt(item.Key.Id_Reg), CInt(item.Key.Progetto_Cod))).ToList()

            Dim multiAzienda As Boolean = dicImpianti.AsEnumerable().Select(Function(x) (x.Key.Piva)).Distinct().ToList().Count > 1

            Select Case parametro
                Case Enum_ParametriModificaMultiplaPianoColturale.IMP_Finalita
                    If valore_parametri_modificati("grfi_cod") IsNot Nothing Then
                        valoreUpdate = valore_parametri_modificati("grfi_cod").ToString()
                        nomeColonnaUpdate = "GRFI_COD"
                        campoModificato = $"Finalita newValue = [{valoreUpdate}]"
                        dataTypeValoreUpdate = "number"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.IMP_Varieta
                    If valore_parametri_modificati("cul_cod") IsNot Nothing Then
                        nomeColonnaUpdate = "cul_cod"
                        valoreUpdate = valore_parametri_modificati("cul_cod")
                        campoModificato = $"Varieta newValue = [{valoreUpdate}]"
                        dataTypeValoreUpdate = "number"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.IMP_GruppoVarietale
                    If valore_parametri_modificati("grva_cod") IsNot Nothing Then
                        nomeColonnaUpdate = "GRVA_Cod_VEG"
                        valoreUpdate = valore_parametri_modificati("grva_cod")
                        campoModificato = $"Gruppo_Varietale newValue = [{valoreUpdate}]"
                        dataTypeValoreUpdate = "number"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.IMP_Copertura
                    If valore_parametri_modificati("Cop_Cod") IsNot Nothing Then
                        nomeColonnaUpdate = "Cop_Cod"
                        valoreUpdate = valore_parametri_modificati("Cop_Cod")
                        campoModificato = $"Copertura newValue = [{valoreUpdate}]"
                        dataTypeValoreUpdate = "number"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.IMP_ImpIrrigazione
                    If valore_parametri_modificati("ImpIrrigazione") IsNot Nothing Then
                        nomeColonnaUpdate = "Imp_Cod"
                        valoreUpdate = valore_parametri_modificati("ImpIrrigazione")
                        campoModificato = $"ImpIrrigazione newValue = [{valoreUpdate}]"
                        dataTypeValoreUpdate = "number"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.IMP_FormaAllevamento
                    If valore_parametri_modificati("Foral_Cod") IsNot Nothing Then
                        nomeColonnaUpdate = "Foral_Cod"
                        valoreUpdate = valore_parametri_modificati("Foral_Cod")
                        campoModificato = $"FormaAllevamento newValue = [{valoreUpdate}]"
                        dataTypeValoreUpdate = "number"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.IMP_Portinnesto
                    If valore_parametri_modificati("Port_Cod") IsNot Nothing Then
                        nomeColonnaUpdate = "Port_Cod"
                        valoreUpdate = valore_parametri_modificati("Port_Cod")
                        campoModificato = $"Portinnesto newValue = [{valoreUpdate}]"
                        dataTypeValoreUpdate = "number"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.IMP_DataInizioPortinnesto
                    If valore_parametri_modificati("Data_Inizio_Portinnesto") IsNot Nothing Then
                        nomeColonnaUpdate = "Data_Inizio_Portinnesto"
                        valoreUpdate = CDate(valore_parametri_modificati("Data_Inizio_Portinnesto").ToString())
                        campoModificato = $"Data_Inizio_Portinnesto newValue = [{valoreUpdate.ToString()}]"
                        dataTypeValoreUpdate = "date"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.IMP_DataFineImpianto
                    If valore_parametri_modificati("Data_Fine_Impianto") IsNot Nothing Then
                        'Passo le chiavi degli impianti e degli esercizi per controllare i Movimenti e i CdG
                        Dim listaBloccatiMovimenti = verificaMovimentieCdG_NEW(listaChiaviImpianti, listaChiaviEsercizi, Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti, AGRODATAINIZIO, CDate(valore_parametri_modificati("Data_Fine_Impianto").ToString), objParametri_Server)

                        If listaBloccatiMovimenti.Count > 0 Then
                            For Each elem In listaBloccatiMovimenti
                                Dim keyImpianto As New keyImpianto With {.Piva = elem.Item1, .Sa_Cod = elem.Item2, .Appezza = elem.Item3, .Id_Reg = elem.Item4}
                                impNonModificabili.Add(keyImpianto, dicImpianti(keyImpianto))
                            Next

                            For Each nonModificabile In impNonModificabili
                                If dicImpiantiPostControlli.ContainsKey(nonModificabile.Key) Then
                                    dicImpiantiPostControlli.Remove(nonModificabile.Key)

                                    Dim Impianto As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Impianti, nonModificabile.Value, multiAzienda)
                                    If String.IsNullOrEmpty(Impianto) Then
                                        Impianto = nonModificabile.Key.Piva + "-" + CStr(nonModificabile.Key.Sa_Cod) + "-" + CStr(nonModificabile.Key.Appezza) + "-" + CStr(nonModificabile.Key.Id_Reg)
                                    End If
                                    erroreMovimenti.Add(Impianto)
                                End If
                            Next
                        End If

                        If dicImpiantiPostControlli.Count > 0 Then
                            impNonModificabili.Clear()

                            Dim incoerentiAzienda As New List(Of keyImpianto)
                            Dim incoerentiCentro As New List(Of keyImpianto)
                            Dim incoerentiCampo As New List(Of keyImpianto)
                            Dim incoerentiAppezzamento As New List(Of keyImpianto)
                            Dim incoerentiImpianto As New List(Of keyImpianto)
                            Dim incoerentiEsercizio As New List(Of keyImpianto)
                            Dim aggiornaDataEsercizio As New Dictionary(Of keyImpianto, DatiAnagrafica)

                            verifica_ValiditaFineImpianti_NEW(dicImpiantiPostControlli, CDate(valore_parametri_modificati("Data_Fine_Impianto").ToString), incoerentiAzienda, incoerentiCentro, incoerentiCampo, incoerentiAppezzamento, incoerentiImpianto, incoerentiEsercizio, aggiornaDataEsercizio, objParametri_Server)

                            If incoerentiImpianto.Count > 0 Then
                                For Each elem In incoerentiImpianto
                                    impNonModificabili.Add(elem, dicImpianti(elem))
                                Next

                                For Each nonModificabile In impNonModificabili
                                    If dicImpiantiPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicImpiantiPostControlli.Remove(nonModificabile.Key)

                                        Dim Impianto As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Impianti, nonModificabile.Value, multiAzienda) &
                                            $" {Gias.ValiditaInizioImpAbbr}: {nonModificabile.Value.Validita_Inizio_Impianto.ToShortDateString()})"
                                        erroreValidita.Add(Impianto)
                                    End If
                                Next
                            End If

                            If incoerentiAppezzamento.Count > 0 Then
                                For Each elem In incoerentiAppezzamento
                                    impNonModificabili.Add(elem, dicImpianti(elem))
                                Next
                                For Each nonModificabile In impNonModificabili
                                    If dicImpiantiPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicImpiantiPostControlli.Remove(nonModificabile.Key)

                                        Dim Impianto As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Impianti, nonModificabile.Value, multiAzienda) &
                                            $" ({Gias.ValiditaAppAbbr}: {nonModificabile.Value.Validita_Inizio_Appezzamento.ToShortDateString()}-{nonModificabile.Value.Validita_Fine_Appezzamento.ToShortDateString()}) "
                                        erroreValidita.Add(Impianto)
                                    End If
                                Next
                            End If

                            If incoerentiCampo.Count > 0 Then
                                For Each elem In incoerentiAppezzamento
                                    impNonModificabili.Add(elem, dicImpianti(elem))
                                Next
                                For Each nonModificabile In impNonModificabili
                                    If dicImpiantiPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicImpiantiPostControlli.Remove(nonModificabile.Key)

                                        Dim Impianto As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Impianti, nonModificabile.Value, multiAzienda) &
                                            $" ({Gias.ValiditaCampo}: {nonModificabile.Value.Validita_Inizio_Campo.ToShortDateString()}-{nonModificabile.Value.Validita_Fine_Campo.ToShortDateString()}) "
                                        erroreValidita.Add(Impianto)
                                    End If
                                Next
                            End If

                            If incoerentiCentro.Count > 0 Then
                                For Each elem In incoerentiCentro
                                    impNonModificabili.Add(elem, dicImpianti(elem))
                                Next
                                For Each nonModificabile In impNonModificabili
                                    If dicImpiantiPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicImpiantiPostControlli.Remove(nonModificabile.Key)
                                        Dim Impianto As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Impianti, nonModificabile.Value, multiAzienda) &
                                            $" ({Gias.ValiditaCentro}: {nonModificabile.Value.Validita_Inizio_Centro.ToShortDateString()}-{nonModificabile.Value.Validita_Fine_Centro.ToShortDateString()}) "
                                        erroreValidita.Add(Impianto)
                                    End If
                                Next
                            End If

                            If incoerentiAzienda.Count > 0 Then
                                For Each elem In incoerentiCentro
                                    impNonModificabili.Add(elem, dicImpianti(elem))
                                Next
                                For Each nonModificabile In impNonModificabili
                                    If dicImpiantiPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicImpiantiPostControlli.Remove(nonModificabile.Key)
                                        Dim Impianto As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Impianti, nonModificabile.Value, multiAzienda) &
                                            $" ({Gias.ValiditaAzienda}: {nonModificabile.Value.Validita_Inizio_Azienda.ToShortDateString()}-{nonModificabile.Value.Validita_Fine_Azienda.ToShortDateString()}) "
                                        erroreValidita.Add(Impianto)
                                    End If
                                Next
                            End If

                            If incoerentiEsercizio.Count > 0 Then
                                For Each elem In incoerentiEsercizio
                                    impNonModificabili.Add(elem, dicImpianti(elem))
                                Next
                                For Each nonModificabile In impNonModificabili
                                    If dicImpiantiPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicImpiantiPostControlli.Remove(nonModificabile.Key)

                                        Dim Impianto As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Impianti, nonModificabile.Value, multiAzienda) &
                                            $" ({Gias.ValiditaEsercizio}{If(nonModificabile.Value.Lotto <> "", $" [{Gias.Lotto}: {nonModificabile.Value.Lotto}]", "")}: {nonModificabile.Value.Validita_Inizio_Esercizio.ToShortDateString()}-{nonModificabile.Value.Validita_Fine_Esercizio.ToShortDateString()}) "
                                        erroreValidita.Add(Impianto)
                                    End If
                                Next
                            End If

                            If dicImpiantiPostControlli.Count > 0 Then
                                campoModificato = $"Validita_Fine_Impianto newValue = [{valore_parametri_modificati("Data_Fine_Impianto").ToString()}]"

                                Dim listChiaviImpianto_POSTCLEANUP = dicImpiantiPostControlli.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza), CInt(item.Key.Id_Reg))).ToList()
                                objImpianto.UpdateColonna_Massivo(listChiaviImpianto_POSTCLEANUP, "Validita_Fine", CDate(valore_parametri_modificati("Data_Fine_Impianto").ToString), "date", timeStamp, objParametri_Server)

                                If aggiornaDataEsercizio.Count > 0 Then
                                    Dim listChiaviEsercizi_POSTCLEANUP = aggiornaDataEsercizio.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza), CInt(item.Value.Id_Reg), CInt(item.Value.Progetto_Cod))).ToList()
                                    objEsercizio.UpdateColonna_Massivo(listChiaviEsercizi_POSTCLEANUP, "Validita_Fine", CDate(valore_parametri_modificati("Data_Fine_Impianto").ToString), "date", timeStamp, objParametri_Server)

                                    'Scrittura tabella Agronica_Log_Anagrafe
                                    objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                                                                      Nothing,
                                                                                      listChiaviEsercizi_POSTCLEANUP,
                                                                                      Enum_EntitaModificaMultiplaPianoColturale.Esercizi,
                                                                                      timeStamp,
                                                                                      $"{NoteLog} ({campoModificato})",
                                                                                      objParametri_Server)


                                End If
                            End If
                        End If
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.IMP_DataInizioImpianto
                    If valore_parametri_modificati("Data_Inizio_Impianto") IsNot Nothing Then
                        nomeColonnaUpdate = "Data_Inizio_Impianto"
                        valoreUpdate = CDate(valore_parametri_modificati("Data_Inizio_Impianto").ToString())
                        campoModificato = $"Data_Inizio_Impianto newValue = [{valoreUpdate.ToString()}]"
                        dataTypeValoreUpdate = "date"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.IMP_SuFila
                    If valore_parametri_modificati("Su_Fila") IsNot Nothing Then
                        valoreCodiceUpdate = CDbl(valore_parametri_modificati("Su_Fila"))
                        codiceUpdate = enum_CodiciAnagrafe.Impianto_SuFila_Maschio
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.IMP_TraFila
                    If valore_parametri_modificati("Tra_Fila") IsNot Nothing Then
                        valoreCodiceUpdate = CDbl(valore_parametri_modificati("Tra_Fila"))
                        codiceUpdate = enum_CodiciAnagrafe.Impianto_TraFila_Maschio
                    End If
            End Select

            If nomeColonnaUpdate <> String.Empty AndAlso valoreUpdate <> String.Empty AndAlso valoreUpdate IsNot Nothing AndAlso
                parametro <> Enum_ParametriModificaMultiplaPianoColturale.IMP_DataFineImpianto Then
                objImpianto.UpdateColonna_Massivo(listaChiaviImpianti, nomeColonnaUpdate, valoreUpdate, dataTypeValoreUpdate, timeStamp, objParametri_Server)
            End If

            If codiceUpdate <> -1 Then
                objImpiantoCodici.InsertUpdateDeleteCodiciImpianto_Massivo(listaChiaviImpianti,
                                                                   If(valoreCodiceUpdate = "", enum_TipoOperazioneDB.Cancellazione, enum_TipoOperazioneDB.Modifica),
                                                                   codiceUpdate,
                                                                   valoreCodiceUpdate,
                                                                   timeStamp,
                                                                   objParametri_Server)
            End If

            If campoModificato <> "" Then
                objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                                                   listaChiaviImpianti,
                                                                   Nothing,
                                                                   Enum_EntitaModificaMultiplaPianoColturale.Impianti,
                                                                   timeStamp,
                                                                   $"{NoteLog} ({campoModificato})",
                                                                   objParametri_Server)
            End If

            If erroreMovimenti.Count > 0 Then
                errore += $"{Gias.ImpossibileModificareDataChiusuraImpiantiCostiMovimentiAssociati}: <br>- {String.Join($"<br>- ", erroreMovimenti)} <br>"
            End If
            If erroreValidita.Count > 0 Then
                If errore <> "" Then
                    errore += "<br>"
                End If
                errore += $"{Gias.ImpossibileModificareDataChiusuraImpiantiNonCoerenteEntita}: <br>- {String.Join($"<br>- ", erroreValidita)} <br>"
            End If

        Catch ex As GiasException
            'Errore gestito
            Throw ex
        Catch ex As Exception
            messaggioErrore = "Enum_ParametriModificaMultiplaPianoColturale Modificato: " & parametro.ToString() & vbCrLf &
            "Oggetto: " & JsonConvert.SerializeObject(valore_parametri_modificati) & vbCrLf &
            "Numero elementi in modifica: " & dicImpianti.Count().ToString() & vbCrLf &
            ex.Message

            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return errore

    End Function
    Private Function ModificaEsercizi_NEW(parametro As Integer,
                                          ByRef valore_parametri_modificati As JObject,
                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                          NoteLog As String) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Ereditatore.ModificaEsercizi_NEW()"
        Dim messaggioErrore As String = ""

        Dim campoModificato As String = ""

        Dim objLogAnagrafe As New AgronicaLogAnagrafe_W
        Dim objEsercizio As New Impresa_Progetti_W
        Dim objEserciziCodici As New Reg_Impianti_Codici_W

        Dim eseNonModificabili As New Dictionary(Of keyEsercizio, DatiAnagrafica)

        Dim nomeColonnaUpdate As String = String.Empty
        Dim valoreUpdate As String = String.Empty
        Dim dataTypeValoreUpdate As String = "string"

        Dim codiceUpdate As Integer = -1
        Dim valoreCodiceUpdate As String = String.Empty

        Dim erroreMovimenti As New List(Of String)
        Dim erroreValidita As New List(Of String)
        Dim errore As String = ""

        Try
            Dim timeStamp = Date.Now()

            'Mi serve come copia dell'oggetto orignale. Da questi verranno rimossi gli impianti non modificabili
            Dim dicEserciziPostControlli As New Dictionary(Of keyEsercizio, DatiAnagrafica)(dicEsercizi)

            'Da passare negli update e nei controlli
            Dim listaChiaviEsercizi = dicEsercizi.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza), CInt(item.Key.Id_Reg), CInt(item.Key.Progetto_Cod))).ToList()

            Dim multiAzienda As Boolean = dicEsercizi.AsEnumerable().Select(Function(x) (x.Key.Piva)).Distinct().ToList().Count > 1

            Dim erroreBIO As New List(Of String)

            Select Case parametro
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_CapitolatoPrivato
                    If valore_parametri_modificati("CapitolatoPrivato") IsNot Nothing Then
                        valoreCodiceUpdate = valore_parametri_modificati("CapitolatoPrivato").ToString()
                        codiceUpdate = enum_CodiciAnagrafe.Capitolato_Privato
                        campoModificato = $"CapitolatoPrivato newValue = [id_cod:{codiceUpdate}, val_cod:{If(valoreCodiceUpdate.ToString() = "", "NULL", valoreCodiceUpdate.ToString()) }]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_OrganismoReferente
                    If valore_parametri_modificati("OrganismoReferente") IsNot Nothing Then
                        valoreCodiceUpdate = valore_parametri_modificati("OrganismoReferente").ToString()
                        codiceUpdate = enum_CodiciAnagrafe.Organismo_Referente
                        campoModificato = $"OrganismoReferente newValue = [id_cod{codiceUpdate}, val_cod:{If(valoreCodiceUpdate.ToString() = "", "NULL", valoreCodiceUpdate.ToString()) }]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_MagazzinoConferimento
                    If valore_parametri_modificati("MagazzinoConferimento") IsNot Nothing Then
                        valoreCodiceUpdate = valore_parametri_modificati("MagazzinoConferimento").ToString()
                        codiceUpdate = enum_CodiciAnagrafe.Magazzino_Conferimento
                        campoModificato = $"MagazzinoConferimento newValue = [id_cod:{codiceUpdate}, val_cod:{If(valoreCodiceUpdate.ToString() = "", "NULL", valoreCodiceUpdate.ToString()) }]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_Certificazione
                    If valore_parametri_modificati("Certificazione") IsNot Nothing Then
                        valoreCodiceUpdate = valore_parametri_modificati("Certificazione").ToString()
                        codiceUpdate = enum_CodiciAnagrafe.Certificazione
                        campoModificato = $"Certificazione newValue = [id_cod:{codiceUpdate}, val_cod:{valoreCodiceUpdate.ToString()}]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_LimiteN
                    If valore_parametri_modificati("N") IsNot Nothing Then
                        valoreCodiceUpdate = If(IsNumeric((valore_parametri_modificati("N"))), CDbl(valore_parametri_modificati("N")), "")
                        codiceUpdate = enum_CodiciAnagrafe.Impianto_LimiteN
                        campoModificato = $"LimiteN newValue = [id_cod:{codiceUpdate}, val_cod:{If(valoreCodiceUpdate.ToString() = "", "NULL", valoreCodiceUpdate.ToString())}]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_LimiteP
                    If valore_parametri_modificati("P") IsNot Nothing Then
                        valoreCodiceUpdate = If(IsNumeric((valore_parametri_modificati("P"))), CDbl(valore_parametri_modificati("P")), "")
                        codiceUpdate = enum_CodiciAnagrafe.Impianto_LimiteP
                        campoModificato = $"LimiteP newValue = [id_cod:{codiceUpdate}, val_cod:{If(valoreCodiceUpdate.ToString() = "", "NULL", valoreCodiceUpdate.ToString())}]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_LimiteK
                    If valore_parametri_modificati("K") IsNot Nothing Then
                        valoreCodiceUpdate = If(IsNumeric((valore_parametri_modificati("K"))), CDbl(valore_parametri_modificati("K")), "")
                        codiceUpdate = enum_CodiciAnagrafe.Impianto_LimiteK
                        campoModificato = $"LimiteK newValue = [id_cod:{codiceUpdate}, val_cod:{If(valoreCodiceUpdate.ToString() = "", "NULL", valoreCodiceUpdate.ToString())}]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_Resa
                    If valore_parametri_modificati("Resa") IsNot Nothing Then
                        valoreUpdate = CDbl(valore_parametri_modificati("Resa").ToString())
                        nomeColonnaUpdate = "Produzione_Prevista"
                        campoModificato = $"Produzione_Prevista newValue = [{valoreUpdate}]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_DataSemina
                    If valore_parametri_modificati("Data_Semina") IsNot Nothing Then
                        valoreUpdate = valore_parametri_modificati("Data_Semina").ToString()
                        If valoreUpdate = "" Then
                            valoreUpdate = AGRODATAINIZIO.ToString()
                        End If
                        nomeColonnaUpdate = "Data_Inizio_Prevista"
                        campoModificato = $"Data_Inizio_Prevista newValue = [{valoreUpdate}]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_DataRaccolta
                    If valore_parametri_modificati("Data_Raccolta") IsNot Nothing Then
                        valoreUpdate = valore_parametri_modificati("Data_Raccolta").ToString()
                        If valoreUpdate = "" Then
                            valoreUpdate = AGRODATAFINE.ToString()
                        End If
                        nomeColonnaUpdate = "Data_Fine_Prevista"
                        campoModificato = $"Data_Fine_Prevista newValue = [{valoreUpdate}]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_DataFioritura
                    If valore_parametri_modificati("Data_Fioritura") IsNot Nothing Then
                        valoreUpdate = valore_parametri_modificati("Data_Fioritura").ToString()
                        If valoreUpdate = "" Then
                            valoreUpdate = AGRODATAFINE.ToString()
                        End If
                        nomeColonnaUpdate = "Data_Fioritura_Prevista"
                        campoModificato = $"Data_Fioritura_Prevista newValue = [{valoreUpdate}]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_Regolamento
                    If valore_parametri_modificati("Reg_Cod") IsNot Nothing Then
                        valoreUpdate = valore_parametri_modificati("Reg_Cod").ToString()

                        'Se vogliamo impostare un Regolamento Esercizio <> BIO devo verificare il metodo di produzione dell'appezzamento
                        'Su app bio, l'unico regolamento possibile è il BIO
                        If valoreUpdate <> "" AndAlso valoreUpdate <> CInt(enum_Cod_Regolamento.Regolamento_bio) Then
                            Dim incoerentiEsercizio As New List(Of keyEsercizio)
                            verifica_BioEsercizi(incoerentiEsercizio, objParametri_Server)

                            If incoerentiEsercizio.Count > 0 Then
                                For Each elem In incoerentiEsercizio
                                    eseNonModificabili.Add(elem, dicEsercizi(elem))
                                Next
                                For Each nonModificabile In eseNonModificabili
                                    If dicEserciziPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicEserciziPostControlli.Remove(nonModificabile.Key)

                                        Dim Esercizio As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Esercizi, nonModificabile.Value, multiAzienda)
                                        erroreBIO.Add(Esercizio)
                                    End If
                                Next
                            End If
                        End If

                        If dicEserciziPostControlli.Count > 0 Then
                            nomeColonnaUpdate = "Regolamento_Cod"
                            campoModificato = $"Regolamento_Cod newValue = [{valoreUpdate}]"
                        End If
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_DPI
                    If valore_parametri_modificati("Dpi_Cod") IsNot Nothing AndAlso valore_parametri_modificati("Flag_PubblicoPrivato") IsNot Nothing Then

                        campoModificato = $"Disciplinare_Cod e Disciplinare_PubblicoPrivato newValue = [{valore_parametri_modificati("Dpi_Cod").ToString()}, {valore_parametri_modificati("Flag_PubblicoPrivato").ToString()}]"

                        'Faccio l'update del flag pubblico/privato
                        objEsercizio.UpdateColonna_Massivo(listaChiaviEsercizi, "Disciplinare_Cod", valore_parametri_modificati("Dpi_Cod").ToString(), "number", timeStamp, objParametri_Server)
                        'Faccio l'update del flag pubblico/privato
                        objEsercizio.UpdateColonna_Massivo(listaChiaviEsercizi, "Disciplinare_PubblicoPrivato", valore_parametri_modificati("Flag_PubblicoPrivato").ToString(), "number", timeStamp, objParametri_Server)
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_FlagSecondoRaccolto
                    If valore_parametri_modificati("FlagSecondoRaccolto") IsNot Nothing Then
                        Dim dummy As String = valore_parametri_modificati("FlagSecondoRaccolto").ToString().ToLowerInvariant()
                        Select Case dummy
                            Case "true"
                                valoreUpdate = 1
                            Case "false"
                                valoreUpdate = 0
                        End Select
                        nomeColonnaUpdate = "FlagSecondoRaccolto"
                        campoModificato = $"FlagSecondoRaccolto newValue = [{dummy}]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_CertificazioneAziendale
                    If valore_parametri_modificati("CertificazioneAziendale") IsNot Nothing Then
                        valoreCodiceUpdate = valore_parametri_modificati("CertificazioneAziendale")
                        codiceUpdate = enum_CodiciAnagrafe.Codice_Certificazione
                        campoModificato = $"CertificazioneAziendale newValue = [id_cod:{codiceUpdate}, val_cod:{If(valoreCodiceUpdate.ToString() = "", "NULL", valoreCodiceUpdate.ToString()) }]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_Contributi
                    If valore_parametri_modificati("Contributi") IsNot Nothing Then
                        valoreCodiceUpdate = valore_parametri_modificati("Contributi")
                        codiceUpdate = enum_CodiciAnagrafe.Contributi
                        campoModificato = $"Contributi newValue = [id_cod:{codiceUpdate}, val_cod:{valoreCodiceUpdate.ToString()}]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_CertificazioneProdotto
                    If valore_parametri_modificati("CertificazioneProdotto") IsNot Nothing Then
                        valoreCodiceUpdate = valore_parametri_modificati("CertificazioneProdotto")
                        codiceUpdate = enum_CodiciAnagrafe.Codice_Certificazione_Prodotto
                        campoModificato = $"CertificazioneProdotto newValue = [id_cod:{codiceUpdate}, val_cod:{If(valoreCodiceUpdate.ToString() = "", "NULL", valoreCodiceUpdate.ToString()) }]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_Residuo
                    If valore_parametri_modificati("Residuo") IsNot Nothing Then
                        valoreCodiceUpdate = valore_parametri_modificati("Residuo")
                        codiceUpdate = enum_CodiciAnagrafe.Codice_Residuo
                        campoModificato = $"Residuo newValue = [id_cod:{codiceUpdate}, val_cod:{valoreCodiceUpdate.ToString()}]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_LicenzaColtivazione
                    If valore_parametri_modificati("LicenzaColtivazione") IsNot Nothing Then
                        valoreCodiceUpdate = valore_parametri_modificati("LicenzaColtivazione")
                        codiceUpdate = enum_CodiciAnagrafe.Zespri_Fasi_Fase
                        campoModificato = $"LicenzaColtivazione newValue = [id_cod:{codiceUpdate}, val_cod:{If(valoreCodiceUpdate.ToString() = "", "NULL", valoreCodiceUpdate.ToString()) }]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_RiferimentoTrasferimentoDati
                    If valore_parametri_modificati("RiferimentoTrasferimentoDati") IsNot Nothing Then
                        valoreCodiceUpdate = valore_parametri_modificati("RiferimentoTrasferimentoDati")
                        codiceUpdate = enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati
                        campoModificato = $"RiferimentoTrasferimentoDati newValue = [id_cod:{CInt(codiceUpdate)}, val_cod:{If(valoreCodiceUpdate.ToString() = "", "NULL", valoreCodiceUpdate.ToString()) }]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_Tecnico
                    If valore_parametri_modificati("Tecnico") IsNot Nothing Then
                        valoreCodiceUpdate = valore_parametri_modificati("Tecnico")
                        codiceUpdate = enum_CodiciAnagrafe.Tecnico
                        campoModificato = $"Tecnico newValue = [id_cod:{CInt(codiceUpdate)}, val_cod:{If(valoreCodiceUpdate.ToString() = "", "NULL", valoreCodiceUpdate.ToString()) }]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_PianoSemina
                    If valore_parametri_modificati("PianoSemina") IsNot Nothing Then
                        valoreCodiceUpdate = valore_parametri_modificati("PianoSemina")
                        codiceUpdate = enum_CodiciAnagrafe.Impianto_PianoSemina
                        campoModificato = $"PianoSemina newValue = [id_cod:{CInt(codiceUpdate)}, val_cod:{If(valoreCodiceUpdate.ToString() = "", "NULL", valoreCodiceUpdate.ToString()) }]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_Prodotto
                    If valore_parametri_modificati("Prodotto") IsNot Nothing Then
                        valoreUpdate = valore_parametri_modificati("Prodotto").ToString()
                        nomeColonnaUpdate = "Mat_Cod"
                        campoModificato = $"Mat_Cod newValue = [{valoreUpdate}]"
                    End If
                Case Enum_ParametriModificaMultiplaPianoColturale.ESE_DisciplinareMassimaleNPK
                    Dim Metodo_Produzione = valore_parametri_modificati("MetodoProduzione_Cod")
                    Dim Disciplinare = valore_parametri_modificati("Disciplinare")
                    Dim Regolamento_Cod = valore_parametri_modificati("Reg_Cod")
                    Dim Disciplinare_Cod = valore_parametri_modificati("Dpi_Cod")
                    Dim Regolamento_Concimazioni_Cod = valore_parametri_modificati("Regolamento_Concimazione_Cod")
                    Dim Disciplinare_PubblicoPrivato = valore_parametri_modificati("Flag_PubblicoPrivato")
                    Dim Stato_Impianto = valore_parametri_modificati("StatoImpianto_Cod")
                    Dim Finalita_Concimazione_Impianto = valore_parametri_modificati("Finalita_Concimazione_Impianto")
                    Dim IAF = valore_parametri_modificati("IAF")
                    Dim LimiteN = valore_parametri_modificati("N")
                    Dim LimiteP = valore_parametri_modificati("P")
                    Dim LimiteK = valore_parametri_modificati("K")

                    If Regolamento_Cod IsNot Nothing Then
                        If CInt(Regolamento_Cod.ToString()) <> CInt(enum_Cod_Regolamento.Regolamento_bio) Then
                            Dim incoerentiEsercizio As New List(Of keyEsercizio)
                            verifica_BioEsercizi(incoerentiEsercizio, objParametri_Server)

                            If incoerentiEsercizio.Count > 0 Then
                                For Each elem In incoerentiEsercizio
                                    eseNonModificabili.Add(elem, dicEsercizi(elem))
                                Next
                                For Each nonModificabile In eseNonModificabili
                                    If dicEserciziPostControlli.ContainsKey(nonModificabile.Key) Then
                                        dicEserciziPostControlli.Remove(nonModificabile.Key)

                                        Dim Esercizio As String = BuildDescrizionexErrore(Enum_EntitaModificaMultiplaPianoColturale.Esercizi, nonModificabile.Value, multiAzienda)
                                        erroreBIO.Add(Esercizio)
                                    End If
                                Next
                            End If
                        End If

                        If dicEserciziPostControlli.Count > 0 Then
                            Dim listaChiaviEsercizi_POSTCLEANUP = dicEserciziPostControlli.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza), CInt(item.Key.Id_Reg), CInt(item.Key.Progetto_Cod))).ToList()
                            objEsercizio.UpdateColonna_Massivo(listaChiaviEsercizi_POSTCLEANUP, "Regolamento_Cod", Regolamento_Cod, "number", timeStamp, objParametri_Server)

                            Dim dummy As String = $"Regolamento_Cod newValue = [{Regolamento_Cod}]"
                            objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                                   Nothing,
                                                   listaChiaviEsercizi,
                                                   Enum_EntitaModificaMultiplaPianoColturale.Esercizi,
                                                   timeStamp,
                                                   $"{NoteLog} ({dummy})",
                                                   objParametri_Server)
                        End If
                    End If

                    If Disciplinare_Cod IsNot Nothing Then
                        'Faccio l'update del flag pubblico/privato
                        objEsercizio.UpdateColonna_Massivo(listaChiaviEsercizi, "Disciplinare_Cod", Disciplinare_Cod, "number", timeStamp, objParametri_Server)

                        Dim dummy As String = $"Disciplinare_Cod newValue = [{Disciplinare_Cod}]"
                        objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                                   Nothing,
                                                   listaChiaviEsercizi,
                                                   Enum_EntitaModificaMultiplaPianoColturale.Esercizi,
                                                   timeStamp,
                                                   $"{NoteLog} ({dummy})",
                                                   objParametri_Server)
                    End If

                    If Disciplinare_PubblicoPrivato IsNot Nothing Then
                        'Faccio l'update del flag pubblico/privato
                        objEsercizio.UpdateColonna_Massivo(listaChiaviEsercizi, "Disciplinare_PubblicoPrivato", Disciplinare_PubblicoPrivato, "number", timeStamp, objParametri_Server)

                        Dim dummy As String = $"Disciplinare_PubblicoPrivato newValue = [{Disciplinare_PubblicoPrivato}]"
                        objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                                   Nothing,
                                                   listaChiaviEsercizi,
                                                   Enum_EntitaModificaMultiplaPianoColturale.Esercizi,
                                                   timeStamp,
                                                   $"{NoteLog} ({dummy})",
                                                   objParametri_Server)
                    End If

                    If Regolamento_Concimazioni_Cod IsNot Nothing Then
                        'Faccio l'update del flag pubblico/privato
                        objEsercizio.UpdateColonna_Massivo(listaChiaviEsercizi, "Regolamento_Concimazioni_Cod", Regolamento_Concimazioni_Cod, "number", timeStamp, objParametri_Server)

                        Dim dummy As String = $"Regolamento_Concimazioni_Cod newValue = [{Regolamento_Concimazioni_Cod}]"
                        objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                                   Nothing,
                                                   listaChiaviEsercizi,
                                                   Enum_EntitaModificaMultiplaPianoColturale.Esercizi,
                                                   timeStamp,
                                                   $"{NoteLog} ({dummy})",
                                                   objParametri_Server)
                    End If

                    If Stato_Impianto IsNot Nothing Then
                        'Faccio l'update del flag pubblico/privato
                        objEsercizio.UpdateColonna_Massivo(listaChiaviEsercizi, "Stato_Impianto", Stato_Impianto, "number", timeStamp, objParametri_Server)

                        Dim dummy As String = $"Stato_Impianto newValue = [{Stato_Impianto}]"
                        objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                                   Nothing,
                                                   listaChiaviEsercizi,
                                                   Enum_EntitaModificaMultiplaPianoColturale.Esercizi,
                                                   timeStamp,
                                                   $"{NoteLog} ({dummy})",
                                                   objParametri_Server)
                    End If

                    If Finalita_Concimazione_Impianto IsNot Nothing Then
                        objEserciziCodici.InsertUpdateDeleteCodiciEsercizio_Massivo(listaChiaviEsercizi,
                                                                                    If(Finalita_Concimazione_Impianto = "", enum_TipoOperazioneDB.Cancellazione, enum_TipoOperazioneDB.Modifica),
                                                                                    enum_CodiciAnagrafe.Finalita_Concimazione_Impianto,
                                                                                    Finalita_Concimazione_Impianto,
                                                                                    timeStamp,
                                                                                    objParametri_Server)
                        Dim dummy As String = $"Finalita_Concimazione_Impianto newValue = [id_cod:{CInt(enum_CodiciAnagrafe.Finalita_Concimazione_Impianto)}, val_cod:{Finalita_Concimazione_Impianto.ToString()}]"
                        objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                                   Nothing,
                                                   listaChiaviEsercizi,
                                                   Enum_EntitaModificaMultiplaPianoColturale.Esercizi,
                                                   timeStamp,
                                                   $"{NoteLog} ({dummy})",
                                                   objParametri_Server)
                    End If

                    If IAF IsNot Nothing Then
                        objEserciziCodici.InsertUpdateDeleteCodiciEsercizio_Massivo(listaChiaviEsercizi,
                                                                                    If(IAF = "", enum_TipoOperazioneDB.Cancellazione, enum_TipoOperazioneDB.Modifica),
                                                                                    enum_CodiciAnagrafe.Impianto_IAF_ImpegniAggiuntiviFacoltativi,
                                                                                    IAF,
                                                                                    timeStamp,
                                                                                    objParametri_Server)
                        Dim dummy As String = $"IAF newValue = [id_cod:{CInt(enum_CodiciAnagrafe.Impianto_IAF_ImpegniAggiuntiviFacoltativi)}, val_cod:{IAF.ToString()}]"
                        objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                                   Nothing,
                                                   listaChiaviEsercizi,
                                                   Enum_EntitaModificaMultiplaPianoColturale.Esercizi,
                                                   timeStamp,
                                                   $"{NoteLog} ({dummy})",
                                                   objParametri_Server)
                    End If

                    If LimiteN IsNot Nothing Then
                        objEserciziCodici.InsertUpdateDeleteCodiciEsercizio_Massivo(listaChiaviEsercizi,
                                                                                    If(LimiteN = "", enum_TipoOperazioneDB.Cancellazione, enum_TipoOperazioneDB.Modifica),
                                                                                    enum_CodiciAnagrafe.Impianto_LimiteN,
                                                                                    If(IsNumeric(LimiteN), CDbl(LimiteN), ""),
                                                                                    timeStamp,
                                                                                    objParametri_Server)
                        Dim dummy As String = $"LimiteN newValue = [id_cod:{CInt(enum_CodiciAnagrafe.Impianto_LimiteN)}, val_cod:{LimiteN.ToString()}]"
                        objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                                   Nothing,
                                                   listaChiaviEsercizi,
                                                   Enum_EntitaModificaMultiplaPianoColturale.Esercizi,
                                                   timeStamp,
                                                   $"{NoteLog} ({dummy})",
                                                   objParametri_Server)
                    End If

                    If LimiteP IsNot Nothing Then
                        objEserciziCodici.InsertUpdateDeleteCodiciEsercizio_Massivo(listaChiaviEsercizi,
                                                                                    If(LimiteP = "", enum_TipoOperazioneDB.Cancellazione, enum_TipoOperazioneDB.Modifica),
                                                                                    enum_CodiciAnagrafe.Impianto_LimiteP,
                                                                                    If(IsNumeric(LimiteP), CDbl(LimiteP), ""),
                                                                                    timeStamp,
                                                                                    objParametri_Server)
                        Dim dummy As String = $"LimiteP newValue = [id_cod:{CInt(enum_CodiciAnagrafe.Impianto_LimiteP)}, val_cod:{LimiteP.ToString()}]"
                        objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                                   Nothing,
                                                   listaChiaviEsercizi,
                                                   Enum_EntitaModificaMultiplaPianoColturale.Esercizi,
                                                   timeStamp,
                                                   $"{NoteLog} ({dummy})",
                                                   objParametri_Server)
                    End If

                    If LimiteK IsNot Nothing Then
                        objEserciziCodici.InsertUpdateDeleteCodiciEsercizio_Massivo(listaChiaviEsercizi,
                                                                                    If(LimiteK = "", enum_TipoOperazioneDB.Cancellazione, enum_TipoOperazioneDB.Modifica),
                                                                                    enum_CodiciAnagrafe.Impianto_LimiteK,
                                                                                    If(IsNumeric(LimiteK), CDbl(LimiteK), ""),
                                                                                    timeStamp,
                                                                                    objParametri_Server)
                        Dim dummy As String = $"LimiteK newValue = [id_cod:{CInt(enum_CodiciAnagrafe.Impianto_LimiteK)}, val_cod:{LimiteK.ToString()}]"
                        objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                                   Nothing,
                                                   listaChiaviEsercizi,
                                                   Enum_EntitaModificaMultiplaPianoColturale.Esercizi,
                                                   timeStamp,
                                                   $"{NoteLog} ({dummy})",
                                                   objParametri_Server)
                    End If
            End Select

            If dicEserciziPostControlli.Count > 0 Then
                If nomeColonnaUpdate <> String.Empty AndAlso valoreUpdate <> String.Empty AndAlso valoreUpdate IsNot Nothing AndAlso
               Not {Enum_ParametriModificaMultiplaPianoColturale.ESE_DPI,
                    Enum_ParametriModificaMultiplaPianoColturale.ESE_DisciplinareMassimaleNPK}.
                    Contains(parametro) Then
                    Dim listaChiaviEsercizi_POSTCLEANUP = dicEserciziPostControlli.AsEnumerable().Select(Function(item) (item.Key.Piva, CInt(item.Key.Sa_Cod), CInt(item.Key.Appezza), CInt(item.Key.Id_Reg), CInt(item.Key.Progetto_Cod))).ToList()
                    objEsercizio.UpdateColonna_Massivo(listaChiaviEsercizi_POSTCLEANUP, nomeColonnaUpdate, valoreUpdate, dataTypeValoreUpdate, timeStamp, objParametri_Server)
                End If

                If codiceUpdate <> -1 Then
                    objEserciziCodici.InsertUpdateDeleteCodiciEsercizio_Massivo(listaChiaviEsercizi,
                                                                                If(valoreCodiceUpdate = "", enum_TipoOperazioneDB.Cancellazione, enum_TipoOperazioneDB.Modifica),
                                                                                codiceUpdate,
                                                                                valoreCodiceUpdate,
                                                                                timeStamp,
                                                                                objParametri_Server)
                End If

                If campoModificato <> "" Then
                    objLogAnagrafe.ScriviLog_Massivo_ModificaMultipla(Nothing,
                                                                      Nothing,
                                                                      listaChiaviEsercizi,
                                                                      Enum_EntitaModificaMultiplaPianoColturale.Esercizi,
                                                                      timeStamp,
                                                                      $"{NoteLog} ({campoModificato})",
                                                                      objParametri_Server)
                End If
            End If

            If erroreBIO.Count > 0 Then
                If errore <> "" Then
                    errore += "<br>"
                End If
                errore += $"{Gias.ImpossibileModificareRegolamentoEserciziBIOAppezzamentiBIO}: <br>- {String.Join($"<br>- ", erroreBIO)} <br>"
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return errore

    End Function
    Private Function BuildDescrizionexErrore(entita As Enum_EntitaModificaMultiplaPianoColturale, nonModificabile As DatiAnagrafica, multiAzienda As Boolean) As String
        Dim descrizione As String = ""

        Select Case entita
            Case Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti
                descrizione = $"{If(multiAzienda = True, $"{Gias.Azienda}: {nonModificabile.Rag_Soc}, ", "")}{Gias.Centro}: {nonModificabile.Sa_Nome}, {nonModificabile.App_Nome}"
            Case Enum_EntitaModificaMultiplaPianoColturale.Impianti
                descrizione = $"{If(multiAzienda = True, $"{Gias.Azienda}: {nonModificabile.Rag_Soc}, ", "")}{Gias.Centro}: {nonModificabile.Sa_Nome}, {nonModificabile.App_Nome}, {Gias.Utilizzo}: {nonModificabile.Utilizzo}"
            Case Enum_EntitaModificaMultiplaPianoColturale.Esercizi
                descrizione = $"{If(multiAzienda = True, $"{Gias.Azienda}: {nonModificabile.Rag_Soc}, ", "")}{Gias.Centro}: {nonModificabile.Sa_Nome}, {nonModificabile.App_Nome}, {Gias.Utilizzo}: {nonModificabile.Utilizzo}{If(nonModificabile.Lotto <> "", $", {Gias.Lotto}: {nonModificabile.Lotto}", "")}"
        End Select

        Return descrizione
    End Function
#End Region


End Class

Public Class DatiAnagrafica
    Public Rag_Soc As String
    Public Validita_Inizio_Azienda As Date
    Public Validita_Fine_Azienda As Date

    Public Sa_Nome As String
    Public Validita_Inizio_Centro As Date
    Public Validita_Fine_Centro As Date

    Public App_Nome As String
    Public Validita_Inizio_Appezzamento As Date
    Public Validita_Fine_Appezzamento As Date
    Public MetodoProduzione As Integer
    Public SoloEserciziBio As Boolean

    Public hasCampo As Boolean
    Public Campo_Des As String
    Public Validita_Inizio_Campo As Date
    Public Validita_Fine_Campo As Date

    Public Id_Reg As Integer
    Public Utilizzo As String
    Public Validita_Inizio_Impianto As Date
    Public Validita_Fine_Impianto As Date

    Public Progetto_Cod As Integer
    Public Lotto As String
    Public Validita_Inizio_Esercizio As Date
    Public Validita_Fine_Esercizio As Date
    Public Regolamento As Integer

    Public EsercizioChiuso As Boolean
    Public ModificaEsercizioPostFiltroValidita As Boolean
End Class

Public Class keyAppezzamento
    Public Piva As String
    Public Sa_Cod As Integer
    Public Appezza As Integer

    ' Override di Equals per confrontare due oggetti keyAppezzamento
    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing Then
            Return False
        End If

        Dim other As keyAppezzamento = TryCast(obj, keyAppezzamento)
        If other Is Nothing Then
            Return False
        End If

        ' Compara le proprietà per determinare se due oggetti sono uguali
        Return Me.Piva = other.Piva AndAlso Me.Sa_Cod = other.Sa_Cod AndAlso Me.Appezza = other.Appezza
    End Function

    ' Override di GetHashCode per restituire un codice hash unico per ogni chiave
    Public Overrides Function GetHashCode() As Integer
        ' Combina i valori delle proprietà per calcolare il codice hash
        Dim hash As Integer = 17
        hash = hash Xor If(Piva Is Nothing, 0, Piva.GetHashCode())
        hash = hash Xor Sa_Cod.GetHashCode()
        hash = hash Xor Appezza.GetHashCode()
        Return hash
    End Function
End Class
Public Class keyImpianto
    Inherits keyAppezzamento
    Public Id_Reg As Integer

    ' Override di Equals per confrontare due oggetti keyAppezzamento
    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing Then
            Return False
        End If

        Dim other As keyImpianto = TryCast(obj, keyImpianto)
        If other Is Nothing Then
            Return False
        End If

        ' Compara le proprietà per determinare se due oggetti sono uguali
        Return Me.Piva = other.Piva AndAlso Me.Sa_Cod = other.Sa_Cod AndAlso Me.Appezza = other.Appezza AndAlso Me.Id_Reg = other.Id_Reg
    End Function

    ' Override di GetHashCode per restituire un codice hash unico per ogni chiave
    Public Overrides Function GetHashCode() As Integer
        ' Combina i valori delle proprietà per calcolare il codice hash
        Dim hash As Integer = 17
        hash = hash Xor If(Piva Is Nothing, 0, Piva.GetHashCode())
        hash = hash Xor Sa_Cod.GetHashCode()
        hash = hash Xor Appezza.GetHashCode()
        hash = hash Xor Id_Reg.GetHashCode()
        Return hash
    End Function
End Class
Public Class keyEsercizio
    Inherits keyImpianto
    Public Progetto_Cod As Integer

    ' Override di Equals per confrontare due oggetti keyAppezzamento
    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing Then
            Return False
        End If

        Dim other As keyEsercizio = TryCast(obj, keyEsercizio)
        If other Is Nothing Then
            Return False
        End If

        ' Compara le proprietà per determinare se due oggetti sono uguali
        Return Me.Piva = other.Piva AndAlso Me.Sa_Cod = other.Sa_Cod AndAlso Me.Appezza = other.Appezza AndAlso Me.Id_Reg = other.Id_Reg AndAlso Me.Progetto_Cod = other.Progetto_Cod
    End Function

    ' Override di GetHashCode per restituire un codice hash unico per ogni chiave
    Public Overrides Function GetHashCode() As Integer
        ' Combina i valori delle proprietà per calcolare il codice hash
        Dim hash As Integer = 17
        hash = hash Xor If(Piva Is Nothing, 0, Piva.GetHashCode())
        hash = hash Xor Sa_Cod.GetHashCode()
        hash = hash Xor Appezza.GetHashCode()
        hash = hash Xor Id_Reg.GetHashCode()
        hash = hash Xor Progetto_Cod.GetHashCode()
        Return hash
    End Function
End Class
