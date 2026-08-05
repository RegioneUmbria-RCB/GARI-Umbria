Imports AgronicaCoreEntityFramework

Public Class Ricette_Zoo_Movimenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

End Class

Public Class Ricette_Zoo_Movimenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Modifica(ByVal Ricetta_Zoo_Movimenti As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Movimenti,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricette_Zoo_Movimenti_W.Scrivi_Modifica()"
        Dim messaggioErrore As String = ""
        Dim idMov As Integer = 0

        Try
            Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim esiste = False
            If Ricetta_Zoo_Movimenti.IdMov = 0 Then
                idMov = agroDP.NuovoId_Tabella_EF(GiasContext, "ricette_zoo_movimenti", 0, 200000000, objParametriServer)
                Ricetta_Zoo_Movimenti.IdMov = idMov
            Else
                idMov = Ricetta_Zoo_Movimenti.IdMov
                Dim ricetteMovCount = From rz In GiasContext.Ricette_Zoo_Movimenti
                                      Where rz.Piva = Ricetta_Zoo_Movimenti.Piva And
                                         rz.Sa_Cod = Ricetta_Zoo_Movimenti.Sa_Cod And
                                         rz.IdRicetta = Ricetta_Zoo_Movimenti.IdRicetta And
                                         rz.IdAgenda = Ricetta_Zoo_Movimenti.IdAgenda And
                                         rz.IdMov = idMov
                                      Select rz
                If ricetteMovCount.Count > 0 Then
                    esiste = True
                End If
            End If

            Dim ricetteZooMovimenti_W As New AgronicaCoreContabDAL.Ricette_Zoo_Movimenti_W
            If esiste Then
                ricetteZooMovimenti_W.Modifica(Ricetta_Zoo_Movimenti, GiasContext, objParametriServer)
            Else
                ricetteZooMovimenti_W.Scrivi(Ricetta_Zoo_Movimenti, GiasContext, objParametriServer)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return idMov

    End Function

    Public Sub Elimina(ByRef Ricette_Zoo_Movimenti As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Movimenti(),
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricette_Zoo_Movimenti_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try
            Dim ricetteZooMovimenti_W As New AgronicaCoreContabDAL.Ricette_Zoo_Movimenti_W
            For Each ricetta In Ricette_Zoo_Movimenti
                ricetteZooMovimenti_W.Elimina(ricetta, GiasContext, objParametriServer)
            Next
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

End Class
