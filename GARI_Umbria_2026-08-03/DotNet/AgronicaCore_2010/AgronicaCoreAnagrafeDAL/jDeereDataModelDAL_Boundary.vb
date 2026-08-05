Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class jDeereDataModelDAL_Boundary_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiViaJsonSQL(
            ByVal xFiltroAggiuntivo As String,
            ByRef objParametri_Server As AgronicaCoreParametri
    ) As String

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafe_DAL.jDeereDataModelDAL_Boundary_R.LeggiDaJsonPath()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable


        Try

            Stb.AppendLine(" Declare @JSON nvarchar(max) ")
            Stb.AppendLine(" Set @json = ( ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" Select ")
            Stb.AppendLine("       ID ")
            Stb.AppendLine("     , Name    ")
            Stb.AppendLine("     , JSON_QUERY(boundaryData, '$.area') as area ")
            Stb.AppendLine("     , JSON_QUERY(boundaryData, '$.workableArea') as workableArea         ")
            Stb.AppendLine("     , sourceType  ")
            Stb.AppendLine("     , active  ")
            Stb.AppendLine("     , irrigated   ")
            Stb.AppendLine("     , type    ")
            Stb.AppendLine("     , passable     ")
            Stb.AppendLine("     , JSON_QUERY(boundaryData, '$.multipolygons') as multipolygons ")
            Stb.AppendLine("     , JSON_QUERY(boundaryData, '$.extent') as extent ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" From [dbo].[jDeereDataModel_Boundary] bb")


            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" WHERE " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri_Server))
            End If

            Stb.AppendLine(" For json path ")
            Stb.AppendLine(" ) ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" Select @json")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        If IsDBNull(DT.Rows(0)(0)) Then
            Return "[]"
        End If

        Return DT.Rows(0)(0)

    End Function

    Public Function LeggiCreateReqViaJsonSQL(
            ByVal ID As Integer,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As String

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafe_DAL.jDeereDataModelDAL_Boundary_R.LeggiCreateReqViaJsonSQL()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable


        Try
            Stb.Length = 0

            Stb.AppendLine(" Declare @JSON nvarchar(max) ")
            Stb.AppendLine(" Set @json = ( ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" Select ")
            Stb.AppendLine("     Name    ")
            Stb.AppendLine("     , 'External' as sourceType ")
            Stb.AppendLine("     , JSON_QUERY(boundaryData, '$.multipolygons') as multipolygons ")
            Stb.AppendLine("     , active  ")
            Stb.AppendLine("     , irrigated   ")
            Stb.AppendLine("     , guid   ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" From [dbo].[jDeereDataModel_Boundary] bb")
            Stb.AppendLine(" WHERE ID=" & ID.ToString() + " ")
            Stb.AppendLine(" For json path ")
            Stb.AppendLine(" ) ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" Select @json")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        If IsDBNull(DT.Rows(0)(0)) Then
            Return "[]"
        End If

        Return DT.Rows(0)(0)

    End Function

    Public Function Test(
                ByVal COD As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim TestataElem As jDeereDataModel_Boundary = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Boundary_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Boundary
            Where m.ID = COD
            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(TestataElem)


    End Function

    Public Function Leggi(
                ByVal COD As String,
                ByVal IDTestataGriglia As Integer,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As jDeereDataModel_Boundary

        Dim TestataElem As jDeereDataModel_Boundary = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Boundary_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Boundary
            Where m.ID = COD
            Select m).FirstOrDefault()

        End Using

        Return TestataElem

    End Function

    Public Function TestGUID(
               ByVal guid As String,
               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
       ) As Boolean

        Dim TestataElem As jDeereDataModel_Boundary = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Boundary_R.TestGUID()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Boundary
            Where m.guid = guid
            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(TestataElem)


    End Function

    Public Function LeggiIDViaGUID(
                ByVal guid As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Integer

        Dim TestataElem As jDeereDataModel_Boundary = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Boundary_R.LeggiIDViaGUID()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Boundary
            Where m.guid = guid
            Select m).FirstOrDefault()

        End Using

        If TestataElem Is Nothing Then
            Return -1
        Else
            Return TestataElem.ID
        End If


    End Function

    Public Function LeggiElencoBoundariesDaInviareAJDeere(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Optional ByVal fldId As Integer = -1) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiElencoBoundariesDaInviareAJDeere()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine("  Select ")
            Stb.AppendLine("        a.ID  ")
            Stb.AppendLine("      , a.Name  ")
            Stb.AppendLine("      , b.IDField  ")
            Stb.AppendLine("      , a.guid as GUIDBoundary  ")
            Stb.AppendLine("      , c.guid as GUIDField  ")
            Stb.AppendLine("      , a.boundaryData  ")
            Stb.AppendLine("      , a.area_unit  ")
            Stb.AppendLine("  From jDeereDataModel_Boundary a ")
            Stb.AppendLine("  inner join jDeereDataModel_FieldXBoundary b ")
            Stb.AppendLine("  on (a.ID=b.IDBoundary) ")
            Stb.AppendLine("  inner join jDeereDataModel_Field c ")
            Stb.AppendLine("  on (b.IDField=c.ID) ")
            Stb.AppendLine("  Where ")
            If fldId = -1 Then
                Stb.AppendLine("  a.guid Like 'AGR-%' ")
            Else
                Stb.AppendLine("  b.IDField=" + fldId.ToString() + " ")
            End If
            Stb.AppendLine("  group by a.ID,a.Name,b.IDField, a.guid,c.guid, a.boundaryData, a.area_unit ")
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


Public Class jDeereDataModelDAL_Boundary_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Aggiorna_jDeereDataModel_Boundary(
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AgronicaCoreMetaschemaDAL.Aggiorna_jDeereDataModel_Boundary()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each curjDeereDataModel_Boundary As jDeereDataModel_Boundary In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try
                                If curjDeereDataModel_Boundary.ID = 0 Then
                                    'Richiedo un nuovo id sequenza
                                    idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "jDeereDataModel_Boundary", 0, 2000000000, objParametri)

                                    curjDeereDataModel_Boundary.ID = idSeq
                                End If
                                GiasContext.jDeereDataModel_Boundary.Add(curjDeereDataModel_Boundary)
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
                        For Each listFattVar As jDeereDataModel_Boundary In EFArrayToUpdate
                            GiasContext.jDeereDataModel_Boundary.Attach(listFattVar)
                            GiasContext.Entry(listFattVar).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listFattVar As jDeereDataModel_Boundary In EFArrayToDelete
                            GiasContext.jDeereDataModel_Boundary.Attach(listFattVar)
                            GiasContext.jDeereDataModel_Boundary.Remove(listFattVar)
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
