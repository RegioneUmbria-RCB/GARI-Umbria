Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD.exceptions

Public Class EFGruppoRaccolta

    Private Shared Function NuovoGruppoRaccolta_Cod(ByRef GiasContext As Gias_DeveloperServer_Entities, ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim idGen As New Agro_Sequenze
        Dim esercizio = idGen.NuovoId_Tabella_EF(GiasContext, "Gruppi_Raccolta", 0, 2000000000, objParametri)

        Return esercizio
    End Function

    Private Shared Function Create_GruppoRaccolta(
        ByRef dal As Gias_DeveloperServer_Entities,
        ByRef objParametri As AgronicaCoreParametri,
        ByRef descr As String,
        ByRef username As String
    ) As Gruppi_Raccolta

        Dim gr As New Gruppi_Raccolta

        gr.GruppoRaccolta_Cod = NuovoGruppoRaccolta_Cod(dal, objParametri)
        gr.GruppoRaccolta_Des = descr

        gr.Validita_Inizio = AGRODATAINIZIO
        gr.Validita_Fine = AGRODATAFINE
        gr.inviato = 0
        gr.datainvio = AGRODATAINIZIO
        gr.Data_Creazione = DateTime.Now
        gr.Data_Modifica = DateTime.Now
        gr.Username_Creazione = username
        gr.Username_Modifica = username
        gr.Validazione = 0
        gr.Data_Validazione = AGRODATAINIZIO
        gr.Username_Validazione = username

        Return gr
    End Function

    Public Shared Function GruppoRaccolta_Scrivi_EF(
        ByVal Dati_GruppoRaccolta As AgronicaCoreModelsSTD.anagrafiche.GruppoRaccolta,
        ByRef objParametriServer As AgronicaCoreParametri,
        ByVal username As String,
        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
        Optional ByVal NewTransaction As Boolean = True,
        Optional NoteLog As String = "",
        Optional AggiornaSoloValidita As Boolean = False
    ) As Gruppi_Raccolta

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFGruppoRaccolta.GruppoRaccolta_Scrivi_EF()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim gr = Create_GruppoRaccolta(GiasContext,
                                         objParametriServer,
                                         Dati_GruppoRaccolta.descrizione,
                                         username
                                         )


        Try

            gr.GruppoRaccolta_Des = Dati_GruppoRaccolta.descrizione
            gr.Validita_Inizio = Dati_GruppoRaccolta.validita.inizio
            gr.Validita_Fine = Dati_GruppoRaccolta.validita.fine

            GiasContext.Gruppi_Raccolta.Add(gr)
            GiasContext.SaveChanges()

            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim DatiEsercizioStr = JsonConvert.SerializeObject(Dati_GruppoRaccolta, a)

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.GruppiRaccolta,
                                                        CStr(gr.GruppoRaccolta_Cod), gr.GruppoRaccolta_Des,
                                                        CStr(gr.Validita_Inizio), CStr(gr.Validita_Fine),
                                                        "", Nothing,
                                                        enum_TipoOperazioneDB.Scrittura,
                                                        objParametriServer, enum_Id_Servizio.GiasOnline,
                                                        NoteLog, DatiEsercizioStr)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return gr

    End Function

    Public Shared Function GruppoRaccolta_Modifica_EF(
        ByVal Dati_GruppoRaccolta As AgronicaCoreModelsSTD.anagrafiche.GruppoRaccolta,
        ByRef objParametriServer As AgronicaCoreParametri,
        ByVal username As String,
        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
        Optional ByVal NewTransaction As Boolean = True,
        Optional NoteLog As String = "",
        Optional AggiornaSoloValidita As Boolean = False
    ) As Gruppi_Raccolta

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.GruppiRaccolta_Write.GruppoRaccolta_Modifica_EF()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim gruppiRaccolta = From grupporaccolta In GiasContext.Gruppi_Raccolta
                             Where grupporaccolta.GruppoRaccolta_Cod = Dati_GruppoRaccolta.codice
                             Select grupporaccolta

        Dim gr = gruppiRaccolta.FirstOrDefault
        If gr Is Nothing Then
            Throw New GiasException("GruppoRaccolta (" & Dati_GruppoRaccolta.codice.ToString() &
                                               ") non trovato in anagrafica. Impossibile proseguire")
        End If

        Try
            gr.GruppoRaccolta_Des = Dati_GruppoRaccolta.descrizione
            gr.Validita_Inizio = Dati_GruppoRaccolta.validita.inizio
            gr.Validita_Fine = Dati_GruppoRaccolta.validita.fine

            GiasContext.Gruppi_Raccolta.Attach(gr)
            GiasContext.Entry(gr).State = EntityState.Modified
            GiasContext.SaveChanges()

            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim DatiGruppoRaccoltaStr = JsonConvert.SerializeObject(Dati_GruppoRaccolta, a)

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.GruppiRaccolta,
                                                        CStr(gr.GruppoRaccolta_Cod), gr.GruppoRaccolta_Des,
                                                        CStr(gr.Validita_Inizio), CStr(gr.Validita_Fine),
                                                        "", Nothing,
                                                        enum_TipoOperazioneDB.Modifica,
                                                        objParametriServer, enum_Id_Servizio.GiasOnline,
                                                        NoteLog, DatiGruppoRaccoltaStr)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return gr

    End Function

    Public Shared Sub GruppoRaccolta_Cancella_EF(ByVal Dati_GruppoRaccolta As AgronicaCoreModelsSTD.anagrafiche.GruppoRaccolta,
                                                 ByRef objParametriServer As AgronicaCoreParametri,
                                                 Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                 Optional ByVal NewTransaction As Boolean = True
                                                )

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFEsercizi.Esercizio_Cancella_EF()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            Dim gruppiraccolta = From grupporaccolta In GiasContext.Gruppi_Raccolta
                                 Where grupporaccolta.GruppoRaccolta_Cod = Dati_GruppoRaccolta.codice
                                 Select grupporaccolta

            Dim gr = gruppiraccolta.FirstOrDefault

            If gr Is Nothing Then
                Throw New Exception("GruppoRaccolta (" & Dati_GruppoRaccolta.codice.ToString() &
                                               ") non trovato in anagrafica. Impossibile proseguire")
            End If


            GiasContext.Gruppi_Raccolta.Attach(gr)
            GiasContext.Gruppi_Raccolta.Remove(gr)
            GiasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Progetti,
                                                        CStr(gr.GruppoRaccolta_Cod), gr.GruppoRaccolta_Des,
                                                        CStr(gr.Validita_Inizio), CStr(gr.Validita_Fine),
                                                        "", Nothing,
                                                        enum_TipoOperazioneDB.Cancellazione,
                                                        objParametriServer, enum_Id_Servizio.GiasOnline)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

    End Sub

End Class
