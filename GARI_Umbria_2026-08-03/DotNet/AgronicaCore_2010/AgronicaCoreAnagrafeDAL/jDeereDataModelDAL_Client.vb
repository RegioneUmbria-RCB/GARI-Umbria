Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class jDeereDataModelDAL_Client_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
                ByVal COD As String,
                ByVal IDTestataGriglia As Integer,
                ByRef objParametri As AgronicaCoreParametri
        ) As jDeereDataModel_Client

        Dim TestataElem As jDeereDataModel_Client = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Client_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Client
            Where m.ID = COD
            Select m).FirstOrDefault()

        End Using

        Return TestataElem


    End Function

    Public Function TestGUID(
               ByVal guid As String,
               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
       ) As Boolean

        Dim TestataElem As jDeereDataModel_Client = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Client_R.TestGUID()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Client
            Where m.guid = guid
            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(TestataElem)


    End Function

    Public Function LeggiIDViaGUID(
                ByVal guid As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Integer

        Dim TestataElem As jDeereDataModel_Client = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Client_R.LeggiIDViaGUID()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Client
            Where m.guid = guid
            Select m).FirstOrDefault()

        End Using

        If TestataElem Is Nothing Then
            Return -1
        Else
            Return TestataElem.ID
        End If
    End Function

    Public Function LeggiGUIDViaName(
            ByVal name As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim TestataElem As jDeereDataModel_Client = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Client_R.LeggiGUIDViaName()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Client
            Where m.Name = name
            Select m).FirstOrDefault()

        End Using

        If TestataElem Is Nothing Then
            Return Nothing
        Else
            Return TestataElem.guid
        End If

    End Function

    Public Function LeggiDettaglioClientByID(
        ByVal ID As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiDettaglioClientByID()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            Stb.Length = 0

            Stb.AppendLine("  Select ")
            Stb.AppendLine("        Name  ")
            Stb.AppendLine("      , guid  ")
            Stb.AppendLine("  From jDeereDataModel_Client ")
            Stb.AppendLine("  Where ID=" + ID.ToString() + "")
            Stb.AppendLine("  ")

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


Public Class jDeereDataModelDAL_Client_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Aggiorna_jDeereDataModel_Client(
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AgronicaCoreMetaschemaDAL.Aggiorna_jDeereDataModel_Client()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each curjDeereDataModel_Client As jDeereDataModel_Client In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try

                                If curjDeereDataModel_Client.ID = 0 Then
                                    idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                            "jDeereDataModel_Client", 0, 2000000000, objParametri)

                                    curjDeereDataModel_Client.ID = idSeq
                                End If
                                GiasContext.jDeereDataModel_Client.Add(curjDeereDataModel_Client)
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
                        For Each listFattVar As jDeereDataModel_Client In EFArrayToUpdate
                            GiasContext.jDeereDataModel_Client.Attach(listFattVar)
                            GiasContext.Entry(listFattVar).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listFattVar As jDeereDataModel_Client In EFArrayToDelete
                            GiasContext.jDeereDataModel_Client.Attach(listFattVar)
                            GiasContext.jDeereDataModel_Client.Remove(listFattVar)
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