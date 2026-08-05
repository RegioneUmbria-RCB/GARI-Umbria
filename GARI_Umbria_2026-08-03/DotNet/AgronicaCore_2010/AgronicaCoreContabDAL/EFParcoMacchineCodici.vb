Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class EFParcoMacchineCodici
    Public Function Write(ByVal macchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                          ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByRef giasContext As Gias_DeveloperServer_Entities = Nothing,
                          Optional ByVal newTransaction As Boolean = True
                          ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFParcoMacchineCodici.Write"
        Dim scope As TransactionScope = Nothing
        Dim closeContext As Boolean = False

        If giasContext Is Nothing Then
            giasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            closeContext = True
        End If

        If newTransaction Then
            scope = New TransactionScope()
        End If

        Try
            If macchina.numero_certificato IsNot Nothing AndAlso macchina.numero_certificato <> "" Then
                Dim pcm = CreateObj(macchina, enum_CodiciAnagrafe.MacchinaCertificazioneTaratura, macchina.numero_certificato, objParametriServer)
                giasContext.Parco_Macchine_Codici.Add(pcm)
            End If

            giasContext.SaveChanges()

            If newTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            'Throw New Exception("[" & nomeRoutine & "] : " & ex.message)
            Return False
        End Try

        If closeContext Then
            giasContext.Dispose()
        End If

        Return True
    End Function

    Public Function Update(ByVal macchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                           ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByRef giasContext As Gias_DeveloperServer_Entities = Nothing,
                           Optional ByVal newTransaction As Boolean = True
                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFParcoMacchineCodici.Update"
        Dim scope As TransactionScope = Nothing
        Dim closeContext As Boolean = False

        If giasContext Is Nothing Then
            giasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            closeContext = True
        End If

        If newTransaction Then
            scope = New TransactionScope()
        End If

        Try
            Dim pcm = Retrive(macchina, enum_CodiciAnagrafe.MacchinaCertificazioneTaratura, giasContext)
            If macchina.numero_certificato IsNot Nothing AndAlso macchina.numero_certificato <> "" Then
                If IsNothing(pcm) Then
                    pcm = CreateObj(macchina, enum_CodiciAnagrafe.MacchinaCertificazioneTaratura, macchina.numero_certificato, objParametriServer)
                    giasContext.Parco_Macchine_Codici.Add(pcm)
                Else
                    pcm.val_cod = macchina.numero_certificato
                    giasContext.Parco_Macchine_Codici.Attach(pcm)
                    giasContext.Entry(pcm).State = EntityState.Modified
                End If
            Else
                If Not IsNothing(pcm) Then
                    giasContext.Parco_Macchine_Codici.Remove(pcm)
                End If
            End If

            giasContext.SaveChanges()

            If newTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            'Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
            Return False
        End Try

        If closeContext Then
            giasContext.Dispose()
        End If

        Return True
    End Function

    Public Function Remove(ByVal macchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                           ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByRef giasContext As Gias_DeveloperServer_Entities = Nothing,
                           Optional ByVal newTransaction As Boolean = True,
                           Optional ByVal saveChanges As Boolean = True
                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFParcoMacchineCodici.Update"
        Dim scope As TransactionScope = Nothing
        Dim closeContext As Boolean = False

        If giasContext Is Nothing Then
            giasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            closeContext = True
        End If

        If newTransaction Then
            scope = New TransactionScope()
        End If

        Try
            Dim pcm = Retrive(macchina, enum_CodiciAnagrafe.MacchinaCertificazioneTaratura, giasContext)
            If Not IsNothing(pcm) Then
                giasContext.Parco_Macchine_Codici.Remove(pcm)
            End If

            If saveChanges Then
                giasContext.SaveChanges()
            End If

            If newTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            'Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
            Return False
        End Try

        If closeContext Then
            giasContext.Dispose()
        End If

        Return True
    End Function

    Private Function CreateObj(ByVal macchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                               ByVal idCod As enum_CodiciAnagrafe,
                               ByVal valCod As String,
                               ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Parco_Macchine_Codici

        Dim pmc As New Parco_Macchine_Codici

        pmc.datainvio = DateTime.Today
        pmc.Data_Creazione = DateTime.Today
        pmc.Data_Modifica = DateTime.Today
        pmc.id_cod = idCod
        pmc.inviato = 0
        pmc.Mac_cod = macchina.codice
        pmc.PIVA = macchina.partitaIva
        pmc.sa_cod = macchina.centroPK.codice
        pmc.Username_Creazione = objParametriServer.UsernameOperazione
        pmc.Username_Modifica = objParametriServer.UsernameOperazione
        pmc.Validita_Fine = macchina.validita.fine
        pmc.Validita_Inizio = macchina.validita.inizio
        pmc.val_cod = valCod

        Return pmc
    End Function

    Public Function Retrive(ByVal macchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                             ByVal idCod As enum_CodiciAnagrafe,
                             ByRef giasContext As Gias_DeveloperServer_Entities) As Parco_Macchine_Codici

        Dim pcms = From pcm In giasContext.Parco_Macchine_Codici
                   Where pcm.Mac_cod = macchina.codice AndAlso
                       pcm.sa_cod = macchina.centroPK.codice AndAlso
                       pcm.PIVA = macchina.partitaIva AndAlso
                       pcm.id_cod = idCod
                   Select pcm

        Return pcms.FirstOrDefault()
    End Function
End Class
