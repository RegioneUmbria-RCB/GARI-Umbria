Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class jDeereDataModelDAL_FieldXBoundary_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Test(
                ByVal IdField As String,
                ByVal IdBoundary As Integer,
                ByRef objParametri As AgronicaCoreParametri
        ) As Boolean

        Dim TestataElem As jDeereDataModel_FieldXBoundary = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModelDAL_FieldXBoundary_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_FieldXBoundary
            Where m.IDField = IdField AndAlso
                m.jDeereDataModel_Boundary.ID = IdBoundary
            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(TestataElem)

    End Function

    Public Function Leggi(
                ByVal IdField As Integer,
                ByVal IdBoundary As Integer,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As jDeereDataModel_FieldXBoundary

        Dim TestataElem As jDeereDataModel_FieldXBoundary = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModelDAL_FieldXBoundary.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_FieldXBoundary
            Where m.IDField = IdField AndAlso
                m.jDeereDataModel_Boundary.ID = IdBoundary
            Select m).FirstOrDefault()

        End Using

        Return TestataElem

    End Function
End Class

Public Class jDeereDataModelDAL_FieldXBoundary_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Aggiorna_jDeereDataModelDAL_FieldXBoundary(
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Aggiorna_jDeereDataModelDAL_FieldXBoundary()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each curjDeereDataModel_FieldXBoundary As jDeereDataModel_FieldXBoundary In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try

                                If curjDeereDataModel_FieldXBoundary.IDField = 0 OrElse
                                   curjDeereDataModel_FieldXBoundary.IDBoundary = 0 Then
                                    Throw New Exception("Chiamata non valida, specificare IDField e IDBoundary")
                                End If
                                GiasContext.jDeereDataModel_FieldXBoundary.Add(curjDeereDataModel_FieldXBoundary)
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
                        For Each listFattVar As jDeereDataModel_FieldXBoundary In EFArrayToUpdate
                            GiasContext.jDeereDataModel_FieldXBoundary.Attach(listFattVar)
                            GiasContext.Entry(listFattVar).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listFattVar As jDeereDataModel_FieldXBoundary In EFArrayToDelete
                            GiasContext.jDeereDataModel_FieldXBoundary.Attach(listFattVar)
                            GiasContext.jDeereDataModel_FieldXBoundary.Remove(listFattVar)
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
