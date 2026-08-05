Imports System.Web
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ

Public Class Localization
    Inherits System.Web.UI.Page

    <WebMethod(EnableSession:=True)>
    Public Shared Function RitornaRisorseBS(ByVal files As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..

            r.RispostaOK = True
            Dim linguaSession As Lingua = CType(System.Web.HttpContext.Current.Session("LinguaCorrente"), Lingua)
            Dim p As String = ""
            If Not files.ToLower().Contains(".dll") Then
                p = HttpContext.Current.Server.MapPath("~\" & files)
            Else
                p = HttpContext.Current.Server.MapPath(".") & "\bin"
            End If
            r.RispostaStringa = AgronicaCoreUtility.Localization.RitornaRisorse(files, linguaSession, p)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                       AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class