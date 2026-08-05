Imports AgronicaCoreEntityFramework

Public Class Ricette_Zoo_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider

End Class

Public Class Ricette_Zoo_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Modifica(ByVal Ricetta_Zoo_Dettagli As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettagli,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricette_Zoo_Dettagli_W.Scrivi_Modifica()"
        Dim messaggioErrore As String = ""
        Dim idDettaglio As Integer = 0

        Try
            Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim esiste = False
            If Ricetta_Zoo_Dettagli.IdDettaglio = 0 Then
                idDettaglio = agroDP.NuovoId_Tabella_EF(GiasContext, "ricette_zoo_dettagli", 0, 200000000, objParametriServer)
                Ricetta_Zoo_Dettagli.IdDettaglio = idDettaglio
            Else
                idDettaglio = Ricetta_Zoo_Dettagli.IdDettaglio
                Dim ricetteDettCount = From rz In GiasContext.Ricette_Zoo_Dettagli
                                       Where rz.Piva = Ricetta_Zoo_Dettagli.Piva And
                                         rz.Sa_Cod = Ricetta_Zoo_Dettagli.Sa_Cod And
                                         rz.IdRicetta = Ricetta_Zoo_Dettagli.IdRicetta And
                                         rz.IdAgenda = Ricetta_Zoo_Dettagli.IdAgenda And
                                         rz.IdMov = Ricetta_Zoo_Dettagli.IdMov And
                                         rz.IdDettaglio = idDettaglio
                                       Select rz
                If ricetteDettCount.Count > 0 Then
                    esiste = True
                End If
            End If

            Dim ricetteZooDettagli_W As New AgronicaCoreContabDAL.Ricette_Zoo_Dettagli_W
            If esiste Then
                ricetteZooDettagli_W.Modifica(Ricetta_Zoo_Dettagli, GiasContext, objParametriServer)
            Else
                ricetteZooDettagli_W.Scrivi(Ricetta_Zoo_Dettagli, GiasContext, objParametriServer)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return idDettaglio

    End Function

    Public Sub Elimina(ByRef Ricetta_Zoo_Dettagli As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettagli(),
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricetta_Zoo_Dettagli_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try
            Dim ricetteZooDettagli_W As New AgronicaCoreContabDAL.Ricette_Zoo_Dettagli_W
            For Each ricetta In Ricetta_Zoo_Dettagli
                ricetteZooDettagli_W.Elimina(ricetta, GiasContext, objParametriServer)
            Next
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

End Class
