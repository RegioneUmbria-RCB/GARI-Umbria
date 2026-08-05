Imports AgronicaCoreEntityFramework

Public Class Ricette_Zoo_Dettaglio_Tecnico_R
    Inherits AgronicaCoreDataProvider.DataProvider

End Class

Public Class Ricette_Zoo_Dettaglio_Tecnico_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Modifica(ByVal Ricette_Zoo_Dett_Tecnico As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettaglio_Tecnico,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricette_Zoo_Dettaglio_Tecnico_W.Scrivi_Modifica()"
        Dim messaggioErrore As String = ""
        Dim idRegDett As Integer = 0

        Try
            Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim esiste = False
            If Ricette_Zoo_Dett_Tecnico.Id_Reg_Dettaglio = 0 Then
                idRegDett = agroDP.NuovoId_Tabella_EF(GiasContext, "ricette_zoo_dettaglio_tecnico", 0, 200000000, objParametriServer)
                Ricette_Zoo_Dett_Tecnico.Id_Reg_Dettaglio = idRegDett
            Else
                idRegDett = Ricette_Zoo_Dett_Tecnico.Id_Reg_Dettaglio
                Dim ricetteDettTecnCount = From rz In GiasContext.Ricette_Zoo_Dettaglio_Tecnico
                                           Where rz.Piva = Ricette_Zoo_Dett_Tecnico.Piva And
                                             rz.Sa_Cod = Ricette_Zoo_Dett_Tecnico.Sa_Cod And
                                             rz.Id_Ricetta = Ricette_Zoo_Dett_Tecnico.Id_Ricetta And
                                             rz.Id_Agenda = Ricette_Zoo_Dett_Tecnico.Id_Agenda And
                                             rz.Id_Mov = Ricette_Zoo_Dett_Tecnico.Id_Mov And
                                             rz.Id_Mov_Det = Ricette_Zoo_Dett_Tecnico.Id_Mov_Det And
                                             rz.Id_Reg_Dettaglio = idRegDett
                                           Select rz
                If ricetteDettTecnCount.Count > 0 Then
                    esiste = True
                End If
            End If

            Dim ricetteZooDettTecn_W As New AgronicaCoreContabDAL.Ricette_Zoo_Dettaglio_Tecnico_W
            If esiste Then
                ricetteZooDettTecn_W.Modifica(Ricette_Zoo_Dett_Tecnico, GiasContext, objParametriServer)
            Else
                ricetteZooDettTecn_W.Scrivi(Ricette_Zoo_Dett_Tecnico, GiasContext, objParametriServer)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return idRegDett

    End Function

    Public Sub Elimina(ByRef Ricette_Zoo_Dett_Tecnico As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettaglio_Tecnico(),
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricette_Zoo_Dettaglio_Tecnico_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try
            Dim ricetteZooDettTecn_W As New AgronicaCoreContabDAL.Ricette_Zoo_Dettaglio_Tecnico_W
            For Each ricetta In Ricette_Zoo_Dett_Tecnico
                ricetteZooDettTecn_W.Elimina(ricetta, GiasContext, objParametriServer)
            Next
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

End Class
