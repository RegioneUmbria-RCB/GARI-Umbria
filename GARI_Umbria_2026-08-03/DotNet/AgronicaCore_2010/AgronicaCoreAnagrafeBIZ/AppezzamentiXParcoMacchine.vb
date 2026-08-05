Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Agea
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions

Public Class AppezzamentiXParcoMacchine_R
    Public Function Read(objServer As AgronicaCoreParametri,
                         objUtenti As AgronicaCoreParametri,
                         Optional piva As String = "",
                         Optional saCod As Int32 = 0,
                         Optional appezza As Int32 = 0,
                         Optional macCod As Int32 = 0,
                         Optional classCode As String = "",
                         Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                         Optional endtValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As List(Of LinkedMachine(Of Appezzamento.PK))

        Dim objDAL As New AgronicaCoreAnagrafeDAL.AppezzamentiXParcoMacchine_R

        CheckUserPermission(objUtenti)

        Dim linkedMachines As New List(Of LinkedMachine(Of Appezzamento.PK))
        Dim dt = objDAL.Read(
            objServer,
            piva,
            saCod,
            appezza,
            macCod,
            classCode,
            startValidity,
            endtValidity
            )

        For Each row In dt.Rows
            Dim machine As New LinkedMachine(Of Appezzamento.PK)
            machine.codice = row.Item("Mac_Cod")
            machine.partitaIva = row.Item("Piva")
            machine.linkedItemPK = New Appezzamento.PK(row.Item("Appezza"), New CentroAziendale.PK(row.Item("Sa_Cod"), row.Item("Piva")))
            machine.linkValidity = New IntervalloTemporale(CDate(row.Item("Validita_Inizio")), CDate(row.Item("Validita_Fine")))
            linkedMachines.Add(machine)
        Next

        Return linkedMachines
    End Function

    Public Function ReadJoinDescriptions(objServer As AgronicaCoreParametri,
                                         objUtenti As AgronicaCoreParametri,
                                         Optional piva As String = "",
                                         Optional saCod As Int32 = 0,
                                         Optional appezza As Int32 = 0,
                                         Optional macCod As Int32 = 0,
                                         Optional classCode As String = "",
                                         Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                                         Optional endtValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As List(Of LinkedMachine(Of Appezzamento.PK))

        Dim objDAL As New AgronicaCoreAnagrafeDAL.AppezzamentiXParcoMacchine_R

        CheckUserPermission(objUtenti)

        Dim linkedMachines As New List(Of LinkedMachine(Of Appezzamento.PK))
        Dim dt = objDAL.ReadJoinDescriptions(
            objServer,
            piva,
            saCod,
            appezza,
            macCod,
            classCode,
            startValidity,
            endtValidity
            )

        For Each row In dt.Rows
            Dim machine As New LinkedMachine(Of Appezzamento.PK)
            machine.descrizione = row.Item("Mac_Des")
            machine.codice = row.Item("Mac_Cod")
            machine.partitaIva = row.Item("Piva")
            machine.linkedItemPK = New Appezzamento.PK(row.Item("Appezza"), New CentroAziendale.PK(row.Item("Sa_Cod"), row.Item("Piva")))
            machine.linkValidity = New IntervalloTemporale(CDate(row.Item("Validita_Inizio")), CDate(row.Item("Validita_Fine")))
            linkedMachines.Add(machine)
        Next

        Return linkedMachines
    End Function

    Public Function ReadJoinLettureContatori(objServer As AgronicaCoreParametri,
                                             objUtenti As AgronicaCoreParametri,
                                             Optional plots As List(Of Appezzamento) = Nothing,
                                             Optional macCod As Int32 = 0) As DataTable

        Dim objDAL As New AgronicaCoreAnagrafeDAL.AppezzamentiXParcoMacchine_R

        CheckUserPermission(objUtenti)

        Dim dt As DataTable = Nothing
        If Not IsNothing(plots) Then
            For Each plot In plots
                Dim dtTemp = objDAL.ReadJoinLettureContatori(
                    objServer,
                    plot.primaryKey.centroAziendalePK.partitaIva,
                    plot.primaryKey.centroAziendalePK.codice,
                    plot.primaryKey.codice,
                    macCod
                    )

                If IsNothing(dt) Then
                    dt = dtTemp
                Else
                    dt.Merge(dtTemp)
                End If
            Next
        Else
            dt = objDAL.ReadJoinLettureContatori(objServer, macCod:=macCod)
        End If

        Return dt
    End Function

    Private Sub CheckUserPermission(objUtenti As AgronicaCoreParametri)
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permission As Boolean = objPermessi.Controlla_Permessi_Utente(
            objUtenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.GestioneAssociazioneAppezzamentiXParcoMacchine,
            enum_Security_Operazione.Lettura,
            Date.Now,
            "",
            objUtenti
            )

        If Not permission Then
            Throw New GiasException("L'utente non possiede i permessi per eseguire l'operazione")
        End If
    End Sub
End Class

Public Class AppezzamentiXParcoMacchine_W
    Public Function Write(plots As List(Of Appezzamento),
                          objServer As AgronicaCoreParametri,
                          objUtenti As AgronicaCoreParametri,
                          Optional giasContext As Gias_DeveloperServer_Entities = Nothing,
                          Optional openNewTransaction As Boolean = True) As Boolean

        Const routineName = "AgronicaCoreAnagrafeBIZ.AppezzamentiXParcoMacchine_W.Write()"
        Dim objDAL As New AgronicaCoreAnagrafeDAL.AppezzamentiXParcoMacchine_W
        Dim objPMBIZ As New AgronicaCoreContabBIZ.Parco_Macchine_R

        CheckUserPermission(objUtenti)

        Dim scope As TransactionScope = Nothing
        Dim closeContext As Boolean = IsNothing(giasContext)

        If IsNothing(giasContext) Then
            giasContext = Gias_EF_Utility.CreateGiasContextConnection(objServer.StringaConnessione)
        End If

        If openNewTransaction Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            scope = New TransactionScope(scopeOption, transactionOptions)
        End If

        Try
            For Each plt In plots
                For Each machine In plt.linkedMachines
                    If Not plt.validita.overlaps(machine.validita) Then
                        Throw New GiasException($"{My.Resources.AgronicaCoreAnagrafeBIZ.AgriculturalPlotsMachinesValiditiesNotOverlaps}: {plt.descrizione}")
                    End If

                    objDAL.Write(
                        plt.primaryKey.centroAziendalePK.partitaIva,
                        plt.primaryKey.centroAziendalePK.codice,
                        plt.primaryKey.codice,
                        machine.codice,
                        machine.linkValidity,
                        objServer
                        )
                Next
            Next

            If openNewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If

            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try

        If closeContext Then
            giasContext.Dispose()
        End If

        Return True
    End Function

    Public Function Edit(machine As AgronicaCoreModelsSTD.anagrafiche.LinkedMachine(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK),
                         objServer As AgronicaCoreParametri,
                         objUtenti As AgronicaCoreParametri) As Boolean

        Dim objDAL As New AgronicaCoreAnagrafeDAL.AppezzamentiXParcoMacchine_W
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        CheckUserPermission(objUtenti)

        Return objDAL.Edit(
            machine.linkValidity,
            objServer,
            machine.linkedItemPK.centroAziendalePK.partitaIva,
            machine.linkedItemPK.centroAziendalePK.codice,
            machine.linkedItemPK.codice,
            machine.codice
            )

    End Function

    Public Function Delete(objServer As AgronicaCoreParametri,
                           objUtenti As AgronicaCoreParametri,
                           Optional piva As String = "",
                           Optional saCod As Int32 = 0,
                           Optional appezza As Int32 = 0,
                           Optional macCod As Int32 = 0,
                           Optional classCode As String = "",
                           Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                           Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE,
                           Optional giasContext As Gias_DeveloperServer_Entities = Nothing,
                           Optional openNewTransaction As Boolean = True) As Boolean

        Const routineName = "AgronicaCoreAnagrafeBIZ.AppezzamentiXParcoMacchine_W.Delete()"
        Dim objDAL As New AgronicaCoreAnagrafeDAL.AppezzamentiXParcoMacchine_W

        CheckUserPermission(objUtenti)

        Dim scope As TransactionScope = Nothing
        Dim closeContext As Boolean = IsNothing(giasContext)

        If IsNothing(giasContext) Then
            giasContext = Gias_EF_Utility.CreateGiasContextConnection(objServer.StringaConnessione)
        End If

        If openNewTransaction Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            scope = New TransactionScope(scopeOption, transactionOptions)
        End If

        Try
            objDAL.Delete(
                objServer,
                piva,
                saCod,
                appezza,
                macCod,
                classCode,
                startValidity,
                endValidity
                )

            If openNewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If

            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try

        If closeContext Then
            giasContext.Dispose()
        End If

        Return True
    End Function

    Public Function DeleteRecords(plots As List(Of Appezzamento),
                                  objServer As AgronicaCoreParametri,
                                  objUtenti As AgronicaCoreParametri,
                                  Optional giasContext As Gias_DeveloperServer_Entities = Nothing,
                                  Optional openNewTransaction As Boolean = True) As Boolean

        Const routineName = "AgronicaCoreAnagrafeBIZ.AppezzamentiXParcoMacchine_W.DeleteRecords()"
        Dim objDALW As New AgronicaCoreAnagrafeDAL.AppezzamentiXParcoMacchine_W

        CheckUserPermission(objUtenti)

        Dim scope As TransactionScope = Nothing
        Dim closeContext As Boolean = IsNothing(giasContext)

        If IsNothing(giasContext) Then
            giasContext = Gias_EF_Utility.CreateGiasContextConnection(objServer.StringaConnessione)
        End If

        If openNewTransaction Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            scope = New TransactionScope(scopeOption, transactionOptions)
        End If

        Try
            For Each plt In plots
                If plt.linkedMachines.Count > 0 Then
                    ' if a machine was chosen then delete only the associations with that machine
                    For Each machine In plt.linkedMachines
                        objDALW.Delete(
                            objServer,
                            plt.primaryKey.centroAziendalePK.partitaIva,
                            plt.primaryKey.centroAziendalePK.codice,
                            plt.primaryKey.codice,
                            machine.codice
                            )
                    Next
                Else
                    objDALW.Delete(
                        objServer,
                        plt.primaryKey.centroAziendalePK.partitaIva,
                        plt.primaryKey.centroAziendalePK.codice,
                        plt.primaryKey.codice
                        )
                End If
            Next

            If openNewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If

            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try

        If closeContext Then
            giasContext.Dispose()
        End If

        Return True
    End Function

    Private Sub CheckUserPermission(objUtenti As AgronicaCoreParametri)
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permission As Boolean = objPermessi.Controlla_Permessi_Utente(
            objUtenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.GestioneAssociazioneAppezzamentiXParcoMacchine,
            enum_Security_Operazione.Modifica,
            Date.Now,
            "",
            objUtenti
            )

        If Not permission Then
            Throw New GiasException("L'utente non possiede i permessi per eseguire l'operazione")
        End If
    End Sub
End Class
