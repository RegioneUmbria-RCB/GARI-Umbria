Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreContabDAL

Public Class IstitutiCreditoUC
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Friend Shared Function Leggi_IstitutiCredito(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim leggiIstituti As New Ist_Credito_R
            DT = leggiIstituti.Leggi(-1, 0, 0, "", "Cod_Istituto, Istituto_Des", objParametri_Server)

            DT.Columns.Add("IdParam", GetType(String))
            DT.Columns("Cod_Istituto").ColumnName = "Istituto_Cod"
            For Each row As DataRow In DT.Rows
                row("IdParam") = $"{row("Piva")}_{row("Istituto_Cod")}"
            Next

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Friend Shared Function AggiornaDaGrigliaIstitutiCredito(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objIstituto As New Ist_Credito_W
            r.RispostaStringa = objIstituto.AggiornaRecordParametriModificati(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe, objParametri_Server)

            If String.IsNullOrEmpty(r.RispostaStringa) Then
                r.RispostaOK = True
            Else
                Throw New Exception(r.RispostaStringa)
            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)
        End Try

        Return r
    End Function


End Class