Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class jDeereDataModelDAL_AccountXOrganization_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Test(
                ByVal Username As String,
                ByVal OrgID As Integer,
                ByRef objParametri As AgronicaCoreParametri
        ) As Boolean

        Dim TestataElem As jDeereDataModel_AccountXOrganization = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModelDAL_AccountXOrganization_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_AccountXOrganization
            Where m.IDOrganization = OrgID AndAlso
                m.jDeereDataModel_Account.UsernaName = Username
            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(TestataElem)


    End Function


    Public Function Leggi(
                ByVal Username As String,
                ByVal OrgID As Integer,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As jDeereDataModel_AccountXOrganization

        Dim TestataElem As jDeereDataModel_AccountXOrganization = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModelDAL_AccountXOrganization_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_AccountXOrganization
            Where m.IDOrganization = OrgID AndAlso
                m.jDeereDataModel_Account.UsernaName = Username
            Select m).FirstOrDefault()

        End Using

        Return TestataElem


    End Function
End Class


Public Class jDeereDataModelDAL_AccountXOrganization_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Aggiorna_jDeereDataModelDAL_AccountXOrganization(
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AgronicaCoreMetaschemaDAL.Aggiorna_jDeereDataModelDAL_AccountXOrganization()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each curjDeereDataModelDAL_AccountXOrganization As jDeereDataModel_AccountXOrganization In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try

                                If curjDeereDataModelDAL_AccountXOrganization.IDAccount = 0 OrElse
                                   curjDeereDataModelDAL_AccountXOrganization.IDOrganization = 0 Then
                                    Throw New Exception("Chiamata non valida, specificare idAccount e IDOrganization")
                                End If
                                GiasContext.jDeereDataModel_AccountXOrganization.Add(curjDeereDataModelDAL_AccountXOrganization)
                                GiasContext.SaveChanges()
                                success = True

                                Exit For
                            Catch ex As Exception
                                Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                            End Try
                        Next
                        ' Al primo errore evito di continuare le modifiche
                        If Not success Then
                            MessaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                            Exit For
                        End If
                    Next

                    If success Then
                        For Each listFattVar As jDeereDataModel_AccountXOrganization In EFArrayToUpdate
                            GiasContext.jDeereDataModel_AccountXOrganization.Attach(listFattVar)
                            GiasContext.Entry(listFattVar).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listFattVar As jDeereDataModel_AccountXOrganization In EFArrayToDelete
                            GiasContext.jDeereDataModel_AccountXOrganization.Attach(listFattVar)
                            GiasContext.jDeereDataModel_AccountXOrganization.Remove(listFattVar)
                            GiasContext.SaveChanges()
                        Next

                        ' COMMIT Effettivo
                        scope.Complete()
                    End If

                End Using
            End Using


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

        End Try

        Return MessaggioErrore

    End Function



End Class
