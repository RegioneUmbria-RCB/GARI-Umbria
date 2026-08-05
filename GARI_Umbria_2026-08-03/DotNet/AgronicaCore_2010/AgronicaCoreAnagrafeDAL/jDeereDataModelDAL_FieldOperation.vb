Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class jDeereDataModelDAL_FieldOperation_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Leggi(
            ByVal OrganizationID As Integer,
            ByVal FieldID As Integer,
            ByVal xFiltroAggiuntivoJohnDeere1 As String,
            ByVal xOrderByJohnDeere1 As String,
            ByVal xFiltroAggiuntivoJohnDeere2 As String,
            ByVal xOrderByJohnDeere2 As String,
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

            Stb.AppendLine(" SELECT  " & vbCrLf)
            Stb.AppendLine("     'SoloJD' as Tipo  ")
            Stb.AppendLine("   , '-' + cast (o.ID as varchar(50)) as chiave")
            Stb.AppendLine("   --   Dati JD")
            Stb.AppendLine("   , fieldOperationType")
            Stb.AppendLine("   , startDate")
            Stb.AppendLine("   , endDate")
            Stb.AppendLine("   --   Dati GIAS")
            Stb.AppendLine("   , '' as DescrizioneGias")
            Stb.AppendLine("   --   info ""field"" ed appezzamento")
            Stb.AppendLine("   , case when b.ID is null then 0 else 1 end as ConfiniPresenti")
            Stb.AppendLine("   , f.Name as FieldName")
            Stb.AppendLine("   , f.ID as FieldID")
            Stb.AppendLine("   , b.name as BoundaryName")
            Stb.AppendLine("   , b.ID as BoundaryID")
            Stb.AppendLine("   , c.name as ClientName ")
            Stb.AppendLine("   , c.ID as ClientID ")
            Stb.AppendLine("   , ff.name as FarmName ")
            Stb.AppendLine("   , ff.id as FarmID")

            Stb.AppendLine("   , '' as App_Nome")
            Stb.AppendLine("   --   Altre Info")
            Stb.AppendLine("   , o.Data_Modifica")
            Stb.AppendLine("   , isnull(DocE.Allegati_Documenti_Cod, 0) as allegatoCollegato_Allegati_Documenti_Cod")

            Stb.AppendLine(" FROM jDeereDataModel_FieldOperation o ")
            Stb.AppendLine("     inner Join jDeereDataModel_Field f ")
            Stb.AppendLine("         On f.ID =o.FieldID ")
            Stb.AppendLine("     inner Join jDeereDataModel_Client c ")
            Stb.AppendLine("         On c.ID = f.ClientID ")
            Stb.AppendLine("     inner Join jDeereDataModel_Farm ff ")
            Stb.AppendLine("         On ff.ID = f.FarmID ")
            Stb.AppendLine("     Left Join jDeereDataModel_FieldXBoundary fb ")
            Stb.AppendLine("         On fb.IDField = f.ID ")
            Stb.AppendLine("     Left Join jdeereDataModel_Boundary b ")
            Stb.AppendLine("         On b.ID = fb.IDBoundary")

            Stb.AppendLine("     Left Join [dbo].[Alert_EntitaxIndici] ee ")
            Stb.AppendLine("         On ee.valore_des = o.Allegati_Documenti_Cod ")
            Stb.AppendLine("         And ee.id_indice = -1 ")
            Stb.AppendLine("      Left Join Alert_Entita DocE ")
            Stb.AppendLine("         On DocE.ID_Alert_Entita = ee.ID_Alert_Entita ")

            Stb.AppendLine(" WHERE o.OrganizationID =  " & Agro_SQL_SaveNum(OrganizationID))
            Stb.AppendLine(" AND o.FieldID =  " & Agro_SQL_SaveNum(FieldID))
            Stb.AppendLine(" AND b.active=1 ")

            Stb.AppendLine(" And Not exists ( ")
            Stb.AppendLine("     Select 1 ")
            Stb.AppendLine("     From [dbo].[jDeereDataModel_EntitaGIAS] gg ")
            Stb.AppendLine("     Where gg.TipoJD =  " & Agro_SQL_SaveNum(TipiEnumerativi.enum_TipoEntitaJohnDeere.Operation))
            Stb.AppendLine("     AND gg.IDJD = o.ID")
            Stb.AppendLine("  )")


            If xFiltroAggiuntivoJohnDeere1 <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivoJohnDeere1))
            End If

            If xOrderByJohnDeere1 <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderByJohnDeere1))
            End If


            Stb.AppendLine("union all")

            Stb.AppendLine(" SELECT  " & vbCrLf)
            Stb.AppendLine("     'Sincro' as Tipo  ")
            Stb.AppendLine("   , cast(oG.Ricetta_Operazione_Cod as varchar(50)) + '-' + cast (o.ID as varchar(50)) as chiave ")
            Stb.AppendLine("   --   Dati JD")
            Stb.AppendLine("   , fieldOperationType")
            Stb.AppendLine("   , startDate")
            Stb.AppendLine("   , endDate")
            Stb.AppendLine("   --   Dati GIAS")
            Stb.AppendLine("   , oG.Ricetta_Operazione_Des as DescrizioneGias ")
            Stb.AppendLine("   --   info ""field"" ed appezzamento")
            Stb.AppendLine("   , case when b.ID is null then 0 else 1 end as ConfiniPresenti")
            Stb.AppendLine("   , f.Name as FieldName")
            Stb.AppendLine("   , f.ID as FieldID")
            Stb.AppendLine("   , b.name as BoundaryName")
            Stb.AppendLine("   , b.ID as BoundaryID")
            Stb.AppendLine("   , c.name as ClientName ")
            Stb.AppendLine("   , c.ID as ClientID ")
            Stb.AppendLine("   , ff.name as FarmName ")
            Stb.AppendLine("   , ff.id as FarmID")

            Stb.AppendLine("   , '' as App_Nome")
            Stb.AppendLine("   --   Altre Info")
            Stb.AppendLine("   , o.Data_Modifica")
            Stb.AppendLine("   , isnull(DocE.Allegati_Documenti_Cod, 0) as allegatoCollegato_Allegati_Documenti_Cod")

            Stb.AppendLine(" FROM jDeereDataModel_FieldOperation o ")
            Stb.AppendLine("     inner Join jDeereDataModel_Field f ")
            Stb.AppendLine("         On f.ID =o.FieldID ")
            Stb.AppendLine("     inner Join jDeereDataModel_Client c ")
            Stb.AppendLine("         On c.ID = f.ClientID ")
            Stb.AppendLine("     inner Join jDeereDataModel_Farm ff ")
            Stb.AppendLine("         On ff.ID = f.FarmID ")
            Stb.AppendLine("     inner Join  [dbo].[jDeereDataModel_EntitaGIAS] gg ")
            Stb.AppendLine("         On  gg.TipoJD = " & Agro_SQL_SaveNum(TipiEnumerativi.enum_TipoEntitaJohnDeere.Operation))
            Stb.AppendLine("         And gg.IDJD = o.ID ")
            Stb.AppendLine("     inner Join Ricette_Operazioni oG ")
            Stb.AppendLine("         On oG.Ricetta_Operazione_Cod = gg.[Gis_Entita_Cod]")
            Stb.AppendLine("     Left Join jDeereDataModel_FieldXBoundary fb ")
            Stb.AppendLine("         On fb.IDField = f.ID ")
            Stb.AppendLine("     Left Join jdeereDataModel_Boundary b ")
            Stb.AppendLine("         On b.ID = fb.IDBoundary")

            Stb.AppendLine("     Left Join [dbo].[Alert_EntitaxIndici] ee ")
            Stb.AppendLine("         On ee.valore_des = o.Allegati_Documenti_Cod ")
            Stb.AppendLine("         And ee.id_indice = -1 ")
            Stb.AppendLine("      Left Join Alert_Entita DocE ")
            Stb.AppendLine("         On DocE.ID_Alert_Entita = ee.ID_Alert_Entita ")

            Stb.AppendLine(" WHERE o.OrganizationID =  " & Agro_SQL_SaveNum(OrganizationID))
            Stb.AppendLine(" AND o.FieldID =  " & Agro_SQL_SaveNum(FieldID))
            Stb.AppendLine(" AND b.active=1 ")

            If xFiltroAggiuntivoJohnDeere2 <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivoJohnDeere2))
            End If

            If xOrderByJohnDeere2 <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderByJohnDeere2))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   o.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   o.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


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

    Public Function LeggiFieldOperation(
            ByVal FieldOperationID As Integer,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiFieldOperation()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" select ")
            Stb.AppendLine(" 	ID, ")
            Stb.AppendLine(" 	fieldOperationType, ")
            Stb.AppendLine("    adaptMachineType, ")
            Stb.AppendLine(" 	cropSeason, ")
            Stb.AppendLine(" 	startDate, ")
            Stb.AppendLine(" 	endDate, ")
            Stb.AppendLine(" 	operationData, ")
            Stb.AppendLine(" 	guid, ")
            Stb.AppendLine(" 	FieldID, ")
            Stb.AppendLine(" 	OrganizationID, ")
            Stb.AppendLine(" 	Allegati_Documenti_Cod ")
            Stb.AppendLine(" from ")
            Stb.AppendLine(" 	jDeereDataModel_FieldOperation ")
            Stb.AppendLine(" where ")
            Stb.AppendLine(" 	ID=" + Agro_SQL_SaveNum(FieldOperationID))

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
                ByVal COD As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim TestataElem As jDeereDataModel_FieldOperation = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModelDAL_FieldOperation_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_FieldOperation
            Where m.ID = COD
            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(TestataElem)


    End Function

    Public Function Leggi(
                ByVal ID As Integer,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As jDeereDataModel_FieldOperation

        Dim TestataElem As jDeereDataModel_FieldOperation = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_FieldOperation_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_FieldOperation
            Where m.ID = ID
            Select m).FirstOrDefault()

        End Using

        Return TestataElem

    End Function

    Public Function TestGUID(
               ByVal guid As String,
               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
       ) As Boolean

        Dim TestataElem As jDeereDataModel_FieldOperation = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_FieldOperation_R.TestGUID()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_FieldOperation
            Where m.guid = guid
            Select m).FirstOrDefault()

        End Using

        Return Not IsNothing(TestataElem)


    End Function

    Public Function LeggiIDViaGUID(
                ByVal guid As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Integer

        Dim TestataElem As jDeereDataModel_FieldOperation = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_FieldOperation_R.LeggiIDViaGUID()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_FieldOperation
            Where m.guid = guid
            Select m).FirstOrDefault()

        End Using

        If TestataElem Is Nothing Then
            Return -1
        Else
            Return TestataElem.ID
        End If
    End Function

    Public Function LeggiGUID(
               ByVal ID As Integer,
               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
       ) As String

        Dim TestataElem As jDeereDataModel_FieldOperation = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_FieldOperation_R.LeggiIDViaGUID()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_FieldOperation
            Where m.ID = ID
            Select m).FirstOrDefault()

        End Using

        If TestataElem Is Nothing Then
            Return -1
        Else
            Return TestataElem.guid
        End If
    End Function

    Public Function LeggiFieldID(
               ByVal ID As Integer,
               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
       ) As Integer

        Dim TestataElem As jDeereDataModel_FieldOperation = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "jDeereDataModel_FieldOperation_R.LeggiIDViaGUID()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            TestataElem =
           (From m In GiasContext.jDeereDataModel_FieldOperation
            Where m.ID = ID
            Select m).FirstOrDefault()

        End Using

        If TestataElem Is Nothing Then
            Return -1
        Else
            Return TestataElem.FieldID
        End If
    End Function
