
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
Imports Newtonsoft.Json.Linq

Public Class Lotto_AssegnazioneUC

    Inherits System.Web.UI.UserControl
    Public Tipo_Lotto As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Ricordarsi quando si include questo User Control nella pagina di aggiungere il Tipo_Lotto tra i parametri
        'del tag UC.
        'Se Tipo_Lotto = 'E' (Entrata) Carico la grid per i Conferimenti.
        'Se Tipo_Lotto = 'L' (Lavorazioni) Carico la grid per le Lavorazioni.

        If String.IsNullOrEmpty(Tipo_Lotto) Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
        Else
            hf_Tipo_Lotto.Value = Tipo_Lotto
        End If

    End Sub


    Public Shared Function Carica_Lotto_AssegnaxRisumSpeVarQualCert(ByVal piva As String, ByVal tipo_lotto As String) As RispostaStandard

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
                l.Leggi(piva, tipo_lotto, "", "", objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function AggiornaLotto_AssegnaxRisumSpeVarQualCert(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard

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