
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeBIZ
Imports AgroAgenda_2010.Resources

Public Class Lotto_AssegnaxRisumSpeVarQualCertUC
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Friend Shared Function Carica_Lotto_AssegnaxRisumSpeVarQualCert(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            'TODO
            Dim l As New Lotto_AssegnaxRisumSpeVarQualCert_DAL_R
            r.RispostaStringa =
                l.Leggi(piva, "E", "", "", objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Friend Shared Function AggiornaLotto_AssegnaxRisumSpeVarQualCert(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim scrivi As New Lotto_AssegnaxRisumSpeVarQualCert_BIZ
            r.RispostaStringa =
                scrivi.Aggiorna_LottoAssegna(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)
        End Try

        Return r
    End Function

End Class