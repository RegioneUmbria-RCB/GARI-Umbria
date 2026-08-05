
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreContabDAL

Public Class GruppiReferenzeUC
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Friend Shared Function Leggi_GruppiReferenze(piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim leggiAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_R
            Dim moduliConferimento = leggiAnagrafeLog.LeggiModuliParametriQualitativi(piva, objParametri_Server)


            Dim objOModuli_Referenze_Config_Testata = New AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_R
            DT = objOModuli_Referenze_Config_Testata.LeggiModuliConferimento(piva, objParametri_Server)

            DT.Columns.Add("IdParam", GetType(String))
            For Each col As DataColumn In moduliConferimento.Columns
                DT.Columns.Add(col.ColumnName, col.DataType)
            Next

            For Each row As DataRow In DT.Rows
                row("IdParam") = $"{row("Piva")}_{row("Modulo_Generazione")}_{row("Id_Testata")}"

                Dim mc As DataRow = moduliConferimento.AsEnumerable() _
                    .FirstOrDefault(Function(x) CInt(x("Modulo_Cod")) = CInt(row("Modulo_Generazione")))

                If Not IsNothing(mc) Then
                    For Each col As DataColumn In moduliConferimento.Columns
                        row(col.ColumnName) = mc(col.ColumnName)
                    Next
                End If
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

    Friend Shared Function AggiornaDaGrigliaGruppiReferenze(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim testata = New AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_W
            r.RispostaStringa = testata.AggiornaRecordModificati(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe, objParametri_Server)

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