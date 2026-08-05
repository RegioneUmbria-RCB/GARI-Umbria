Imports AgronicaCoreEntityFramework

Public Class Ricette_Zoo_Destinazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

End Class

Public Class Ricette_Zoo_Destinazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Modifica(ByVal Ricetta_Zoo_Destinazioni As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Destinazioni,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricette_Zoo_Destinazioni_W.Scrivi_Modifica()"
        Dim messaggioErrore As String = ""
        Dim idDestinazione As Integer = 0

        Try
            Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim esiste = False
            If Ricetta_Zoo_Destinazioni.IdDestinazione = 0 Then
                idDestinazione = agroDP.NuovoId_Tabella_EF(GiasContext, "ricette_zoo_destinazioni", 0, 200000000, objParametriServer)
                Ricetta_Zoo_Destinazioni.IdDestinazione = idDestinazione
            Else
                idDestinazione = Ricetta_Zoo_Destinazioni.IdDestinazione
                Dim ricetteDestCount = From rz In GiasContext.Ricette_Zoo_Destinazioni
                                       Where rz.Piva = Ricetta_Zoo_Destinazioni.Piva And
                                         rz.Sa_Cod = Ricetta_Zoo_Destinazioni.Sa_Cod And
                                         rz.IdRicetta = Ricetta_Zoo_Destinazioni.IdRicetta And
                                         rz.IdAgenda = Ricetta_Zoo_Destinazioni.IdAgenda And
                                         rz.IdMov = Ricetta_Zoo_Destinazioni.IdMov And
                                         rz.IdDettaglio = Ricetta_Zoo_Destinazioni.IdDettaglio And
                                         rz.IdDestinazione = idDestinazione
                                       Select rz
                If ricetteDestCount.Count > 0 Then
                    esiste = True
                End If
            End If

            Dim ricetteZooDestinazioni_W As New AgronicaCoreContabDAL.Ricette_Zoo_Destinazioni_W
            If esiste Then
                ricetteZooDestinazioni_W.Modifica(Ricetta_Zoo_Destinazioni, GiasContext, objParametriServer)
            Else
                ricetteZooDestinazioni_W.Scrivi(Ricetta_Zoo_Destinazioni, GiasContext, objParametriServer)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return idDestinazione

    End Function

    Public Sub Elimina(ByRef Ricetta_Zoo_Destinazioni As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Destinazioni(),
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricette_Zoo_Destinazioni_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try
            Dim ricetteZooDestinazioni_W As New AgronicaCoreContabDAL.Ricette_Zoo_Destinazioni_W
            For Each ricetta In Ricetta_Zoo_Destinazioni
                ricetteZooDestinazioni_W.Elimina(ricetta, GiasContext, objParametriServer)
            Next
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

End Class
