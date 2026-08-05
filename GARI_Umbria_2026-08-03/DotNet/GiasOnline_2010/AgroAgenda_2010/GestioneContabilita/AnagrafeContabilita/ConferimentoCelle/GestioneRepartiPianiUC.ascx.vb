Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class GestioneRepartiPianiUC
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Friend Shared Function LeggiCentriAziendali(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objReparti As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim DT As DataTable = objReparti.Leggi(piva, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                    "", "", objParametri_Server)

            Dim colonneTenere As String() = {"Sa_Cod", "Sa_Des"}
            DT.Columns("sa_cod").ColumnName = "Sa_Cod"
            DT.Columns("sa_nome").ColumnName = "Sa_Des"

            Dim daRimuovere = DT.Columns.Cast(Of DataColumn) _
                .Where(Function(c) Not colonneTenere.Contains(c.ColumnName)) _
                .Select(Function(c) c.ColumnName) _
                .ToArray()

            For Each col As String In daRimuovere
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

    Friend Shared Function Leggi_RepartiPiani(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objReparti As New AgronicaCoreAnagrafeDAL.Cantina_Caratter_R
            Dim DT As DataTable = objReparti.Leggi(piva, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    "", "", objParametri_Server)

            DT.Columns("sa_cod").ColumnName = "Sa_Cod"
            DT.Columns.Add("IdParam", GetType(String))

            For Each row As DataRow In DT.Rows
                row("IdParam") = row("PIVA") & "_" & row("Sa_Cod") & "_" & row("Piano_Cod")
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

    Friend Shared Function AggiornaDaGrigliaReparti(ByVal piva As String,
                                                    ByVal righeInserite As String,
                                                    ByVal righeModificate As String,
                                                    ByVal righeCancellate As String,
                                                    ByVal tutteleRighe As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objReparti As New AgronicaCoreAnagrafeDAL.Cantina_Caratter_W
            r.RispostaStringa = objReparti.AggiornaRecordParametriModificati(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe, objParametri_Server)

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