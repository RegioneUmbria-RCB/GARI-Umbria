Imports System.Web
Imports System.Web.SessionState
Imports System.Diagnostics
Imports System.Web.UI

Public Class Custom500
    Inherits System.Web.UI.Page

    Public messaggio As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim msgErrore As String = ""

        If (Not (TypeOf Context.Handler Is IRequiresSessionState OrElse
        TypeOf Context.Handler Is IReadOnlySessionState)) OrElse
        IsNothing(Session("ASG_objParametri_Utenti")) Then

            Ridirezione()
            msgErrore = "Sessione Scaduta."
            Exit Sub

        End If

        Try

            If Not Session("objTrace") Is Nothing Then

                Dim Eccezione As Exception = Session("objTrace").GetBaseException()

                'uso questa funzione per ottenere il Messaggio..:
                Dim MessaggioErroreDettaglio As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(Eccezione, True, aCapo:="<br />", source:=True)

                msgErrore = "Si è verificato un errore.<br><h3>Se il problema persiste contattare l'assistenza allegando il messaggio riportato sotto.<br><b>Ricordarsi di indicare i dati necessari per la rigenerazione dell'errore</b></h3>"

                phDettaglioErrore.Controls.Add(New LiteralControl(MessaggioErroreDettaglio))

                Session("objTrace") = Nothing

            ElseIf Request.QueryString("UtenteAbilitato_Lettura") <> "" Then

                msgErrore = "Non si dispone dei permessi GIAS. Usare un utente differente o rivolgersi all'assistenza"

            Else
                Ridirezione()
            End If

        Catch ex As Exception
            If Not Debugger.IsAttached Then
                Ridirezione()
            End If
        End Try

        errore.Text = msgErrore

    End Sub

    Private Sub Ridirezione()
        phChiamataGoURL.Controls.Add(New LiteralControl("<script>goURL();</script>"))
    End Sub

End Class