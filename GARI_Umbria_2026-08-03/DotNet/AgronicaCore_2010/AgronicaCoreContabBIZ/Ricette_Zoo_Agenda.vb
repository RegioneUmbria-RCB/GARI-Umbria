Imports AgronicaCoreEntityFramework

Public Class Ricette_Zoo_Agenda_R
    Inherits AgronicaCoreDataProvider.DataProvider

End Class

Public Class Ricette_Zoo_Agenda_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Modifica(ByVal Ricetta_Zoo_Agenda As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Agenda,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricette_Zoo_Agenda_W.Scrivi_Modifica()"
        Dim messaggioErrore As String = ""
        Dim idAgenda As Integer = 0

        Try
            Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim esiste = False
            If Ricetta_Zoo_Agenda.IdAgenda = 0 Then
                idAgenda = agroDP.NuovoId_Tabella_EF(GiasContext, "ricette_zoo_agenda", 0, 200000000, objParametriServer)
                Ricetta_Zoo_Agenda.IdAgenda = idAgenda
            Else
                idAgenda = Ricetta_Zoo_Agenda.IdAgenda
                Dim ricetteAgCount = From rz In GiasContext.Ricette_Zoo_Agenda
                                     Where rz.Piva = Ricetta_Zoo_Agenda.Piva And
                                         rz.Sa_Cod = Ricetta_Zoo_Agenda.Sa_Cod And
                                         rz.IdRicetta = Ricetta_Zoo_Agenda.IdRicetta And
                                         rz.IdAgenda = idAgenda
                                     Select rz
                If ricetteAgCount.Count > 0 Then
                    esiste = True
                End If
            End If

            Dim ricetteZooAgenda_W As New AgronicaCoreContabDAL.Ricette_Zoo_Agenda_W
            If esiste Then
                ricetteZooAgenda_W.Modifica(Ricetta_Zoo_Agenda, GiasContext, objParametriServer)
            Else
                ricetteZooAgenda_W.Scrivi(Ricetta_Zoo_Agenda, GiasContext, objParametriServer)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return idAgenda

    End Function

    Public Sub Elimina(ByRef Ricette_Zoo_Agenda As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Agenda(),
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricetta_Zoo_Agenda_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try
            Dim ricetteZooAgenda_W As New AgronicaCoreContabDAL.Ricette_Zoo_Agenda_W
            For Each ricetta In Ricette_Zoo_Agenda
                ricetteZooAgenda_W.Elimina(ricetta, GiasContext, objParametriServer)
            Next
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

    Public Sub EliminaRicetteCollegate(ByRef Ricette_Zoo_Agenda As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Agenda(),
                                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricette_Zoo_Agenda_W.EliminaRicetteCollegate()"
        Dim messaggioErrore As String = ""

        Try
            Dim ricetteZooAgenda_W As New AgronicaCoreContabDAL.Ricette_Zoo_Agenda_W
            Dim ricetteZooxAgenda_W As New AgronicaCoreContabBIZ.Ricette_ZooxAgenda_W
            Dim ricetteZooMovimenti_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Movimenti_W
            Dim ricetteZooDettagli_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Dettagli_W
            Dim ricetteZooDettTecn_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Dettaglio_Tecnico_W
            Dim ricetteZooDestinazioni_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Destinazioni_W

            For Each ricettaAg In Ricette_Zoo_Agenda
                Dim piva As String = ricettaAg.Piva
                Dim saCod As Integer = ricettaAg.Sa_Cod
                Dim idRicetta As Integer = ricettaAg.IdRicetta
                Dim idAgenda As Integer = ricettaAg.IdAgenda

                'RICETTE_ZOO_DESTINAZIONI
                Dim ricetteZooDestinazioniToDelete = GiasContext.Ricette_Zoo_Destinazioni.
                    Where(Function(rzd) rzd.Piva = piva AndAlso rzd.Sa_Cod = saCod AndAlso rzd.IdRicetta = idRicetta AndAlso rzd.IdAgenda = idAgenda).ToArray
                ricetteZooDestinazioni_W.Elimina(ricetteZooDestinazioniToDelete, GiasContext, objParametriServer)

                'RICETTE_ZOO_DETTAGLIO_TECNICO
                Dim ricetteZooDettTecnToDelete = GiasContext.Ricette_Zoo_Dettaglio_Tecnico.
                    Where(Function(rzdt) rzdt.Piva = piva AndAlso rzdt.Sa_Cod = saCod AndAlso rzdt.Id_Agenda = idAgenda).ToArray
                ricetteZooDettTecn_W.Elimina(ricetteZooDettTecnToDelete, GiasContext, objParametriServer)

                'RICETTE_ZOO_DETTAGLI
                Dim ricetteZooDettagliToDelete = GiasContext.Ricette_Zoo_Dettagli.
                    Where(Function(rzd) rzd.Piva = piva AndAlso rzd.Sa_Cod = saCod AndAlso rzd.IdRicetta = idRicetta AndAlso rzd.IdAgenda = idAgenda).ToArray
                ricetteZooDettagli_W.Elimina(ricetteZooDettagliToDelete, GiasContext, objParametriServer)

                'RICETTE_ZOO_MOVIMENTI
                Dim ricetteZooMovimentiToDelete = GiasContext.Ricette_Zoo_Movimenti.
                    Where(Function(rzm) rzm.Piva = piva AndAlso rzm.Sa_Cod = saCod AndAlso rzm.IdRicetta = idRicetta AndAlso rzm.IdAgenda = idAgenda).ToArray
                ricetteZooMovimenti_W.Elimina(ricetteZooMovimentiToDelete, GiasContext, objParametriServer)

                Dim ricetteZooxAgendaToDelete = GiasContext.Ricette_ZooxAgenda.
                    Where(Function(rzxa) rzxa.Id_Ricetta = idRicetta AndAlso rzxa.Id_RigaRicetta = idAgenda).ToArray
                ricetteZooxAgenda_W.Elimina(ricetteZooxAgendaToDelete, GiasContext, objParametriServer)

                'RICETTE_ZOO_AGENDA
                ricetteZooAgenda_W.Elimina(ricettaAg, GiasContext, objParametriServer)
            Next
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

End Class
