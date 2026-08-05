Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreModelsSTD.metaschema

Public Class Contribute_R
    Public Function Read(objServer As AgronicaCoreParametri,
                         objUtenti As AgronicaCoreParametri,
                         Optional code As Int32 = 0,
                         Optional type As Int32 = 0,
                         Optional description As String = "",
                         Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                         Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As List(Of Contribute)

        Dim objDAL As New AgronicaCoreMetaSchemaDAL.Contribute_R

        ' waiting to know the correct permission required
        'CheckUserPermission(objUtenti)

        Dim contributes As New List(Of Contribute)
        Dim dt = objDAL.Read(
            objServer,
            code,
            type,
            description,
            startValidity,
            endValidity
            )

        For Each row In dt.Rows
            Dim contribute As New Contribute
            contribute.code = row.Item("ContributoCod")
            contribute.type = row.Item("Tipo")
            contribute.description = row.Item("ContributoDes")
            contribute.validity = New IntervalloTemporale(CDate(row.Item("Validita_Inizio")), CDate(row.Item("Validita_Fine")))
            contributes.Add(contribute)
        Next

        Return contributes

    End Function

    Private Sub CheckUserPermission(objUtenti As AgronicaCoreParametri)
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permission As Boolean = objPermessi.Controlla_Permessi_Utente(
            objUtenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.GestioneContributiACA,
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

Public Class Contribute_W

    Public Function Edit(code As Int32,
                         type As Int32,
                         objServer As AgronicaCoreParametri,
                         Optional description As String = "",
                         Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                         Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As Boolean

        Const routineName = "AgronicaCoreMetaSchemaBIZ.Contribute_W.Edit()"
        Dim objDAL As New AgronicaCoreMetaSchemaDAL.Contribute_W

        ' waiting to know the correct permission required
        'CheckUserPermission(objUtenti)

        Try
            Return objDAL.Edit(
                code,
                type,
                objServer,
                description,
                startValidity,
                endValidity
                )
        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try

    End Function

    Public Function Delete(objServer As AgronicaCoreParametri,
                           Optional code As Int32 = 0,
                           Optional type As Int32 = 0,
                           Optional description As String = "",
                           Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                           Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As Boolean

        Const routineName = "AgronicaCoreMetaSchemaBIZ.Contribute_W.Delete()"
        Dim objDAL As New AgronicaCoreMetaSchemaDAL.Contribute_W

        ' waiting to know the correct permission required
        'CheckUserPermission(objUtenti)

        Try
            Return objDAL.Delete(
                objServer,
                code,
                type,
                description,
                startValidity,
                endValidity
                )
        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try

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

        If Not permission Then
            Throw New GiasException("L'utente non possiede i permessi per eseguire l'operazione")
        End If
    End Sub
End Class
