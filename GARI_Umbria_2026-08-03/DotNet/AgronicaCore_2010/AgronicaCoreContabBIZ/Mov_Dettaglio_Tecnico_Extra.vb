Imports System.Data.Entity
Imports AgronicaCoreEntityFramework

Public Class Mov_Dettaglio_Tecnico_Extra
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Scrivi_Modifica(ByVal Movimenti As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico_Extra,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Integer

        Const nomeRoutine = "AgronicaCoreContabBIZ.Mov_Dettaglio_Tecnico_Extra.Scrivi_Modifica()"
        Dim messaggioErrore As String = ""
        Dim Piva As String = ""
        Dim Sa_Cod As Integer = 0
        Dim Id_Agenda As Integer = 0
        Dim idMov As Integer = 0
        Dim idMov_Det As Integer = 0
        Dim Id_Reg_Dettaglio As Integer = 0

        Try
            Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim esiste = False

            If Movimenti.Id_Mov = 0 Then

                idMov_Det = agroDP.NuovoId_Tabella_EF(GiasContext, "MOVIMENTI_DETTAGLI_TECNICI", 0, 200000000, objParametriServer)
                Movimenti.Id_Mov_Det = idMov_Det

            Else

                Piva = Movimenti.Piva
                Sa_Cod = Movimenti.Sa_Cod
                Id_Agenda = Movimenti.Id_Agenda
                idMov = Movimenti.Id_Mov
                idMov_Det = Movimenti.Id_Mov_Det
                Id_Reg_Dettaglio = Movimenti.Id_Reg_Dettaglio

                Dim movimentiCount = (From a In GiasContext.Mov_Dettaglio_Tecnico_Extra
                                      Where a.Id_Agenda = Movimenti.Id_Agenda And
                                         a.Piva = Piva And
                                         a.Sa_Cod = Sa_Cod And
                                         a.Id_Mov = idMov And
                                         a.Id_Mov_Det = idMov_Det And
                                         a.Id_Reg_Dettaglio = Id_Reg_Dettaglio
                                      Select a).ToList
                If movimentiCount.Count > 0 Then
                    esiste = True
                End If

            End If


            Dim movimentiW As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_Extra_W

            If esiste Then
                movimentiW.Modifica(Movimenti, GiasContext, objParametriServer)
            Else
                movimentiW.Scrivi(Movimenti, GiasContext, objParametriServer)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return idMov

    End Function

    Public Sub Elimina(ByRef Movimenti As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico_Extra(),
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreContabBIZ.Mov_Dettaglio_Tecnico_Extra.Elimina()"
        Dim messaggioErrore As String = ""

        Try

            Dim movimentiW As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_Extra_W
            For Each Movimento In Movimenti
                movimentiW.Elimina(Movimento, GiasContext, objParametriServer)
            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub


End Class
