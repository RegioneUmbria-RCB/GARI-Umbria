Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Public Class jDeereDataModelDAL_FieldOperationMeasurement_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Test(
                ByVal COD As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim TestataElem As jDeereDataModel_FieldOperationMeasurement = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModelDAL_FieldOperationMeasurement_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_FieldOperationMeasurement
            Where m.ID = COD
            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(TestataElem)


    End Function

    Public Function Leggi(
                ByVal ID As Integer,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As jDeereDataModel_FieldOperationMeasurement

        Dim TestataElem As jDeereDataModel_FieldOperationMeasurement = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModelDAL_FieldOperationMeasurement_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_FieldOperationMeasurement
            Where m.ID = ID
            Select m).FirstOrDefault()

        End Using

        Return TestataElem

    End Function

    Public Function TestMeasureXFieldOP(
                ByVal FieldOpID As Integer,
                ByVal MeasureName As String,
                ByVal MeasureCateg As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim TestataElem As jDeereDataModel_FieldOperationMeasurement = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModelDAL_FieldOperationMeasurement_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_FieldOperationMeasurement
            Where m.FieldOperationID = FieldOpID And m.measurementName = MeasureName And m.measurementCategory = MeasureCateg
            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(TestataElem)

    End Function

End Class

Public Class jDeereDataModelDAL_FieldOperationMeasurement_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Aggiorna_jDeereDataModel_FieldOperationMeasurement(
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperationMeasurement_W.Aggiorna_jDeereDataModel_FieldOperationMeasurement()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each curjDeereDataModel_FieldOperationMeasurement As jDeereDataModel_FieldOperationMeasurement In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try
                                If curjDeereDataModel_FieldOperationMeasurement.ID = 0 Then
                                    'Richiedo un nuovo id sequenza
                                    idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "jDeereDataModel_FieldOperation", 0, 2000000000, objParametri)

                                    curjDeereDataModel_FieldOperationMeasurement.ID = idSeq
                                End If
                                GiasContext.jDeereDataModel_FieldOperationMeasurement.Add(curjDeereDataModel_FieldOperationMeasurement)
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
                        For Each listFattVar As jDeereDataModel_FieldOperationMeasurement In EFArrayToUpdate
                            GiasContext.jDeereDataModel_FieldOperationMeasurement.Attach(listFattVar)
                            GiasContext.Entry(listFattVar).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listFattVar As jDeereDataModel_FieldOperationMeasurement In EFArrayToDelete
                            GiasContext.jDeereDataModel_FieldOperationMeasurement.Attach(listFattVar)
                            GiasContext.jDeereDataModel_FieldOperationMeasurement.Remove(listFattVar)
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