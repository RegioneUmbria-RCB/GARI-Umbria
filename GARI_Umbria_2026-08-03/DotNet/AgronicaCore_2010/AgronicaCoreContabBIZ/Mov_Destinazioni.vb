Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework

Public Class Mov_Destinazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

End Class

Public Class Mov_Destinazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Scrivi_Modifica(ByVal Mov_Destinazioni As AgronicaCoreEntityFramework_POCO.Mov_Destinazioni,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByRef objParametriServer As AgronicaCoreParametri,
                                    Optional BypassControlloEsite As Boolean = False
                                    ) As Integer

        Const nomeRoutine = "ContabBIZ.Mov_Destinazioni_W.Scrivi_Modifica()"
        Dim messaggioErrore As String = ""

        Try
            Dim agroDP As New Agro_Sequenze
            Dim esiste = False
            
            if Not bypassControlloEsite Then
                ' Controllo se esiste già un movimento con gli stessi parametri
                Dim movimentiCount = From a In GiasContext.Mov_Destinazioni
                                     Where a.Id_Agenda = Mov_Destinazioni.Id_Agenda And
                                           a.Id_Mov = Mov_Destinazioni.Id_Mov And
                                           a.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det And
                                           a.Appezza = Mov_Destinazioni.Appezza And
                                           a.Id_Destinazione = Mov_Destinazioni.Id_Destinazione
                                     Select a

                If movimentiCount.Count > 0 Then
                    esiste = True
                End If
            End If

            'Dim mov_dels = From a In GiasContext.Mov_Destinazioni
            '               Where a.Id_Agenda = Mov_Destinazioni.Id_Agenda And
            '                                  a.Id_Mov = Mov_Destinazioni.Id_Mov And
            '                                  a.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det And
            '                                  a.Appezza = Mov_Destinazioni.Appezza And
            '                                  a.Id_Destinazione <> Mov_Destinazioni.Id_Destinazione
            '               Select a

            'For Each mov_del In mov_dels
            '    GiasContext.Mov_Destinazioni.DeleteObject(mov_del)
            'Next

            'GiasContext.SaveChanges()

            Dim movimentiW As New AgronicaCoreContabDAL.Mov_Destinazioni_W

            If esiste Then
                movimentiW.Modifica(Mov_Destinazioni, GiasContext, objParametriServer)
            Else
                movimentiW.Scrivi(Mov_Destinazioni, GiasContext, objParametriServer)
            End If

            Return 0

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Function

    Public Sub Elimina(ByRef Mov_Destinazioni As AgronicaCoreEntityFramework_POCO.Mov_Destinazioni(),
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabBIZ.Mov_Destinazioni_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try
            Dim movimentiW As New AgronicaCoreContabDAL.Mov_Destinazioni_W
            For Each Movimento In Mov_Destinazioni
                movimentiW.Elimina(Movimento, GiasContext, objParametriServer)
            Next
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

End Class