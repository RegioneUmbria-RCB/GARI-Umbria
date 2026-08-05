Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL

Public Class EF_GIS_Entita
    Inherits AgronicaCoreDataProvider.DataProvider

    Private Shared Function Create_GIS_Entita(ByRef dal As Gias_DeveloperServer_Entities,
                                              ByRef objParametri As AgronicaCoreParametri,
                                              ByRef username As String
                                              ) As GIS_Entita

        Dim entita As New GIS_Entita
        entita.PivaSuperUser = objParametri.PivaSuperUser
        entita.Entita_Cod = NuovoEntita_Cod(dal, objParametri)
        entita.TipoEntita_Cod = 0
        entita.Piva = ""
        entita.Sa_Cod = 0
        entita.Appezza = 0
        entita.Campo_Cod = 0
        entita.Id_Imp = 0
        entita.PROV = 0
        entita.COM = 0
        entita.SEZIONE = -1
        entita.FOGLIO = -1
        entita.NUMERO = -1
        entita.SUBALTERNO = "-1"
        entita.Programmazione_Entita_Cod = 0
        entita.ID_Agenda = 0
        entita.Ricetta_Operazione_cod = 0
        entita.analisi_campione_cod = 0
        entita.OLDGrafica_ID = ""
        entita.inviato = 0
        entita.Data_Creazione = DateTime.Now
        entita.Data_Modifica = DateTime.Now
        entita.Username_Creazione = username
        entita.Username_Modifica = username
        entita.Validita_Inizio = AGRODATAINIZIO
        entita.Validita_Fine = AGRODATAFINE
        entita.programmazione_cod = 0
        entita.id_mov_det = 0
        entita.Fabbricato_Cod = 0
        entita.Area_Cod = 0

        Return entita

    End Function


    Private Shared Function NuovoEntita_Cod(ByRef GiasContext As Gias_DeveloperServer_Entities, ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim idGen As New Agro_Sequenze
        'Dim entita_cod = idGen.NuovoId_Tabella_EF(GiasContext, "gis_entita", 0, 2000000000, objParametri)
        Dim entita_cod = idGen.NuovoId_Tabella("gis_entita", 0, 2000000000, objParametri)

        Return entita_cod
    End Function


    Public Shared Function GIS_Entita_Scrivi_EF(
        ByVal Dati_GIS_Entita As Entita,
        ByRef objParametriServer As AgronicaCoreParametri,
        ByVal username As String,
        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
        Optional ByVal NewTransaction As Boolean = True,
        Optional ByVal LogVerbose As Boolean = False
        ) As GIS_Entita

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Scrivi_EF()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False
        'Lavez - 27/05/2025 - Log verboso
        Dim logprovder As LogProvider = Nothing

        If LogVerbose Then
            logprovder = New LogProvider()
        End If
        'Lavez - 27/05/2025 - Log verboso

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If NewTransaction Then
            scope = New TransactionScope()
        End If
        'Lavez - 27/05/2025 - Log verboso
        If LogVerbose Then
            logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Prima di Create_GIS_Entita (calcolo nuovo id)", False)
        End If
        Dim entita = Create_GIS_Entita(GiasContext,
                                       objParametriServer,
                                       username)
        'Lavez - 27/05/2025 - Log verboso
        If LogVerbose Then
            logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Dopo di Create_GIS_Entita (calcolo nuovo id)", False)
        End If
        Try

            entita.TipoEntita_Cod = Dati_GIS_Entita.TipoEntita
            entita.Piva = Dati_GIS_Entita.Piva
            entita.Sa_Cod = Dati_GIS_Entita.Sa_Cod
            entita.Appezza = Dati_GIS_Entita.Appezza
            entita.Campo_Cod = Dati_GIS_Entita.Campo_cod
            entita.Id_Imp = Dati_GIS_Entita.Id_Imp
            entita.analisi_campione_cod = Dati_GIS_Entita.Analisi_Campione_Cod
            entita.Fabbricato_Cod = Dati_GIS_Entita.Fabbricato_Cod

            GiasContext.GIS_Entita.Add(entita)
            GiasContext.SaveChanges()

            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Dopo di Create_GIS_Entita (save changes)", False)
            End If

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As Exception

            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return entita

    End Function

    Public Shared Function GIS_Entita_Modifica_EF(
        ByVal Dati_GIS_Entita As Entita,
        ByRef objParametriServer As AgronicaCoreParametri,
        ByVal username As String,
        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
        Optional ByVal NewTransaction As Boolean = True,
        Optional ByVal AggiornaTipoEntita As Boolean = False,
        Optional ByVal LogVerbose As Boolean = False
        ) As GIS_Entita

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Modifica_EF()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        'Lavez - 27/05/2025 - Log verboso
        Dim logprovder As LogProvider = Nothing

        If LogVerbose Then
            logprovder = New LogProvider()
        End If
        'Lavez - 27/05/2025 - Log verbos

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If NewTransaction Then
            scope = New TransactionScope()
        End If

        'Lavez - 27/05/2025 - Log verboso
        If LogVerbose Then
            logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Prima di lettura entita", False)
        End If

        Dim entitaList = From ent In GiasContext.GIS_Entita
                         Where ent.PivaSuperUser = Dati_GIS_Entita.PivaSuperUser AndAlso
                             ent.Entita_Cod = Dati_GIS_Entita.EntitaCod
                         Select ent

        Dim entita = entitaList.FirstOrDefault

        'Lavez - 27/05/2025 - Log verboso
        If LogVerbose Then
            logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Dopo di lettura entita", False)
        End If

        Try

            entita.Piva = Dati_GIS_Entita.Piva
            entita.Sa_Cod = Dati_GIS_Entita.Sa_Cod
            entita.Appezza = Dati_GIS_Entita.Appezza
            entita.Campo_Cod = Dati_GIS_Entita.Campo_cod
            entita.Id_Imp = Dati_GIS_Entita.Id_Imp

            If AggiornaTipoEntita Then
                entita.TipoEntita_Cod = Dati_GIS_Entita.TipoEntita
            End If

            GiasContext.GIS_Entita.Attach(entita)
            GiasContext.Entry(entita).State = Entity.EntityState.Modified
            GiasContext.SaveChanges()

            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Dopo di SaveChanges", False)
            End If

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return entita

    End Function

    Public Shared Sub GIS_Entita_Cancella_EF(ByVal Dati_GIS_Entita As Entita,
                                             ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             ByVal username As String,
                                             Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                             Optional ByVal NewTransaction As Boolean = True
                                             )

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Cancella_EF()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim entitaList = From ent In GiasContext.GIS_Entita
                         Where ent.PivaSuperUser = Dati_GIS_Entita.PivaSuperUser AndAlso
                                 ent.Entita_Cod = Dati_GIS_Entita.EntitaCod
                         Select ent

        Dim entita = entitaList.FirstOrDefault

        Try

            GiasContext.GIS_Entita.Attach(entita)
            GiasContext.GIS_Entita.Remove(entita)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

    End Sub
End Class
