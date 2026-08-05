Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class EFIndirizzi
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Shared Function CreateIndirizziEF(ByRef dal As Gias_DeveloperServer_Entities,
                                             ByRef objParametri As AgronicaCoreParametri,
                                             ByRef username As String
                                             ) As Indirizzi

        Dim ind As New Indirizzi
        Dim idGen As New Agro_Sequenze

        ind.cod_indirizzo = idGen.NuovoId_Tabella_EF(dal, "Indirizzi", 0, 2000000, objParametri)
        ind.ind_des = ""
        ind.frz_des = ""
        ind.CAP = "00000"
        ind.com_des = ""
        ind.pro_cod = "00"
        ind.stato = "IT"
        ind.note = ""
        ind.pro_cod_istat = "000"
        ind.com_cod_istat = "000"
        ind.inviato = 0
        ind.datainvio = DateTime.Now
        ind.Data_Creazione = DateTime.Now
        ind.Data_Modifica = DateTime.Now
        ind.Validita_Inizio = AGRODATAINIZIO
        ind.Validita_Fine = AGRODATAFINE
        ind.Username_Creazione = username
        ind.Username_Modifica = username
        ind.Validazione = 0
        ind.Data_Validazione = DateTime.Now
        ind.UserName_Validazione = ""
        ind.Codice_Lingua = "it"
        ind.Codice_Alternativo = ""

        dal.Indirizzi.Add(ind)
        dal.SaveChanges()

        Return ind

    End Function

    Public Shared Function Indirizzo_Scrivi_EF(ByVal dati_indirizzo As AgronicaCoreModelsSTD.anagrafiche.Indirizzo,
                                         ByRef OUTPUT_IndirizzoCod As Integer,
                                         ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                         Optional ByVal NewTransaction As Boolean = True
                                         ) As Indirizzi
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFIndirizzi.Indirizzo_Scrivi_EF()"
        Dim messaggioErrore As String = ""

        Dim xRisp As Indirizzi = Nothing

        Dim Piva_SuperUser = objParametriServer.PivaSuperUser


        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)

        Dim InnerGiasContext As Gias_DeveloperServer_Entities = Nothing
        Dim scope As TransactionScope = Nothing

        Try
            InnerGiasContext = GiasContext
            If NewTransaction Then
                scope = New TransactionScope()
                InnerGiasContext = New Gias_DeveloperServer_Entities(EFConnString)
            End If

            Dim indirizzo = CreateIndirizziEF(InnerGiasContext,
                                              objParametriServer,
                                              If(objParametriUtenti.UtenteUsername <> "", objParametriUtenti.UtenteUsername, objParametriServer.UsernameOperazione)
                                              )

            OUTPUT_IndirizzoCod = indirizzo.cod_indirizzo
            indirizzo.ind_des = dati_indirizzo.via
            indirizzo.frz_des = dati_indirizzo.frazione
            indirizzo.CAP = dati_indirizzo.cap
            indirizzo.com_des = dati_indirizzo.istatComune.localita
            indirizzo.pro_cod = dati_indirizzo.istatComune.comuni_prov
            indirizzo.stato = dati_indirizzo.stato.codice
            indirizzo.note = dati_indirizzo.note
            indirizzo.pro_cod_istat = dati_indirizzo.istatComune.prov
            indirizzo.com_cod_istat = dati_indirizzo.istatComune.com

            InnerGiasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Indirizzi,
                                                                                 CStr(indirizzo.cod_indirizzo), Nothing,
                                                                                 Nothing, Nothing,
                                                                                 Nothing, Nothing,
                                                                                 enum_TipoOperazioneDB.Scrittura,
                                                                                 objParametriServer, enum_Id_Servizio.GiasOnline)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()
            xRisp = indirizzo

            If NewTransaction Then
                If success Then
                    scope.Complete()
                End If
                scope.Dispose()
                InnerGiasContext.Dispose()
            End If
        Catch ex As Exception
            xRisp = Nothing
            OUTPUT_IndirizzoCod = -1
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Shared Function Indirizzo_Modifica_EF(ByVal dati_indirizzo As AgronicaCoreModelsSTD.anagrafiche.Indirizzo,
                                            ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef username As String,
                                            Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                            Optional ByVal NewTransaction As Boolean = True
                                            ) As Indirizzi

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFIndirizzi.Indirizzo_Modifica_EF()"
        Dim messaggioErrore As String = ""

        Dim xRisp As Indirizzi = Nothing

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)

        Dim InnerGiasContext As Gias_DeveloperServer_Entities = Nothing
        Dim scope As TransactionScope = Nothing

        Try
            InnerGiasContext = GiasContext
            If NewTransaction Then
                scope = New TransactionScope()
                InnerGiasContext = New Gias_DeveloperServer_Entities(EFConnString)
            End If

            Dim indirizzi = From indirizzo In GiasContext.Indirizzi
                            Where indirizzo.cod_indirizzo = dati_indirizzo.codice
                            Select indirizzo

            Dim ind = indirizzi.FirstOrDefault

            ind.ind_des = dati_indirizzo.via
            ind.frz_des = dati_indirizzo.frazione
            ind.CAP = dati_indirizzo.cap
            ind.com_des = dati_indirizzo.istatComune.localita
            ind.pro_cod = ""
            ind.stato = dati_indirizzo.stato.codice
            If dati_indirizzo.note IsNot Nothing Then
                ind.note = dati_indirizzo.note
            End If
            ind.pro_cod_istat = dati_indirizzo.istatComune.prov
            ind.com_cod_istat = dati_indirizzo.istatComune.com
            ind.Data_Modifica = DateTime.Now
            ind.Username_Modifica = If(username <> "", username, objParametriServer.UsernameOperazione)
            ind.Validita_Inizio = AGRODATAINIZIO
            ind.Validita_Fine = AGRODATAFINE

            InnerGiasContext.Indirizzi.Attach(ind)
            InnerGiasContext.Entry(ind).State = EntityState.Modified
            InnerGiasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Indirizzi,
                                                                                 CStr(ind.cod_indirizzo), Nothing,
                                                                                 Nothing, Nothing,
                                                                                 Nothing, Nothing,
                                                                                 enum_TipoOperazioneDB.Modifica,
                                                                                 objParametriServer, enum_Id_Servizio.GiasOnline)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            xRisp = ind

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
                InnerGiasContext.Dispose()
            End If
        Catch ex As Exception
            xRisp = Nothing
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Shared Sub Indirizzo_Cancella_EF(ByVal dati_indirizzo As AgronicaCoreModelsSTD.anagrafiche.Indirizzo,
                                            ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                            Optional ByVal NewTransaction As Boolean = True
                                            )



        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFIndirizzi.Indirizzo_Cancella_EF"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)

        Dim InnerGiasContext As Gias_DeveloperServer_Entities = Nothing
        Dim scope As TransactionScope = Nothing

        InnerGiasContext = GiasContext
        If NewTransaction Then
            scope = New TransactionScope()
            InnerGiasContext = New Gias_DeveloperServer_Entities(EFConnString)
        End If

        Try

            Dim indirizzi = From indirizzo In GiasContext.Indirizzi
                            Where indirizzo.cod_indirizzo = dati_indirizzo.codice
                            Select indirizzo

            Dim ind = indirizzi.FirstOrDefault


            GiasContext.Indirizzi.Attach(ind)
            GiasContext.Indirizzi.Remove(ind)

            GiasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Indirizzi,
                                                                                 CStr(ind.cod_indirizzo), Nothing,
                                                                                 Nothing, Nothing,
                                                                                 Nothing, Nothing,
                                                                                 enum_TipoOperazioneDB.Cancellazione,
                                                                                 objParametriServer, enum_Id_Servizio.GiasOnline)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
                InnerGiasContext.Dispose()
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

End Class
