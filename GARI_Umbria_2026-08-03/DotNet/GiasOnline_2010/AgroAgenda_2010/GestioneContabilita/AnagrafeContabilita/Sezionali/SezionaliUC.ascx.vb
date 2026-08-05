Imports AgroAgenda_2010.Resources
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports OutData.Metaschema

Public Class SezionaliUC
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Friend Shared Function Leggi_RegimiFiscale() As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New List(Of ListaRegimiFiscali)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim rfDal = New RegimiFiscali_R()
            DT = rfDal.LeggiRegimiFiscali(objParametri_Server)

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

    Friend Shared Function Leggi_Sezionali(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim leggiImpreseSezionali As New Imprese_Sezionali_R
            DT = leggiImpreseSezionali.Leggi_ImpreseSezionali(piva, objParametri_Server)

            DT.Columns.Add("IdParam", GetType(String))
            DT.Columns("RegimeFiscale_Cod").ColumnName = "Regime_Fiscale_Cod"
            DT.Columns("EsigibilitaIva").ColumnName = "Esigibilita_Iva_Cod"
            DT.Columns("Fatturazione").ColumnName = "Fatturazione_Elettronica_Cod"

            For Each row As DataRow In DT.Rows
                If IsDBNull(row("Sezionale_Cod")) Then
                    row("Sezionale_Cod") = 0
                End If
                row("IdParam") = $"{row("Piva")}_{row("Sezionale_Cod")}"
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

    Friend Shared Function AggiornaDaGrigliaSezionali(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objImpreseSezionali As New Imprese_Sezionali_W
            r.RispostaStringa = objImpreseSezionali.AggiornaRecordParametriModificati(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe, objParametri_Server)

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