End Class

Public Class jDeereDataModelDAL_FieldOperation_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Aggiorna_jDeereDataModel_FieldOperation(
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperation_W.Aggiorna_jDeereDataModel_FieldOperation()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each curjDeereDataModel_FieldOperation As jDeereDataModel_FieldOperation In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try
                                If curjDeereDataModel_FieldOperation.ID = 0 Then
                                    'Richiedo un nuovo id sequenza
                                    idSeq = ObjSequenze.NuovoId_Tabella_EF(GiasContext,
                                               "jDeereDataModel_FieldOperation", 0, 2000000000, objParametri)

                                    curjDeereDataModel_FieldOperation.ID = idSeq
                                End If
                                GiasContext.jDeereDataModel_FieldOperation.Add(curjDeereDataModel_FieldOperation)
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
                        For Each listFattVar As jDeereDataModel_FieldOperation In EFArrayToUpdate
                            GiasContext.jDeereDataModel_FieldOperation.Attach(listFattVar)
                            GiasContext.Entry(listFattVar).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listFattVar As jDeereDataModel_FieldOperation In EFArrayToDelete
                            GiasContext.jDeereDataModel_FieldOperation.Attach(listFattVar)
                            GiasContext.jDeereDataModel_FieldOperation.Remove(listFattVar)
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
