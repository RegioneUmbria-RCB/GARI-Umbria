Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Agenda_W : Inherits DALBase

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Sub Aggiorna_MVV(ByVal piva As String, ByVal idAgenda As Integer, ByVal numeroMVV As String)

        Dim nomeProcedura As String = "Agenda_W.Aggiorna_MVV"
        Dim StrSQL As New Text.StringBuilder

        Try

            StrSQL.AppendLine(" UPDATE movimenti  ")
            StrSQL.AppendLine(" set doc_numero_Sin = '" & Agro_SQL_SaveText(numeroMVV) & "' ")
            StrSQL.AppendLine(" Where movimenti.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            StrSQL.AppendLine(" And movimenti.id_agenda = " & Agro_SQL_SaveNum(idAgenda))

            EseguiQuery_Scrittura(_objParametriServer, StrSQL.ToString, nomeProcedura)

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try

    End Sub

End Class