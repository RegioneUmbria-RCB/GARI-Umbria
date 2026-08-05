Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions

Public Class Gruppi_Raccolta
    Public Function ScriviModificaCancella_GruppiRaccolta(
        ByRef objGruppoRaccolta As AgronicaCoreModelsSTD.anagrafiche.GruppoRaccolta,
        ByVal tipoOperazione As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri,
        Optional NoteLog As String = NOTELOG_ANAGRAFE_NG
    ) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Campo_W.Scrivi_Campo_Anagrafica()"
        Dim MessaggioErrore As String = ""

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser
        Dim dal As New AgronicaCoreAnagrafeDAL.EFGruppoRaccolta()

        Dim ObjSequenze = New Agro_Sequenze
        Dim success As Boolean = True

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Dim stWa As New Stopwatch
        stWa.Start()
        Dim scopeOption As New TransactionScopeOption
        Dim transactionOptions As New TransactionOptions
        transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted

        Dim EntitaCodxImg As New List(Of Integer)

        Using scope As New TransactionScope(scopeOption, transactionOptions)
            Using GiasContext As Gias_DeveloperServer_Entities = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)

                Try
                    'dal.ContextOptions.UseLegacyPreserveChangesBehavior = False
                    'Open the contextObject connection state explicitly
                    'GiasContext.Database.Connection.Open()

                    ''disabilitaMergeOptions(dal)

                    'GiasContext.Database.ExecuteSqlCommand("SET ARITHABORT ON;")


                    If (tipoOperazione = enum_TipoOperazioneDB.Scrittura) Then

                        Dim Campo_POCO = dal.GruppoRaccolta_Scrivi_EF(
                            objGruppoRaccolta,
                            objParametri_Server,
                            objParametri_Utenti.UsernameOperazione,
                            GiasContext,
                            False,
                            NoteLog:=NoteLog
                        )

                    ElseIf (tipoOperazione = enum_TipoOperazioneDB.Modifica) Then
                        '--------------------------------------------------------------------------------------
                        ' aggiorno le validita solo se è date di inizio o fine del campo sono variate
                        Dim gruppiRaccolta_Read As New AgronicaCoreAnagrafeDAL.Gruppi_Raccolta_Read
                        Dim dt = gruppiRaccolta_Read.LeggiGruppoRaccolta(
                            objGruppoRaccolta.codice,
                            objParametri_Server
                        )

                        Dim gr As New GruppoRaccolta(0, "")

                        If dt.Rows.Count = 1 Then
                            gr = New GruppoRaccolta(dt.Rows(0).Item("GruppoRaccolta_Cod"), dt.Rows(0).Item("GruppoRaccolta_Des"))
                            gr.validita = New IntervalloTemporale(dt.Rows(0).Item("Validita_Inizio"), dt.Rows(0).Item("Validita_Fine"))
                        End If

                        Dim GruppoRaccolta_POCO = dal.GruppoRaccolta_Modifica_EF(objGruppoRaccolta, objParametri_Server, objParametri_Server.UsernameOperazione, GiasContext, False, NoteLog:=NoteLog)

                    ElseIf (tipoOperazione = enum_TipoOperazioneDB.Cancellazione) Then

                        dal.GruppoRaccolta_Cancella_EF(objGruppoRaccolta, objParametri_Server, GiasContext, False)

                    End If

                    scope.Complete()

                Catch ex As GiasException
                    If scope IsNot Nothing Then
                        scope.Dispose()
                    End If
                    MessaggioErrore = ex.Message
                    Throw ex
                Catch ex As Exception
                    scope.Dispose()
                    MessaggioErrore = ex.Message
                    Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

                Finally

                    If GiasContext IsNot Nothing AndAlso GiasContext.Database.Connection.State = ConnectionState.Open Then
                        GiasContext.Database.Connection.Close()
                    End If

                End Try

            End Using
        End Using

        stWa.Stop()
        Dim totalTime As TimeSpan = stWa.Elapsed

        Return MessaggioErrore

    End Function

End Class
