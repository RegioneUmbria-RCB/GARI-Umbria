Imports System.Data.Entity
Imports System.Text
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class jDeereDataModelDAL_Field_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiViaJsonSQL(
            ByVal LeggiClients As Boolean,
            ByVal leggifarms As Boolean,
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
            Stb.AppendLine("       id ")
            Stb.AppendLine("     , name ")
            Stb.AppendLine("     , archived ")
            Stb.AppendLine("     , guid ")

            If leggifarms Then
                Stb.AppendLine("     , ( ")
                Stb.AppendLine("         Select ")
                Stb.AppendLine("               id ")
                Stb.AppendLine("             , name ")
                Stb.AppendLine("             , guid ")
                Stb.AppendLine("             From [dbo].[jDeereDataModel_Farm] f ")
                Stb.AppendLine("             Where f.ID = ff.FarmID ")
                Stb.AppendLine("             For json path ")
                Stb.AppendLine("     ) farms ")
            End If 'farms

            If LeggiClients Then
                Stb.AppendLine("     , ( ")
                Stb.AppendLine("         Select ")
                Stb.AppendLine("               id ")
                Stb.AppendLine("             , name ")
                Stb.AppendLine("             , guid ")
                Stb.AppendLine("             From [dbo].[jDeereDataModel_Client] c ")
                Stb.AppendLine("             Where c.ID = ff.ClientID ")
                Stb.AppendLine("             For json path ")
                Stb.AppendLine("     ) clients ")
            End If 'clients

            Stb.AppendLine(" From [dbo].[jDeereDataModel_Field] ff ")
            Stb.AppendLine(" ")

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" WHERE " & xFiltroAggiuntivo)
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

    Public Function LeggiDettaglioFieldByID(
        ByVal ID As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiDettaglioFieldByID()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine("  Select ")
            Stb.AppendLine("        Name  ")
            Stb.AppendLine("      , archived  ")
            Stb.AppendLine("      , FarmID  ")
            Stb.AppendLine("      , ClientID  ")
            Stb.AppendLine("      , guid  ")
            Stb.AppendLine("  From jDeereDataModel_Field ")
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

    Public Function LeggiFieldsDaInviareAJDeere(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Optional listaKey As List(Of Integer) = Nothing) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiDettaglioFieldByID()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim listKeyStr As String = ""
        If listaKey IsNot Nothing Then
            listKeyStr = String.Join(",", listaKey)
        End If

        Try

            Stb.Length = 0

            Stb.AppendLine("  Select ")
            Stb.AppendLine("        ID  ")
            Stb.AppendLine("      ,  Name  ")
            Stb.AppendLine("      , archived  ")
            Stb.AppendLine("      , FarmID  ")
            Stb.AppendLine("      , ClientID  ")
            Stb.AppendLine("      , guid  ")
            Stb.AppendLine("  From jDeereDataModel_Field a")
            Stb.AppendLine("  Where 1=1 ")
            If listKeyStr <> "" Then
                Stb.AppendLine("  and ID in (" + Agro_SQL_Save_Clausola_IN(listKeyStr) + ") ")
            Else
                Stb.AppendLine("  and a.guid like 'AGR-%' ")
            End If
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

    Public Function LeggiConSincronizzazioneAppezzamenti(
        ByVal OrganizationID As Integer,
        ByVal piva As String,
        ByVal sa_Cod As Integer,
        ByVal appezza As Integer,
        ByVal xFiltroAggiuntivoJohnDeere As String,
        ByVal xFiltroAggiuntivoGias As String,
        ByVal xFiltroAggiuntivoJohnDeere2 As String,
        ByVal xOrderByJohnDeere2 As String,
        ByVal xOrderByJohnDeere As String,
        ByVal xOrderByGias As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine("  Select ")
            Stb.AppendLine("        'SoloJD' as Tipo  ")
            Stb.AppendLine("      ,  '-0-' + cast(ff.id as varchar(50)) as chiave  ")
            Stb.AppendLine("      , '' as piva  ")
            Stb.AppendLine("      , 0 as sa_cod  ")
            Stb.AppendLine("      , 0 as appezza  ")
            Stb.AppendLine("      , '' as app_nome  ")
            Stb.AppendLine("      , 0 as sup_app  ")
            Stb.AppendLine("      , '' as RiferimentoAlfanumerico  ")
            Stb.AppendLine("      , ff.Name as Val_Cod  ")
            Stb.AppendLine("      , isnull(cc.Name, '') as BoundaryName  ")
            Stb.AppendLine("      , fr.Name as FarmName  ")
            Stb.AppendLine("      , fr.ID as FarmID  ")
            Stb.AppendLine("      , cl.Name as ClientName  ")
            Stb.AppendLine("      , cl.ID as ClientID  ")
            Stb.AppendLine("      , case when cc.IDBoundary is null then 0 else 1 end as ConfiniPresenti")
            Stb.AppendLine("      , ff.data_modifica  ")
            Stb.AppendLine("  From jDeereDataModel_Field ff ")
            Stb.AppendLine("      inner join jDeereDataModel_Farm fr ")
            Stb.AppendLine("          on ff.FarmID = fr.ID ")
            Stb.AppendLine("      inner join jDeereDataModel_Client cl ")
            Stb.AppendLine("          on ff.ClientID = cl.ID ")

            LeggiConSincronizzazioneAppezzamentiConfiniGetQuery(Stb)

            Stb.AppendLine("  where ff.organizationID = " & Agro_SQL_SaveNum(OrganizationID) & " and Not exists (  ")
            Stb.AppendLine("      select 1  ")
            Stb.AppendLine("      From jDeereDataModel_EntitaGIAS JDG  ")
            Stb.AppendLine("      Where JDG.TipoJD = " & enum_TipoEntitaJohnDeere.Field)
            Stb.AppendLine("      And JDG.IDJD = ff.ID  ")
            Stb.AppendLine("   ) ")
            Stb.AppendLine("  ")

            If xFiltroAggiuntivoJohnDeere <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivoJohnDeere))
            End If


            If xOrderByJohnDeere <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderByJohnDeere))
            End If

            Stb.AppendLine("  UNION ALL ")

            Stb.AppendLine("  Select ")
            Stb.AppendLine("        'Sincro' as Tipo  ")
            Stb.AppendLine("     ,  isnull(a.piva + '-' + cast(a.sa_cod as varchar(50)) + '-' + cast(a.Appezza as varchar(50)), '-0-' + cast(ff.id as varchar(50))) as chiave ")
            Stb.AppendLine("     , isnull(a.piva, '') as piva ")
            Stb.AppendLine("     , isnull(a.sa_cod, 0) as sa_cod ")
            Stb.AppendLine("     , isnull(a.appezza, 0) as appezza ")
            Stb.AppendLine("     , isnull(app_nome, '') as app_nome ")
            Stb.AppendLine("     , isnull(sup_App, 0) as sup_app ")
            Stb.AppendLine("     , isNull(rif.val_cod, '') as RiferimentoAlfanumerico ")
            Stb.AppendLine("     , ff.Name as Val_Cod ")
            Stb.AppendLine("      , isnull(cc.Name, '') as BoundaryName  ")
            Stb.AppendLine("     , fr.Name as FarmName  ")
            Stb.AppendLine("     , fr.ID as FarmID  ")
            Stb.AppendLine("     , cl.Name as ClientName  ")
            Stb.AppendLine("     , cl.ID as ClientID  ")
            Stb.AppendLine("     , case when cc.IDBoundary is null then 0 else 1 end as ConfiniPresenti")
            Stb.AppendLine("     , ff.data_modifica ")
            Stb.AppendLine(" From jDeereDataModel_Field ff ")
            Stb.AppendLine("      inner join jDeereDataModel_Farm fr ")
            Stb.AppendLine("          on ff.FarmID = fr.ID ")
            Stb.AppendLine("      inner join jDeereDataModel_Client cl ")
            Stb.AppendLine("          on ff.ClientID = cl.ID ")
            LeggiConSincronizzazioneAppezzamentiConfiniGetQuery(Stb)
            Stb.AppendLine("     inner join ( ")
            Stb.AppendLine("     select * ")
            Stb.AppendLine("     from jDeereDataModel_EntitaGIAS JDG ")
            Stb.AppendLine("     where JDG.piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If sa_Cod <> 0 Then
                Stb.AppendLine(" and JDG.sa_cod = " & Agro_SQL_SaveNum(sa_Cod) & " ")
            End If

            If appezza <> 0 Then
                Stb.AppendLine(" and JDG.appezza = " & Agro_SQL_SaveNum(appezza) & " ")
            End If

            Stb.AppendLine("     ) JDG ")
            Stb.AppendLine("        on JDG.IDJD = ff.ID ")
            Stb.AppendLine("        and JDG.TipoJD =  " & enum_TipoEntitaJohnDeere.Field)

            Stb.AppendLine("     Left Join Appezzamento a ")
            Stb.AppendLine("         On a.piva = JDG.piva ")
            Stb.AppendLine("         And a.SA_COD = JDG.sa_cod ")
            Stb.AppendLine("         And a.APPEZZA = JDG.appezza ")
            Stb.AppendLine("     Left Join Appezzamento_Codici rif ")
            Stb.AppendLine("         On a.piva = rif.piva ")
            Stb.AppendLine("         And a.SA_COD = rif.sa_cod ")
            Stb.AppendLine("         And a.APPEZZA = rif.appezza ")
            Stb.AppendLine("         And rif.id_cod = 1104")


            If xFiltroAggiuntivoJohnDeere2 <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivoJohnDeere2))
            End If


            If xOrderByJohnDeere2 <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderByJohnDeere2))
            End If

            Stb.AppendLine("  UNION ALL ")

            Stb.AppendLine("  Select ")
            Stb.AppendLine("        'SoloGias' as Tipo  ")
            Stb.AppendLine("     ,  a.piva + '-' + cast(a.sa_cod as varchar(50)) + '-' + cast(a.Appezza as varchar(50)) as chiave ")
            Stb.AppendLine("     , a.piva ")
            Stb.AppendLine("     , a.sa_cod ")
            Stb.AppendLine("     , a.appezza ")
            Stb.AppendLine("     , app_nome ")
            Stb.AppendLine("     , sup_App ")
            Stb.AppendLine("     , isNull(rif.val_cod, '') as RiferimentoAlfanumerico ")
            Stb.AppendLine("     , '' as Val_Cod ")
            Stb.AppendLine("     , '' as BoundaryName ")
            Stb.AppendLine("     , '' as FarmName  ")
            Stb.AppendLine("     , 0 as FarmID  ")
            Stb.AppendLine("     , '' as ClientName  ")
            Stb.AppendLine("     , 0 as ClientID  ")
            Stb.AppendLine("     , case when e.Entita_Cod is null then 0 else 1 end as ConfiniPresenti  ")
            Stb.AppendLine("     , a.data_modifica ")
            Stb.AppendLine(" From Appezzamento a ")
            Stb.AppendLine("      Left Join GIS_Entita e ")
            Stb.AppendLine("           On a.piva = e.piva  ")
            Stb.AppendLine("          And a.SA_COD = e.sa_cod  ")
            Stb.AppendLine("          And a.APPEZZA = e.appezza  ")
            Stb.AppendLine("          And e.TipoEntita_cod = 1")

            Stb.AppendLine("     Left Join Appezzamento_Codici rif ")
            Stb.AppendLine("         On a.piva = rif.piva ")
            Stb.AppendLine("         And a.SA_COD = rif.sa_cod ")
            Stb.AppendLine("         And a.APPEZZA = rif.appezza ")
            Stb.AppendLine("         And rif.id_cod = 1104")

            Stb.AppendLine("  where Not exists( ")
            Stb.AppendLine("     select 1 ")
            Stb.AppendLine("     From jDeereDataModel_EntitaGIAS JDG ")
            Stb.AppendLine("     Where JDG.piva = a.piva ")
            Stb.AppendLine("     And   JDG.sa_cod = a.SA_COD ")
            Stb.AppendLine("     And   JDG.appezza = a.APPEZZA ")
            Stb.AppendLine("  )")


            Stb.AppendLine("   and a.piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If sa_Cod <> 0 Then
                Stb.AppendLine("   and a.sa_cod = " & Agro_SQL_SaveNum(sa_Cod) & " ")
            End If

            If appezza <> 0 Then
                Stb.AppendLine("   and a.appezza = " & Agro_SQL_SaveNum(appezza) & " ")
            End If


            If xFiltroAggiuntivoGias <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivoGias))
            End If


            If xOrderByGias <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderByGias))
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

    Private Shared Sub LeggiConSincronizzazioneAppezzamentiConfiniGetQuery(Stb As StringBuilder)
        Stb.AppendLine("      Left Join( ")
        Stb.AppendLine("         select c.*, b.Name ")
        Stb.AppendLine("         from ( ")
        Stb.AppendLine("             select max(fb.IDBoundary) As IDBoundary, fb.IDField ")
        Stb.AppendLine("             From [dbo].[jDeereDataModel_FieldXBoundary] fb ")
        Stb.AppendLine("                 inner Join [dbo].[jDeereDataModel_Boundary] b  ")
        Stb.AppendLine("                     On b.id = fb.IDBoundary  ")
        Stb.AppendLine("             where b.active = 1 ")
        Stb.AppendLine("             group by fb.IDField ")
        Stb.AppendLine("           ) c  ")
        Stb.AppendLine("           inner Join [dbo].[jDeereDataModel_Boundary] b  ")
        Stb.AppendLine("               On b.id = c.IDBoundary  ")
        Stb.AppendLine("       ) cc  ")
        Stb.AppendLine("         On cc.IDField = ff.id")
    End Sub

    Public Function TestGUID(
                ByVal guid As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim TestataElem As jDeereDataModel_Field = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Field_R.TestGUID()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Field
            Where m.guid = guid
            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(TestataElem)


    End Function

    Public Function Test(
                ByVal ID As Integer,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim TestataElem As jDeereDataModel_Field = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Field_R.Test()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Field
            Where m.ID = ID
            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(TestataElem)


    End Function

    Public Function LeggiIDViaGUID(
                ByVal guid As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Integer

        Dim TestataElem As jDeereDataModel_Field = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Field_R.LeggiIDViaGUID()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Field
            Where m.guid = guid
            Select m).FirstOrDefault()

        End Using

        If TestataElem Is Nothing Then
            Return -1
        Else
            Return TestataElem.ID
        End If


    End Function

    Public Function LeggiGUIDViaID(
                ByVal ID As Integer,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        Dim TestataElem As jDeereDataModel_Field = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Field_R.LeggiGUIDViaID()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Field
            Where m.ID = ID
            Select m).FirstOrDefault()

        End Using

        If TestataElem Is Nothing Then
            Return -1
        Else
            Return TestataElem.guid
        End If


    End Function

    Public Function Leggi(
                ByVal ID As Integer,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As jDeereDataModel_Field

        Dim TestataElem As jDeereDataModel_Field = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_Field_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_Field
            Where m.ID = ID
            Select m).FirstOrDefault()

        End Using

        Return TestataElem


    End Function

End Class





Public Class jDeereDataModelDAL_Field_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Aggiorna_jDeereDataModel_Field(
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AgronicaCoreMetaschemaDAL.Aggiorna_jDeereDataModel_Field()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each curjDeereDataModel_Fields As jDeereDataModel_Field In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try

                                'Richiedo un nuovo id sequenza
                                If curjDeereDataModel_Fields.ID = 0 Then
                                    idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "jDeereDataModel_Field", 0, 2000000000, objParametri)

                                    curjDeereDataModel_Fields.ID = idSeq
                                End If
                                GiasContext.jDeereDataModel_Field.Add(curjDeereDataModel_Fields)
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
                        For Each listFattVar As jDeereDataModel_Field In EFArrayToUpdate
                            GiasContext.jDeereDataModel_Field.Attach(listFattVar)
                            GiasContext.Entry(listFattVar).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listFattVar As jDeereDataModel_Field In EFArrayToDelete
                            GiasContext.jDeereDataModel_Field.Attach(listFattVar)
                            GiasContext.jDeereDataModel_Field.Remove(listFattVar)
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

