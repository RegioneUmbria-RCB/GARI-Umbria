Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreContabDAL
Imports AgronicaCoreAnagrafeDAL

Public Class LiquiditaUC
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Friend Shared Function ElencoIstitutiCredito(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim leggiIstituti As New Ist_Credito_R
            DT = leggiIstituti.Leggi(-1, 0, 0, "", "", objParametri_Server)

            DT.Columns("Cod_Istituto").ColumnName = "Istituto_Cod"
            Dim colonneDaTenere As String() = {"Istituto_Cod", "Istituto_Des"}
            Dim colonneDaRimuovere = DT.Columns.OfType(Of DataColumn) _
                .Where(Function(c) Not colonneDaTenere.Contains(c.ColumnName)) _
                .ToArray()
            For Each col In colonneDaRimuovere
                DT.Columns.Remove(col)
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

    Friend Shared Function Leggi_Liquidita(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim leggiLiquidita As New Liquidita_R
            DT = leggiLiquidita.Leggi("", $"Riferimento = '{piva}' AND Cod_Istituto > 0", "", objParametri_Server)

            DT.Columns("Cau_Risorsa").ColumnName = "Risorsa_Cod"
            DT.Columns("Cod_Istituto").ColumnName = "Istituto_Cod"
            DT.Columns("Cod_Contatto").ColumnName = "Contatto_Cod"
            DT.Columns("ChkDefault").ColumnName = "Default_Cod"

            DT.Columns.Add("IdParam", GetType(String))
            For Each row As DataRow In DT.Rows
                row("IdParam") = $"{row("Piva")}_{row("Cod_Liquidita")}"
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

    Friend Shared Function AggiornaDaGrigliaLiquidita(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objLiquidita As New Liquidita_W
            r.RispostaStringa = objLiquidita.AggiornaRecordParametriModificati(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe, objParametri_Server)

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