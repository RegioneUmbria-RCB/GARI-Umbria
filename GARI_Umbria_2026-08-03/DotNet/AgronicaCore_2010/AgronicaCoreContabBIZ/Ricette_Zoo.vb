Imports AgronicaCoreContabDAL
Imports AgronicaCoreEntityFramework

Public Class Ricette_Zoo_R
    Inherits AgronicaCoreDataProvider.DataProvider

End Class

Public Class Ricette_Zoo_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Modifica(ByVal Ricetta_Zoo As AgronicaCoreEntityFramework_POCO.Ricette_Zoo,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricette_Zoo_W.Scrivi_Modifica()"
        Dim messaggioErrore As String = ""
        Dim idRicetta As Integer = 0

        Try
            Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim esiste = False
            If Ricetta_Zoo.IdRicetta = 0 Then
                idRicetta = agroDP.NuovoId_Tabella_EF(GiasContext, "ricette_zoo", 0, 200000000, objParametriServer)
                Ricetta_Zoo.IdRicetta = idRicetta
            Else
                idRicetta = Ricetta_Zoo.IdRicetta
                Dim ricetteCount = From rz In GiasContext.Ricette_Zoo
                                   Where rz.Piva = Ricetta_Zoo.Piva And
                                         rz.Sa_Cod = Ricetta_Zoo.Sa_Cod And
                                         rz.Sta_Num = Ricetta_Zoo.Sta_Num And
                                         rz.IdRicetta = idRicetta
                                   Select rz
                If ricetteCount.Count > 0 Then
                    esiste = True
                End If

            End If

            Dim ricetteZoo_W As New AgronicaCoreContabDAL.Ricette_Zoo_W
            If esiste Then
                ricetteZoo_W.Modifica(Ricetta_Zoo, GiasContext, objParametriServer)
            Else
                ricetteZoo_W.Scrivi(Ricetta_Zoo, GiasContext, objParametriServer)
            End If
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return idRicetta

    End Function

    Public Sub Elimina(ByRef Ricette_Zoo As AgronicaCoreEntityFramework_POCO.Ricette_Zoo(),
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricette_Zoo_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try
            Dim ricetteZoo_W As New AgronicaCoreContabDAL.Ricette_Zoo_W
            For Each ricetta In Ricette_Zoo
                ricetteZoo_W.Elimina(ricetta, GiasContext, objParametriServer)
            Next
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

    Public Sub EliminaRicetteCollegate(ByRef Ricette_Zoo As AgronicaCoreEntityFramework_POCO.Ricette_Zoo(),
                                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Const nomeRoutine = "AgronicaCoreContabBIZ.Ricette_Zoo_W.EliminaRicetteCollegate()"
        Dim messaggioErrore As String = ""

        Try
            Dim ricetteZoo_W As New AgronicaCoreContabDAL.Ricette_Zoo_W
            Dim ricetteZooAgenda_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Agenda_W
            Dim ricetteZooxAgenda_W As New AgronicaCoreContabBIZ.Ricette_ZooxAgenda_W
            Dim ricetteZooMovimenti_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Movimenti_W
            Dim ricetteZooDettagli_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Dettagli_W
            Dim ricetteZooDettTecnico_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Dettaglio_Tecnico_W
            Dim ricetteZooDestinazioni_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Destinazioni_W

            For Each ricetta In Ricette_Zoo
                Dim piva As String = ricetta.Piva
                Dim saCod As Integer = ricetta.Sa_Cod
                Dim idRicetta As Integer = ricetta.IdRicetta

                'RICETTE_ZOO_DESTINAZIONI
                Dim ricetteZooDestinazioniToDelete = GiasContext.Ricette_Zoo_Destinazioni.
                    Where(Function(rzd) rzd.Piva = piva AndAlso rzd.Sa_Cod = saCod AndAlso rzd.IdRicetta = idRicetta).ToArray
                ricetteZooDestinazioni_W.Elimina(ricetteZooDestinazioniToDelete, GiasContext, objParametriServer)

                'RICETTE_ZOO_DETTAGLI
                Dim ricetteZooDettagliToDelete = GiasContext.Ricette_Zoo_Dettagli.
                    Where(Function(rzd) rzd.Piva = piva AndAlso rzd.Sa_Cod = saCod AndAlso rzd.IdRicetta = idRicetta).ToArray
                ricetteZooDettagli_W.Elimina(ricetteZooDettagliToDelete, GiasContext, objParametriServer)

                'RICETTE_ZOO_MOVIMENTI
                Dim ricetteZooMovimentiToDelete = GiasContext.Ricette_Zoo_Movimenti.
                    Where(Function(rzm) rzm.Piva = piva AndAlso rzm.Sa_Cod = saCod AndAlso rzm.IdRicetta = idRicetta).ToArray
                ricetteZooMovimenti_W.Elimina(ricetteZooMovimentiToDelete, GiasContext, objParametriServer)

                'RICETTE_ZOOXAGENDA
                Dim ricetteZooxAgendaToDelete = GiasContext.Ricette_ZooxAgenda.Where(Function(rzxa) rzxa.Id_Ricetta = idRicetta).ToArray
                ricetteZooxAgenda_W.Elimina(ricetteZooxAgendaToDelete, GiasContext, objParametriServer)

                'RICETTE_ZOO_AGENDA
                Dim ricetteZooAgendaToDelete = GiasContext.Ricette_Zoo_Agenda.
                    Where(Function(rza) rza.Piva = piva AndAlso rza.Sa_Cod = saCod AndAlso rza.IdRicetta = idRicetta).ToArray

                'RICETTE_ZOO_DETTAGLIO_TECNICO
                Dim idAgendaToDelete = ricetteZooAgendaToDelete.Select(Function(rza) rza.IdAgenda).ToList
                Dim ricetteZooDettTecnToDelete = GiasContext.Ricette_Zoo_Dettaglio_Tecnico.
                    Where(Function(rzdt) rzdt.Piva = piva AndAlso rzdt.Sa_Cod = saCod AndAlso idAgendaToDelete.Contains(rzdt.Id_Agenda)).ToArray
                ricetteZooDettTecnico_W.elimina(ricetteZooDettTecnToDelete, GiasContext, objParametriServer)

                ricetteZooAgenda_W.Elimina(ricetteZooAgendaToDelete, GiasContext, objParametriServer)

                'RICETTE_ZOO
                ricetteZoo_W.Elimina(ricetta, GiasContext, objParametriServer)
            Next
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

End Class
