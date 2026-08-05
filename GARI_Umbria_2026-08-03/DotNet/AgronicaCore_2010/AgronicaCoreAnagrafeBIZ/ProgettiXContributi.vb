Imports System.Linq
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreModelsSTD.metaschema

Public Class ProgettiXContributi_R
    Public Function Read(objServer As AgronicaCoreParametri,
                         objUtenti As AgronicaCoreParametri,
                         Optional project As Int32 = 0,
                         Optional contribute As Int32 = 0,
                         Optional type As Int32 = 0,
                         Optional piva As String = "",
                         Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                         Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As List(Of LinkedContribute(Of KeyValuePair(Of Int32, String)))

        Dim objDAL As New AgronicaCoreAnagrafeDAL.ProgettiXContributi_R

        CheckUserPermission(objUtenti)

        Dim linkedContributes As New List(Of LinkedContribute(Of KeyValuePair(Of Int32, String)))
        Dim dt = objDAL.Read(
            objServer,
            project:=project,
            contribute:=contribute,
            type:=type,
            piva:=piva,
            startValidity:=startValidity,
            endValidity:=endValidity
            )

        For Each row In dt.Rows
            Dim linkedContribute As New LinkedContribute(Of KeyValuePair(Of Int32, String))(
                row.Item("ContributoCod"),
                row.Item("ContributoTipo"),
                New KeyValuePair(Of Int32, String)(row.Item("ProgettoCod"), row.Item("Piva"))
                )
            linkedContribute.description = row.Item("ContributoDes")
            linkedContribute.linkValidity = New IntervalloTemporale(CDate(row.Item("Validita_Inizio")), CDate(row.Item("Validita_Fine")))
            linkedContribute.validity = New IntervalloTemporale(CDate(row.Item("ContributoValidita_Inizio")), CDate(row.Item("ContributoValidita_Fine")))
            linkedContributes.Add(linkedContribute)
        Next

        Return linkedContributes

    End Function

    Private Sub CheckUserPermission(objUtenti As AgronicaCoreParametri)
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim contributesP As Boolean = objPermessi.Controlla_Permessi_Utente(
            objUtenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.GestioneContributiACA,
            enum_Security_Operazione.Lettura,
            Date.Now,
            "",
            objUtenti
            )

        Dim plantsP As Boolean = objPermessi.Controlla_Permessi_Utente(
            objUtenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.Anagrafica_Impianto,
            enum_Security_Operazione.Lettura,
            Date.Now,
            "",
            objUtenti
            )

        If Not plantsP AndAlso Not contributesP Then
            Throw New GiasException("L'utente non possiede i permessi per leggere le associazioni Contributi - Esercizi")
        End If
    End Sub
End Class

Public Class ProgettiXContributi_W

    Public Function Write(project As Int32,
                          contribute As Int32,
                          type As Int32,
                          piva As String,
                          objServer As AgronicaCoreParametri,
                          objUtenti As AgronicaCoreParametri,
                          Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                          Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE,
                          Optional giasContext As Gias_DeveloperServer_Entities = Nothing,
                          Optional openNewTransaction As Boolean = True) As Boolean

        Const routineName = "AgronicaCoreAnagrafeBIZ.ProgettiXContributi.Write()"
        Dim objDAL As New AgronicaCoreAnagrafeDAL.ProgettiXContributi_W
        Dim objContributeBIZ As New AgronicaCoreMetaSchemaBIZ.Contribute_R
        Dim result As Boolean = False

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
            If contribute = 0 Then
                Throw New ArgumentException("Wrong param", "contribute")
            End If

            If project = 0 Then
                Throw New ArgumentException("Wrong param", "project")
            End If

            Dim contr = objContributeBIZ.Read(objServer, objUtenti, contribute, type)
            If contr.Count = 1 AndAlso Not contr(0).validity.overlaps(New IntervalloTemporale(startValidity, endValidity)) Then
                Throw New GiasException($"La validità dell'associazione non è compatibile con quella del contributo")
            End If

            result = objDAL.Write(
                project:=project,
                contribute:=contribute,
                type:=type,
                piva:=piva,
                objServer:=objServer,
                startValidity:=startValidity,
                endValidity:=endValidity
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

        Return result

    End Function

    Public Function Edit(objServer As AgronicaCoreParametri,
                         objUtenti As AgronicaCoreParametri,
                         Optional project As Int32 = 0,
                         Optional contribute As Int32 = 0,
                         Optional type As Int32 = 0,
                         Optional piva As String = "",
                         Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                         Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As Boolean

        Dim objDAL As New AgronicaCoreAnagrafeDAL.ProgettiXContributi_W
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        CheckUserPermission(objUtenti)

        Return objDAL.Edit(
            objServer,
            project:=project,
            contribute:=contribute,
            type:=type,
            piva:=piva,
            startValidity:=startValidity,
            endValidity:=endValidity
            )

    End Function

    Public Function Delete(objServer As AgronicaCoreParametri,
                           objUtenti As AgronicaCoreParametri,
                           Optional piva As String = "",
                           Optional project As Int32 = 0,
                           Optional contribute As Int32 = 0,
                           Optional type As Int32 = 0,
                           Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                           Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE,
                           Optional giasContext As Gias_DeveloperServer_Entities = Nothing,
                           Optional openNewTransaction As Boolean = True) As Boolean

        Const routineName = "AgronicaCoreAnagrafeBIZ.ProgettiXContributi.Delete()"
        Dim objDAL As New AgronicaCoreAnagrafeDAL.ProgettiXContributi_W

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
                project:=project,
                contribute:=contribute,
                type:=type,
                piva:=piva,
                startValidity:=startValidity,
                endValidity:=endValidity
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

    Public Function Update(objServer As AgronicaCoreParametri,
                           objUtenti As AgronicaCoreParametri,
                           links As List(Of LinkedContribute(Of KeyValuePair(Of Int32, String))),
                           Optional giasContext As Gias_DeveloperServer_Entities = Nothing,
                           Optional openNewTransaction As Boolean = True) As Boolean

        Const routineName = "AgronicaCoreAnagrafeBIZ.ProgettiXContributi.Update()"
        Dim objDAL As New AgronicaCoreAnagrafeDAL.ProgettiXContributi_W
        Dim objBIZ_R As New ProgettiXContributi_R

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
            If Not IsNothing(links) Then
                Dim pks As New HashSet(Of KeyValuePair(Of Int32, String))(
                    links.Select(Of KeyValuePair(Of Int32, String))(
                    Function(l) New KeyValuePair(Of Int32, String)(l.linkedItemPK.Key, l.linkedItemPK.Value))
                    )
                Dim linksSet As New HashSet(Of Tuple(Of Int32, Int32, Int32, String))(
                    links.Select(Of Tuple(Of Int32, Int32, Int32, String))(
                    Function(l) New Tuple(Of Int32, Int32, Int32, String)(l.code, l.type, l.linkedItemPK.Key, l.linkedItemPK.Value))
                    )

                For Each pk In pks
                    Dim records = objBIZ_R.Read(
                        objServer:=objServer,
                        objUtenti:=objUtenti,
                        project:=pk.Key,
                        piva:=pk.Value
                        )

                    Dim recordsSet As New HashSet(Of Tuple(Of Int32, Int32, Int32, String))(
                        records.Select(Of Tuple(Of Int32, Int32, Int32, String))(
                        Function(r) New Tuple(Of Int32, Int32, Int32, String)(r.code, r.type, r.linkedItemPK.Key, r.linkedItemPK.Value))
                        )
                    Dim recordsToDelete = recordsSet.Except(linksSet)

                    For Each r In recordsToDelete
                        Me.Delete(objServer:=objServer, objUtenti:=objUtenti, project:=r.Item3, contribute:=r.Item1, type:=r.Item2, piva:=r.Item4)
                    Next
                Next

                For Each l In links
                    If l.code <> 0 AndAlso l.linkedItemPK.Key <> 0 Then
                        Me.Write(
                            project:=l.linkedItemPK.Key,
                            contribute:=l.code,
                            type:=l.type,
                            piva:=l.linkedItemPK.Value,
                            objServer:=objServer,
                            objUtenti:=objUtenti,
                            giasContext:=giasContext,
                            openNewTransaction:=False
                            )
                    End If
                Next
            End If

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

    Public Function WriteRecords(objServer As AgronicaCoreParametri,
                                 objUtenti As AgronicaCoreParametri,
                                 exs As List(Of Esercizio),
                                 Optional giasContext As Gias_DeveloperServer_Entities = Nothing,
                                 Optional openNewTransaction As Boolean = True) As Boolean

        Const routineName = "AgronicaCoreAnagrafeBIZ.ProgettiXContributi.Write()"
        Dim objDAL As New AgronicaCoreAnagrafeDAL.ProgettiXContributi_W
        Dim objContributeBIZ As New AgronicaCoreMetaSchemaBIZ.Contribute_R
        Dim result As Boolean = False

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
            For Each e In exs

                If Not IsNothing(e.acaContributes) Then
                    For Each c In e.acaContributes
                        Dim contr = objContributeBIZ.Read(objServer, objUtenti, c.code, c.type)
                        If contr.Count = 1 AndAlso Not contr(0).validity.overlaps(e.validita) Then
                            Throw New GiasException($"La validità dell'esercizio non è compatibile con quella del contributo")
                        End If

                        result = objDAL.Write(
                            project:=e.codice,
                            contribute:=c.code,
                            type:=c.type,
                            piva:=e.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                            objServer:=objServer
                            )

                        If Not result Then
                            Throw New GiasException("Errore durante il salvataggio")
                        End If
                    Next
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

        Return result

    End Function

    Private Sub CheckUserPermission(objUtenti As AgronicaCoreParametri)
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permission As Boolean = objPermessi.Controlla_Permessi_Utente(
            objUtenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.GestioneContributiACA,
            enum_Security_Operazione.Modifica,
            Date.Now,
            "",
            objUtenti
            )

        Dim plantsP As Boolean = objPermessi.Controlla_Permessi_Utente(
            objUtenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.Anagrafica_Impianto,
            enum_Security_Operazione.Modifica,
            Date.Now,
            "",
            objUtenti
            )

        If Not permission AndAlso Not plantsP Then
            Throw New GiasException("L'utente non possiede i permessi per modificare le associazioni Contributi - Esercizi")
        End If
    End Sub
End Class
