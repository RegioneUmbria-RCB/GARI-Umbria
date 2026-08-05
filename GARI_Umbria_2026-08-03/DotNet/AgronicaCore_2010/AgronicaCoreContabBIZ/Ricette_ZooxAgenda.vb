Imports AgronicaCoreEntityFramework

Public Class Ricette_ZooxAgenda_R
    Inherits AgronicaCoreDataProvider.DataProvider

End Class

Public Class Ricette_ZooxAgenda_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Modifica(ByVal Ricetta_ZooxAgenda As AgronicaCoreEntityFramework_POCO.Ricette_ZooxAgenda,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricette_ZooxAgenda_W.Scrivi_Modifica()"
        Dim messaggioErrore As String = ""

        Try
            Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim esiste = False

            Dim idRicetta As Integer = Ricetta_ZooxAgenda.Id_Ricetta
            Dim idRigaRicetta As Integer = Ricetta_ZooxAgenda.Id_RigaRicetta
            Dim idAgenda As Integer = Ricetta_ZooxAgenda.Id_Agenda

            esiste = GiasContext.Ricette_ZooxAgenda.Where(Function(rzxa) rzxa.Id_Ricetta = idRicetta AndAlso
                                                              rzxa.Id_RigaRicetta = idRigaRicetta AndAlso rzxa.Id_Agenda = idAgenda).Any()

            Dim ricetteZooxAgenda_W As New AgronicaCoreContabDAL.Ricette_ZooxAgenda_W
            If esiste Then
                ricetteZooxAgenda_W.Modifica(Ricetta_ZooxAgenda, GiasContext, objParametriServer)
            Else
                ricetteZooxAgenda_W.Scrivi(Ricetta_ZooxAgenda, GiasContext, objParametriServer)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return True

    End Function

    Public Sub Elimina(ByRef Ricette_ZooxAgenda As AgronicaCoreEntityFramework_POCO.Ricette_ZooxAgenda(),
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricette_ZooxAgenda_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try
            Dim ricetteZooxAgenda_W As New AgronicaCoreContabDAL.Ricette_ZooxAgenda_W
            For Each ricetta In Ricette_ZooxAgenda
                ricetteZooxAgenda_W.Elimina(ricetta, GiasContext, objParametriServer)
            Next
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

End Class
