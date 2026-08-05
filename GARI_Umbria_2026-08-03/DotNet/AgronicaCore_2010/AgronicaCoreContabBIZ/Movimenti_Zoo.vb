Imports System.Data.OleDb
Imports System.Xml
Imports System
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider
Imports System.Threading.Tasks
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Web
Imports AgronicaCoreMetaSchemaDAL
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreUtility
Imports System.Collections.Concurrent
Imports AgronicaCoreDataProvider.My.Resources
Imports System.Linq

Public Class Movimenti_Zoo_R
    Inherits AgronicaCoreDataProvider.DataProvider

End Class

Public Class Movimenti_Zoo_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Modifica(ByVal Movimenti_Zoo As AgronicaCoreEntityFramework_POCO.Movimenti_Zoo,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByRef objParametriServer As AgronicaCoreParametri) As Integer
        Const nomeRoutine = "AgronicaCoreContabBIZ.Movimenti_Zoo_W.Scrivi_Modifica()"
        Dim messaggioErrore As String = ""
        Dim piva As String = ""
        Dim saCod As Integer = 0
        Dim idAgenda As Integer = 0
        Dim idMov As Integer = 0

        Try
            Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim esiste = False

            If Movimenti_Zoo.Piva = String.Empty Or Movimenti_Zoo.Id_Agenda = 0 Or Movimenti_Zoo.Id_Mov = 0 Then
                Throw New Exception("Un campo chiave non è stato valorizzato, impossibile salvare il record.")
            Else
                piva = Movimenti_Zoo.Piva
                saCod = Movimenti_Zoo.Sa_Cod
                idAgenda = Movimenti_Zoo.Id_Agenda
                idMov = Movimenti_Zoo.Id_Mov

                Dim movimentiCount = From mz In GiasContext.Movimenti_Zoo
                                     Where mz.Piva = piva And
                                         mz.Sa_Cod = saCod And
                                         mz.Id_Agenda = idAgenda And
                                         mz.Id_Mov = idMov
                                     Select mz
                If movimentiCount.Count > 0 Then
                    esiste = True
                End If

            End If

            Dim movZ_R As New AgronicaCoreContabDAL.Movimenti_Zoo_W
            If esiste Then
                movZ_R.Modifica(Movimenti_Zoo, GiasContext, objParametriServer)
            Else
                movZ_R.Scrivi(Movimenti_Zoo, GiasContext, objParametriServer)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return idMov

    End Function

    Public Sub Elimina(ByRef Movimenti_Zoo As AgronicaCoreEntityFramework_POCO.Movimenti_Zoo(),
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreParametri)
        Const nomeRoutine = "AgronicaCoreContabBIZ.Movimenti_Zoo_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try
            Dim movimentiZoo_W As New AgronicaCoreContabDAL.Movimenti_Zoo_W
            For Each movz In Movimenti_Zoo
                movimentiZoo_W.Elimina(movz, GiasContext, objParametriServer)
            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

End Class
