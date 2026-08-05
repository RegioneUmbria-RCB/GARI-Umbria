Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.anagrafiche

Public Class Budget_EFCampi
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Campo_Scrivi_EF(ByVal Id_Budget As Integer,
                                    ByVal DatiCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
                                    ByRef objParametriServer As AgronicaCoreParametri,
                                    ByRef objParametri_Utenti As AgronicaCoreParametri,
                                    ByVal username As String,
                                    Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                    Optional ByVal NewTransaction As Boolean = True
                                    ) As Budget_Campi

        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFCampi.Campo_Scrivi_EF"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        Dim campo = CreateCampo(Id_Budget,
                                DatiCampo.primaryKey.centroAziendalePK.partitaIva,
                                DatiCampo.primaryKey.centroAziendalePK.codice,
                                username,
                                objParametriServer,
                                objParametri_Utenti
                                )


        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try
            campo.Campo_Des = DatiCampo.descrizione

            If DatiCampo.descrizione Is Nothing OrElse DatiCampo.descrizione = "" Then
                Dim campo_des = ""

                Dim seqApp = From seq In GiasContext.SeqCampi
                             Where seq.Piva = DatiCampo.primaryKey.centroAziendalePK.partitaIva AndAlso
                                 seq.Sa_Cod = DatiCampo.primaryKey.centroAziendalePK.codice
                             Select seq

                Dim basecod As Long = 0
                If seqApp.FirstOrDefault() IsNot Nothing Then
                    basecod = seqApp.FirstOrDefault().Base
                End If

                campo_des = "Campo " & ((campo.Campo_Cod - basecod) Mod 1000).ToString("D3")

                campo.Campo_Des = campo_des
            End If


            campo.Sa_Cod = DatiCampo.primaryKey.centroAziendalePK.codice

            campo.Campo_Tipo = If(DatiCampo.serra, 1, 0)
            campo.Username_Creazione = username
            campo.Username_Modifica = username
            campo.Validita_Inizio = DatiCampo.validita.inizio
            campo.Validita_Fine = DatiCampo.validita.fine
            campo.Gru_Cod = DatiCampo.orientamento_Colturale
            campo.inviato = 0
            campo.UserName_Validazione = username
            campo.Validazione = 0
            campo.Veg_Cod = DatiCampo.specie.codice

            ' per scrittura dei codici c'è una funzione: AgronicaCoreanagrafeBIZ.Codici_W.Scrivi_Codici_Campo



            GiasContext.Budget_Campi.Add(campo)
            GiasContext.SaveChanges()

            Dim DatiCampoStr = ""
            If DatiCampo IsNot Nothing Then
                Dim a As New Newtonsoft.Json.JsonSerializerSettings With {.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local}
                DatiCampoStr = Newtonsoft.Json.JsonConvert.SerializeObject(DatiCampo, a)
            End If


            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Campi,
                                                                                 CStr(campo.Piva),
                                                                                 CStr(campo.Sa_Cod),
                                                                                 CStr(DatiCampo.primaryKey.codice),
                                                                                 Nothing, Nothing, Nothing,
                                                                                 TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                                 objParametriServer, enum_Id_Servizio.GiasOnline,
                                                                                 Id_Budget:=campo.Id_Budget, objectData:=DatiCampoStr)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            campo = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return campo
    End Function

    Public Shared Function Campo_Modifica_EF(ByVal Id_Budget As Integer,
                                             ByVal DatiCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
                                             ByRef objParametriServer As AgronicaCoreParametri,
                                             ByVal username As String,
                                             ByRef objParametriUtenti As AgronicaCoreParametri,
                                             Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                             Optional ByVal NewTransaction As Boolean = True,
                                             Optional NoteLog As String = NOTELOG_ANAGRAFE_NG
                                             ) As Budget_Campi

        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFCampi.Campo_Modifica_EF"
        Dim messaggioErrore As String = String.Empty
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim campi = From campo In GiasContext.Budget_Campi
                    Where campo.Id_Budget = Id_Budget AndAlso
                        campo.Piva = DatiCampo.primaryKey.centroAziendalePK.partitaIva AndAlso
                        campo.Campo_Cod = DatiCampo.primaryKey.codice AndAlso
                        campo.Sa_Cod = DatiCampo.primaryKey.centroAziendalePK.codice
                    Select campo

        Dim c = campi.FirstOrDefault()

        Try

            If c Is Nothing Then
                Throw New Exception("Campo non trovato")
            End If

            'c.Campo_Cod = DatiCampo.primaryKey.codice
            'c.Piva = DatiCampo.primaryKey.centroAziendalePK.partitaIva
            'c.Sa_Cod = DatiCampo.primaryKey.centroAziendalePK.codice
            c.Campo_Des = DatiCampo.descrizione
            c.Campo_Tipo = If(DatiCampo.serra, 1, 0)
            c.Username_Creazione = username
            c.Username_Modifica = username
            c.Validita_Inizio = DatiCampo.validita.inizio
            c.Validita_Fine = DatiCampo.validita.fine
            c.Gru_Cod = DatiCampo.orientamento_Colturale
            c.UserName_Validazione = username
            c.Veg_Cod = DatiCampo.specie.codice

            ' per scrittura dei codici c'è una funzione: AgronicaCoreanagrafeBIZ.Codici_W.Scrivi_Codici_Campo

            GiasContext.Budget_Campi.Attach(c)
            GiasContext.Entry(c).State = EntityState.Modified
            GiasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Campi,
                                                                                CStr(c.Piva), CStr(c.Sa_Cod),
                                                                                CStr(c.Campo_Cod), Nothing,
                                                                                Nothing, Nothing,
                                                                                TipiEnumerativi.enum_TipoOperazioneDB.Modifica,
                                                                                objParametriServer, enum_Id_Servizio.GiasOnline,
                                                                                Id_Budget:=c.Id_Budget, note:=NoteLog)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As Exception
            c = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return c
    End Function

    Public Function Create_CampiCodici(ByRef dal As Gias_DeveloperServer_Entities,
                                       ByRef objParametri As AgronicaCoreParametri,
                                       ByRef campo As AgronicaCoreEntityFramework_POCO.Budget_Campi,
                                       ByRef id_cod As Integer,
                                       ByRef val_cod As String,
                                       ByRef username As String
                                       ) As AgronicaCoreEntityFramework_POCO.Budget_Campi_Codici

        Dim camp = Create_CampiCodici(dal, objParametri, campo.Id_Budget, campo.Piva, campo.Sa_Cod, campo.Campo_Cod, id_cod, val_cod, username)

        Return camp
    End Function

    Private Function Create_CampiCodici(ByRef dal As Gias_DeveloperServer_Entities,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        ByRef Id_Budget As Integer,
                                        ByRef piva As String,
                                        ByRef sa_cod As Integer,
                                        ByRef campo_cod As Integer,
                                        ByRef id_cod As Integer,
                                        ByRef val_cod As String,
                                        ByRef username As String
                                        ) As AgronicaCoreEntityFramework_POCO.Budget_Campi_Codici

        Dim campo As New AgronicaCoreEntityFramework_POCO.Budget_Campi_Codici

        campo.Id_Budget = Id_Budget
        campo.PIVA = piva
        campo.sa_cod = sa_cod
        campo.campo_cod = campo_cod

        campo.id_cod = id_cod
        campo.val_cod = val_cod

        campo.inviato = 0
        campo.datainvio = DateTime.Now

        campo.Data_Creazione = DateTime.Now
        campo.Data_Modifica = DateTime.Now

        campo.Validita_Inizio = AGRODATAINIZIO
        campo.Validita_Fine = AGRODATAFINE

        campo.Username_Creazione = username
        campo.Username_Modifica = username

        campo.Validazione = 0
        campo.Data_Validazione = DateTime.Now
        campo.UserName_Validazione = ""

        dal.SaveChanges()

        Return campo
    End Function

    Private Function CreateCampo(ByRef Id_budget As Integer,
                                 ByRef piva As String,
                                 ByRef sa_cod As Integer,
                                 ByRef username As String,
                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreParametri
                                 ) As AgronicaCoreEntityFramework_POCO.Budget_Campi

        Dim campo As New AgronicaCoreEntityFramework_POCO.Budget_Campi

        campo.Campo_Cod = CreateNuovo_Campo_Cod(piva, sa_cod, objParametri_Server, objParametri_Utenti)

        campo.Id_Budget = Id_budget
        campo.Piva = piva
        campo.Sa_Cod = sa_cod
        campo.Campo_Des = ""
        campo.Campo_Tipo = 0
        campo.ConfiniRischio = ""
        campo.Conversione_Fine = AGRODATAFINE
        campo.Conversione_Inizio = AGRODATAINIZIO
        campo.datainvio = DateTime.Now
        campo.Data_Creazione = DateTime.Now
        campo.Data_Modifica = DateTime.Now
        campo.Username_Creazione = username
        campo.Username_Modifica = username
        campo.Validita_Inizio = AGRODATAINIZIO
        campo.Validita_Fine = AGRODATAFINE
        campo.Data_Validazione = AGRODATAINIZIO
        campo.Gru_Cod = 0
        campo.inviato = 0
        campo.SAU_Biologico = 0
        campo.SAU_Convenzionale = 0
        campo.SAU_Conversione = 0
        campo.SAU_Totale = 0
        campo.Username_Creazione = ""
        campo.Username_Modifica = ""
        campo.UserName_Validazione = ""
        campo.Validazione = 0
        campo.Veg_Cod = 0

        Return campo

    End Function

    Private Function CreateNuovo_Campo_Cod(ByVal piva As String,
                                           ByVal sa_cod As Integer,
                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreParametri) As Int32

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim ObjSequenze As New Agro_Sequenze

        Dim MessaggioErrore As String = String.Empty
        Dim idSeq As Integer = Nothing

        Dim base_code As Long
        Dim top_code As Long

        Try
            Dim dal As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
            dal.Calcola_BaseCode_TopCode(base_code, top_code, objParametri_Utenti)

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                idSeq = ObjSequenze.NuovoId_Campi(piva,
                                                  sa_cod,
                                                  base_code,
                                                  top_code,
                                                  objParametri_Server)
            End Using
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception(MessaggioErrore)
        End Try

        Return idSeq

    End Function

    Public Shared Sub Campo_Cancella_EF(ByVal Id_Budget As Integer,
                                           ByVal DatiCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
                                           ByRef objParametriServer As AgronicaCoreParametri,
                                           ByRef objParametriUtenti As AgronicaCoreParametri,
                                           Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                           Optional ByVal NewTransaction As Boolean = True,
                                           Optional NoteLog As String = NOTELOG_ANAGRAFE_NG
                                           )

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFCampi.Campo_Cancella_EF"
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
            Dim campi = From campo In GiasContext.Budget_Campi
                        Where campo.Id_Budget = Id_Budget AndAlso
                                    campo.Piva = DatiCampo.primaryKey.centroAziendalePK.partitaIva AndAlso
                                    campo.Campo_Cod = DatiCampo.primaryKey.codice
                        Select campo

            Dim c = campi.FirstOrDefault()

            If c Is Nothing Then
                Throw New Exception("Campo non trovato")
            End If

            ' per scrittura dei codici c'è una funzione: AgronicaCoreanagrafeBIZ.Codici_W.Scrivi_Codici_Campo

            GiasContext.Budget_Campi.Attach(c)
            GiasContext.Budget_Campi.Remove(c)
            GiasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Campi,
                                                                                 CStr(c.Piva), CStr(c.Sa_Cod),
                                                                                 CStr(c.Campo_Cod), Nothing,
                                                                                 Nothing, Nothing,
                                                                                 TipiEnumerativi.enum_TipoOperazioneDB.Modifica,
                                                                                 objParametriServer, enum_Id_Servizio.GiasOnline,
                                                                                 Id_Budget:=c.Id_Budget, note:=NoteLog)

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
