Imports System.Data.Common
Imports System.Xml
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports System.Runtime.CompilerServices


Public Class Mov_Dettaglio_Tecnico_R
    Inherits AgronicaCoreDataProvider.LogProvider

End Class


Public Class Mov_Dettaglio_Tecnico_W
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Scrivi_Modifica(ByVal mov_dettaglio_tecnico As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico,
                                ByRef GiasContext As Gias_DeveloperServer_Entities,
                                ByRef objParametriServer As AgronicaCoreParametri
                                ) As Integer

        Const nomeRoutine = "ContabBIZ.Mov_Dettaglio_Tecnico_W.Scrivi_Modifica()"
        Dim messaggioErrore As String = ""
        Dim Id_Reg_Dettaglio As Integer = 0

        Try

            Dim agroDP As New Agro_Sequenze
            Dim esiste As Boolean = False
            Dim cambioChiave = False
            If mov_dettaglio_tecnico.Id_Reg_Dettaglio = 0 Then

                Id_Reg_Dettaglio = agroDP.NuovoId_Tabella_EF(GiasContext, "MOVIMENTI_DETTAGLI_TECNICI", 0, 200000000, objParametriServer)
                mov_dettaglio_tecnico.Id_Reg_Dettaglio = Id_Reg_Dettaglio

            Else

                Id_Reg_Dettaglio = mov_dettaglio_tecnico.Id_Reg_Dettaglio

                Dim movimentiCount = From a In GiasContext.Mov_Dettaglio_Tecnico
                                     Where a.Id_Agenda = mov_dettaglio_tecnico.Id_Agenda AndAlso
                                           a.Id_Mov = mov_dettaglio_tecnico.Id_Mov AndAlso
                                           a.Id_Mov_Det = mov_dettaglio_tecnico.Id_Mov_Det AndAlso
                                           a.Id_Reg_Dettaglio = Id_Reg_Dettaglio AndAlso
                                           a.Piva = mov_dettaglio_tecnico.Piva AndAlso
                                           a.Sa_Cod = mov_dettaglio_tecnico.Sa_Cod
                                     Select a
                If movimentiCount.Count > 0 Then
                    esiste = True
                Else
                    'Ho mantenuto ID_Agenda, ID_Mov e Id_Mov_Det ma è cambiata la chiave: PIVA o Sa_Cod
                    cambioChiave = True
                End If

            End If

            If cambioChiave Then
                Dim movimentiDel = From a In GiasContext.Mov_Dettaglio_Tecnico
                                   Where a.Id_Agenda = mov_dettaglio_tecnico.Id_Agenda AndAlso
                                         a.Id_Mov = mov_dettaglio_tecnico.Id_Mov AndAlso
                                         a.Id_Mov_Det = mov_dettaglio_tecnico.Id_Mov_Det AndAlso
                                         a.Id_Reg_Dettaglio = Id_Reg_Dettaglio
                                   Select a

                For Each mov In movimentiDel
                    GiasContext.Mov_Dettaglio_Tecnico.Remove(mov)
                    GiasContext.SaveChanges()
                Next

            End If

            Dim movimentiW As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W
            If esiste Then
                movimentiW.Modifica(mov_dettaglio_tecnico, GiasContext, objParametriServer)
            Else
                movimentiW.Scrivi(mov_dettaglio_tecnico, GiasContext, objParametriServer)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Id_Reg_Dettaglio

    End Function

    Public Sub Elimina(ByRef Movi_dettaglio_tecnico As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico(),
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabBIZ.Mov_Dettaglio_Tecnico_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try

            Dim movimentiW As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W
            For Each Movimento In Movi_dettaglio_tecnico
                movimentiW.Elimina(Movimento, GiasContext, objParametriServer)
            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

End Class
