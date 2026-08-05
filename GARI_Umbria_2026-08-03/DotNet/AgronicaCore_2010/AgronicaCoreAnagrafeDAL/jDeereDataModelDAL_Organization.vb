Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class jDeereDataModelDAL_Organization_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function LeggiOrganizzazioni(
        ByVal username As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine("  Select  ")
            Stb.AppendLine("     o.* ")
            Stb.AppendLine(" From jDeereDataModel_AccountXOrganization ag ")
            Stb.AppendLine("     inner Join jDeereDataModel_Organization o ")
            Stb.AppendLine("         On ag.IDOrganization = o.ID ")
            Stb.AppendLine("     inner Join jDeereDataModel_Account a ")
            Stb.AppendLine("         On ag.IDAccount = a.ID ")
            Stb.AppendLine(" where a.UsernaName = '" & Agro_SQL_SaveText(username) & "'")

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Test(
                ByVal COD As Integer,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim TestataElem As jDeereDataModel_Organization = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Organization_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Organization
            Where m.ID = COD
            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(TestataElem)


    End Function


    Public Function TestGUID(
                ByVal guid As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim TestataElem As jDeereDataModel_Organization = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Organization_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Organization
            Where m.guid = guid
            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(TestataElem)


    End Function

    Public Function Leggi(
                ByVal ID As Integer,
                ByVal IDTestataGriglia As Integer,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As jDeereDataModel_Organization

        Dim TestataElem As jDeereDataModel_Organization = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Organization_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Organization
            Where m.ID = ID
            Select m).FirstOrDefault()

        End Using

        Return TestataElem


    End Function

    Public Function LeggiIDViaGUID(
            ByVal GUID As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Integer

        Dim TestataElem As jDeereDataModel_Organization = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Organization_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Organization
            Where m.guid = GUID
            Select m).FirstOrDefault()

        End Using

        If TestataElem IsNot Nothing Then
            Return TestataElem.ID
        Else
            Return -1
        End If



    End Function


    Public Function LeggiViaGUID(
            ByVal GUID As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As jDeereDataModel_Organization

        Dim TestataElem As jDeereDataModel_Organization = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Organization_R.LeggiViaGUID()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Organization
            Where m.guid = GUID
            Select m).FirstOrDefault()

        End Using


        Return TestataElem



    End Function

End Class


Public Class jDeereDataModelDAL_Organization_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Aggiorna_jDeereDataModel_Organization(
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AgronicaCoreMetaschemaDAL.Aggiorna_jDeereDataModel_Organization()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each curjDeereDataModel_Organization As jDeereDataModel_Organization In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try

                                'Richiedo un nuovo id sequenza
                                If curjDeereDataModel_Organization.ID Then
                                    idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "jDeereDataModel_Organization", 0, 2000000000, objParametri)

                                    curjDeereDataModel_Organization.ID = idSeq
                                End If

                                GiasContext.jDeereDataModel_Organization.Add(curjDeereDataModel_Organization)
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
                        For Each listFattVar As jDeereDataModel_Organization In EFArrayToUpdate
                            GiasContext.jDeereDataModel_Organization.Attach(listFattVar)
                            GiasContext.Entry(listFattVar).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listFattVar As jDeereDataModel_Organization In EFArrayToDelete
                            GiasContext.jDeereDataModel_Organization.Attach(listFattVar)
                            GiasContext.jDeereDataModel_Organization.Remove(listFattVar)
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