Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Web.Services

Public Class SchedaCatastoUtilizzi_Filtro
    Inherits System.Web.UI.Page

    'Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    'Dim Piva As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        ' Master()..Text = "Esportazione conferimenti in formato CSV"

        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                               AgroKey_EncoderDecoder, _
                               Server)

        ' objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


    End Sub



    <WebMethod(EnableSession:=True)> _
    Public Shared Function Stampa(ByVal piva As String, ByVal dataDa As String, ByVal dataA As String) As AgronicaCoreVarieBIZ.rispostaStandard

        Dim r As New AgronicaCoreVarieBIZ.rispostaStandard
        'Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        'OrElse IsNothing(objParametri_Server)
        If HttpContext.Current.Session.IsNewSession Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim URL As String = "SchedaCatastoUtilizzi.aspx?p=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder) & _
                                                                "&di=" & Stringa_Codifica(dataDa, AgroKey_EncoderDecoder) & _
                                                                "&df=" & Stringa_Codifica(dataA, AgroKey_EncoderDecoder)

            r.RispostaOK = True
            r.RispostaStringa = url

        Catch ex As Exception
            r.RispostaOK = False
            r.RispostaStringa = ""
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & _
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function



End Class