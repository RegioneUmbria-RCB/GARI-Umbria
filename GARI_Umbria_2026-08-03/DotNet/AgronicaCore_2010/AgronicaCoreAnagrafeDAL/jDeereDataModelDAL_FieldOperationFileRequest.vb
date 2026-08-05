Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json

Public Class jDeereDataModelDAL_FieldOperationFileRequest_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Test(
                ByVal COD As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim TestataElem As jDeereDataModel_FieldOperationFileRequest = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModelDAL_FieldOperationFileRequest_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_FieldOperationFileRequest
            Where m.ID = COD
            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(TestataElem)
    End Function

    Public Function Leggi(
                ByVal ID As Integer,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As jDeereDataModel_FieldOperationFileRequest

        Dim TestataElem As jDeereDataModel_FieldOperationFileRequest = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModelDAL_FieldOperationMeasurement_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_FieldOperationFileRequest
            Where m.ID = ID
            Select m).FirstOrDefault()

        End Using

        Return TestataElem

    End Function

    Public Function LeggiDaFieldOperation(
                ByVal FieldOperationID As Integer,
                ByVal FieldOperationGUID As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModelDAL_FieldOperationMeasurement_R.LeggiDaFieldOperation()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" select ")
            Stb.AppendLine(" 	ID, ")
            Stb.AppendLine(" 	FieldOperationID, ")
            Stb.AppendLine(" 	FieldOperationGUID, ")
            Stb.AppendLine(" 	Allegati_Documenti_Cod, ")
            Stb.AppendLine(" 	OAuth_Token, ")
            Stb.AppendLine(" 	RequestState, ")
            Stb.AppendLine(" 	Request_Parameters, ")
            Stb.AppendLine(" 	Esito ")
            Stb.AppendLine(" from ")
            Stb.AppendLine(" 	jDeereDataModel_FieldOperationFileRequest ")
            Stb.AppendLine(" where ")
            Stb.AppendLine(" 	FieldOperationID=" + Agro_SQL_SaveNum(FieldOperationID))
            Stb.AppendLine(" 	and FieldOperationGUID='" + Agro_SQL_SaveText(FieldOperationGUID) + "'")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiRichieste(
                ByRef ID As Integer,
                ByRef FlagDaElaborare As Boolean,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModelDAL_FieldOperationMeasurement_R.LeggiRichiesteDaElaborare()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" select ")
            Stb.AppendLine(" 	ID, ")
            Stb.AppendLine(" 	FieldOperationID, ")
            Stb.AppendLine(" 	FieldOperationGUID, ")
            Stb.AppendLine(" 	OAuth_Token, ")
            Stb.AppendLine(" 	Request_Parameters ")
            Stb.AppendLine(" from ")
            Stb.AppendLine(" 	jDeereDataModel_FieldOperationFileRequest ")
            Stb.AppendLine(" where ")
            If FlagDaElaborare = True Then
                Stb.AppendLine(" 	RequestState=0 ")
            Else
                Stb.AppendLine(" 	RequestState=1 ")
            End If
            If ID <> 0 Then
                Stb.AppendLine(" 	and ID=" + Agro_SQL_SaveNum(ID))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function
End Class

Public Class jDeereDataModelDAL_FieldOperationFileRequest_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Aggiorna_jDeereDataModel_FieldOperationFileRequest(
               ByVal EFArrayToInsert As ArrayList,
               ByVal EFArrayToUpdate As ArrayList,
               ByVal EFArrayToDelete As ArrayList,
               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
               ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperationFileRequest_W.Aggiorna_jDeereDataModel_FieldOperationFileRequest()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each curjDeereDataModel_FieldOperationFileRequest As jDeereDataModel_FieldOperationFileRequest In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try
                                If curjDeereDataModel_FieldOperationFileRequest.ID = 0 Then
                                    'Richiedo un nuovo id sequenza
                                    idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "jDeereDataModel_FieldOperationFileRequest", 0, 2000000000, objParametri)

                                    curjDeereDataModel_FieldOperationFileRequest.ID = idSeq
                                End If
                                GiasContext.jDeereDataModel_FieldOperationFileRequest.Add(curjDeereDataModel_FieldOperationFileRequest)
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
                        For Each listFattVar As jDeereDataModel_FieldOperationFileRequest In EFArrayToUpdate
                            GiasContext.jDeereDataModel_FieldOperationFileRequest.Attach(listFattVar)
                            GiasContext.Entry(listFattVar).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listFattVar As jDeereDataModel_FieldOperationFileRequest In EFArrayToDelete
                            GiasContext.jDeereDataModel_FieldOperationFileRequest.Attach(listFattVar)
                            GiasContext.jDeereDataModel_FieldOperationFileRequest.Remove(listFattVar)
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
