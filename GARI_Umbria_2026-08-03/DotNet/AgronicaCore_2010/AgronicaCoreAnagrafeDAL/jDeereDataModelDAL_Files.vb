Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class jDeereDataModelDAL_Files_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal ID As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As jDeereDataModel_Files

        Dim nomeRoutine As String = "jDeereDataModelDAL_Files_R.Leggi()"

        Dim testataElem As jDeereDataModel_Files = Nothing

        Dim gefutils As New Gias_EF_Utility

        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

            testataElem = (From m In giasContext.jDeereDataModel_Files
                            Where m.ID = ID
                            Select m).FirstOrDefault()

        End Using

        Return testataElem

    End Function

    Public Function TestGUID(ByVal guid As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim nomeRoutine As String = "jDeereDataModelDAL_Files_R.TestGUID()"

        Dim testataElem As jDeereDataModel_Files = Nothing

        Dim gefutils As New Gias_EF_Utility

        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

            testataElem = (From m In giasContext.jDeereDataModel_Files
                            Where m.guid = guid
                            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(testataElem)

    End Function

    Public Function Test(ByVal ID As Integer,
                         ByRef objParametri As AgronicaCoreParametri
                         ) As Boolean

        Dim nomeRoutine As String = "jDeereDataModel_Field_R.Test()"

        Dim testataElem As jDeereDataModel_Files = Nothing

        Dim gefutils As New Gias_EF_Utility

        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

            testataElem = (From m In giasContext.jDeereDataModel_Files
                            Where m.ID = ID
                            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(testataElem)

    End Function

    Public Function LeggiIDViaGUID(ByVal guid As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Integer

        Dim nomeRoutine As String = "jDeereDataModelDAL_Files_R.LeggiIDViaGUID()"

        Dim testataElem As jDeereDataModel_Files = Nothing

        Dim gefutils As New Gias_EF_Utility

        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

            testataElem = (From m In giasContext.jDeereDataModel_Files
                            Where m.guid = guid
                            Select m).FirstOrDefault()

        End Using

        If testataElem Is Nothing Then
            Return -1
        Else
            Return testataElem.ID
        End If

    End Function

End Class

Public Class jDeereDataModelDAL_Files_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Aggiorna_jDeereDataModel_Files(ByVal EFArrayToInsert As ArrayList,
                                                   ByVal EFArrayToUpdate As ArrayList,
                                                   ByVal EFArrayToDelete As ArrayList,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Files_W.Aggiorna_jDeereDataModel_Files()"

        Dim messaggioErrore As String = ""

        Dim objSequenze As New Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(efConnString)

                    Dim idSeq As Integer = 0

                    For Each curjDeereDataModel_Files As jDeereDataModel_Files In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try

                                If curjDeereDataModel_Files.ID = 0 Then
                                    idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext,
                                            "jDeereDataModel_Files", 0, 2000000000, objParametri)

                                    curjDeereDataModel_Files.ID = idSeq
                                End If
                                GiasContext.jDeereDataModel_Files.Add(curjDeereDataModel_Files)
                                GiasContext.SaveChanges()
                                success = True

                                Exit For
                            Catch ex As Exception
                                Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                            End Try
                        Next
                        ' Al primo errore evito di continuare le modifiche
                        If Not success Then
                            messaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                            Exit For
                        End If
                    Next



                    If success Then
                        For Each listFattVar As jDeereDataModel_Files In EFArrayToUpdate
                            GiasContext.jDeereDataModel_Files.Attach(listFattVar)
                            GiasContext.Entry(listFattVar).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listFattVar As jDeereDataModel_Files In EFArrayToDelete
                            GiasContext.jDeereDataModel_Files.Attach(listFattVar)
                            GiasContext.jDeereDataModel_Files.Remove(listFattVar)
                            GiasContext.SaveChanges()
                        Next

                        ' COMMIT Effettivo
                        scope.Complete()
                    End If

                End Using
            End Using


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)
        End Try

        Return messaggioErrore

    End Function

End Class
